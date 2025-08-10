using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a single inventory slot that can hold an item and display its icon, count, and tooltip.
/// </summary>
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// ===== INSTANCE VARIABLES =====
    /// <summary>
    /// Identifies the position (row/column) of the slot in the inventory grid.
    /// </summary>
    [SerializeField]
    private SlotID slotID;

    /// <summary>
    /// Identifies the position of the slot in the hot bar grid.
    /// </summary>
    [SerializeField]
    [Range(0, 3)]
    private int hotBarIndex;

    /// <summary>
    /// UI element shown/hidden based on whether the slot is empty.
    /// </summary>
    [SerializeField]
    private GameObject isEmptyObject;

    /// <summary>
    /// Image component used to display the item icon.
    /// </summary>
    [SerializeField]
    private Image slotImage;

    /// <summary>
    /// Text component used to display the current item count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI slotCountText;

    /// <summary>
    /// Reference to the tooltip UI for showing item details.
    /// </summary>
    private TooltipUI tooltip;

    /// <summary>
    /// The item currently held in the slot.
    /// </summary>
    private Item item;

    /// ===== INSTANCE VARIABLES =====
    /// <summary>
    /// The current count of the item in the slot.
    /// </summary>
    private int count;

    /// ===== BOOLEANS =====
    /// <summary>
    /// Indicates whether the slot is currently empty.
    /// </summary>
    [SerializeField]
    private bool isEmpty = true;

    /// <summary>
    /// Initializes references to required components (e.g., TooltipUI).
    /// </summary>
    void Start()
    {
        tooltip = FindAnyObjectByType<TooltipUI>();
    }

    /// <summary>
    /// Sets the item and count for the slot and updates its visuals.
    /// </summary>
    /// <param name="item">The item to assign to the slot.</param>
    /// <param name="count">The number of items in the slot.</param>
    public void SetItem(Item item, int count)
    {
        isEmpty = false;
        if (isEmptyObject != null)
            isEmptyObject.SetActive(true);

        slotImage.sprite = item.icon;
        this.item = item;

        if (item.maxStack > 1)
            slotCountText.text = count.ToString() + "/" + item.maxStack.ToString();

        this.count = count;
    }

    /// <summary>
    /// Clears the slot, removing any item and resetting the UI.
    /// </summary>
    public void ClearSlot()
    {
        isEmpty = true;
        if (isEmptyObject != null)
            isEmptyObject.SetActive(false);

        slotImage.sprite = null;
        slotCountText.text = "";
        this.item = null;
        this.count = 0;
    }

    /// <summary>
    /// Returns whether the slot is empty.
    /// </summary>
    public bool getIsEmpty()
    {
        return isEmpty;
    }

    /// <summary>
    /// Sets the empty status of the slot.
    /// </summary>
    /// <param name="isEmpty">Whether the slot is empty.</param>
    public void setEmpty(bool isEmpty)
    {
        this.isEmpty = isEmpty;
    }

    /// <summary>
    /// Returns the index of the slot in the hot bar grid.
    /// </summary>
    public int getHotBarIndex()
    {
        return hotBarIndex;
    }

    /// <summary>
    /// Returns the column index of the slot.
    /// </summary>
    public int getColumn()
    {
        return slotID.getColumn();
    }

    /// <summary>
    /// Returns the row index of the slot.
    /// </summary>
    public int getRow()
    {
        return slotID.getRow();
    }

    /// <summary>
    /// Returns the full SlotID object of the slot.
    /// </summary>
    public SlotID getSlotID()
    {
        return slotID;
    }

    /// <summary>
    /// Returns the item currently in the slot.
    /// </summary>
    public Item getItem()
    {
        return item;
    }

    /// <summary>
    /// Returns the count of items in the slot.
    /// </summary>
    public int getCount()
    {
        return count;
    }

    /// <summary>
    /// Adds to the item count and updates the display text.
    /// </summary>
    /// <param name="count">Amount to add.</param>
    public void addCount(int count)
    {
        this.count += count;
        changeCountText();
    }

    /// <summary>
    /// Removes from the item count and updates the display text.
    /// </summary>
    /// <param name="count">Amount to remove.</param>
    public void removeCount(int count)
    {
        this.count -= count;
        changeCountText();
    }

    /// <summary>
    /// Updates the slot's count text UI.
    /// </summary>
    private void changeCountText()
    {
        if (item.maxStack > 1)
            slotCountText.text = count.ToString() + "/" + item.maxStack.ToString();
    }

    /// <summary>
    /// Shows the item tooltip when the pointer enters the slot area.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && !isEmpty)
        {
            Vector2 mousePos = Input.mousePosition;
            if (tooltip != null)
                tooltip.ShowTooltip(item, mousePos);
        }
    }

    /// <summary>
    /// Hides the item tooltip when the pointer exits the slot area.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.HideTooltip();
    }
}
