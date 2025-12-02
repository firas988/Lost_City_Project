using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a single inventory slot that can hold an item and display its icon, count, and tooltip.
/// Manages the visual representation and state of items within the inventory grid system.
/// Integrates with tooltip system for item information display on hover.
/// </summary>
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Slot Identification
    /// <summary>
    /// Identifies the position (row/column) of the slot in the inventory grid.
    /// Used for locating slots in the 2D inventory array.
    /// </summary>
    [SerializeField]
    private SlotID slotID;

    /// <summary>
    /// Identifies the position of the slot in the hot bar grid.
    /// Range 0-3: 0 for weapon slot, 1-3 for consumable slots.
    /// </summary>
    [SerializeField]
    [Range(0, 3)]
    private int hotBarIndex;
    #endregion

    #region UI Components
    /// <summary>
    /// UI element shown/hidden based on whether the slot is empty.
    /// Provides visual feedback about the slot's current state.
    /// </summary>
    [SerializeField]
    private GameObject isEmptyObject;

    /// <summary>
    /// Image component used to display the item icon.
    /// Shows the visual representation of the stored item.
    /// </summary>
    [SerializeField]
    private Image slotImage;

    /// <summary>
    /// Text component used to display the current item count.
    /// Shows stack information for items that can be stacked.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI slotCountText;

    /// <summary>
    /// Reference to the tooltip UI for showing item details on hover.
    /// </summary>
    private TooltipUI tooltip;
    #endregion

    #region Item State
    /// <summary>
    /// The item currently held in the slot.
    /// Contains the item data and properties for display and interaction.
    /// </summary>
    private Item item;

    /// <summary>
    /// The current count of the item in the slot.
    /// Represents the number of items in the stack.
    /// </summary>
    private int count;

    /// <summary>
    /// Indicates whether the slot is currently empty.
    /// Used to determine visual state and interaction behavior.
    /// </summary>
    [SerializeField]
    private bool isEmpty = true;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references to required components and sets up the tooltip system.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(1)
    void Start()
    {
        // Find tooltip component for item information display
        tooltip = transform.parent.parent.GetComponentInChildren<TooltipUI>();
    }
    #endregion

    #region Item Management
    /// <summary>
    /// Sets the item and count for the slot and updates its visuals.
    /// Handles both single items and stacked items with appropriate display logic.
    /// </summary>
    /// <param name="item">The item to assign to the slot.</param>
    /// <param name="count">The number of items in the slot.</param>
    // COMPLEXITY ANALYSIS: SetItem() - O(1)
    public void SetItem(Item item, int count)
    {
        // Update slot state and visual indicators
        isEmpty = false;
        if (isEmptyObject != null)
            isEmptyObject.SetActive(true);

        // Set item icon and store reference
        slotImage.sprite = item.getIcon();
        this.item = item;

        // Update count text for stackable items
        if (item.getMaxStack() > 1)
            slotCountText.text = count.ToString() + "/" + item.getMaxStack().ToString();

        // Store count and update internal state
        this.count = count;
    }

    /// <summary>
    /// Clears the slot, removing any item and resetting the UI.
    /// Resets all visual elements and internal state to empty.
    /// </summary>
    // COMPLEXITY ANALYSIS: ClearSlot() - O(1)
    public void ClearSlot()
    {
        // Reset slot state and visual indicators
        isEmpty = true;
        if (isEmptyObject != null)
            isEmptyObject.SetActive(false);

        // Clear visual elements
        slotImage.sprite = null;
        slotCountText.text = "";

        // Reset internal state
        this.item = null;
        this.count = 0;
    }
    #endregion

    #region State Queries
    /// <summary>
    /// Returns whether the slot is currently empty.
    /// </summary>
    /// <returns>True if the slot contains no item, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getIsEmpty() - O(1)
    public bool getIsEmpty()
    {
        return isEmpty;
    }

    /// <summary>
    /// Sets the empty status of the slot.
    /// </summary>
    /// <param name="isEmpty">Whether the slot should be considered empty.</param>
    // COMPLEXITY ANALYSIS: setEmpty() - O(1)
    public void setEmpty(bool isEmpty)
    {
        this.isEmpty = isEmpty;
    }

    /// <summary>
    /// Returns the index of the slot in the hot bar grid.
    /// </summary>
    /// <returns>The hot bar index (0-3) of this slot.</returns>
    // COMPLEXITY ANALYSIS: getHotBarIndex() - O(1)
    public int getHotBarIndex()
    {
        return hotBarIndex;
    }

    /// <summary>
    /// Returns the column index of the slot in the inventory grid.
    /// </summary>
    /// <returns>The column index of this slot.</returns>
    // COMPLEXITY ANALYSIS: getColumn() - O(1)
    public int getColumn()
    {
        return slotID.getColumn();
    }

    /// <summary>
    /// Returns the row index of the slot in the inventory grid.
    /// </summary>
    /// <returns>The row index of this slot.</returns>
    // COMPLEXITY ANALYSIS: getRow() - O(1)
    public int getRow()
    {
        return slotID.getRow();
    }

    /// <summary>
    /// Returns the full SlotID object containing position information.
    /// </summary>
    /// <returns>The SlotID object with row and column coordinates.</returns>
    // COMPLEXITY ANALYSIS: getSlotID() - O(1)
    public SlotID getSlotID()
    {
        return slotID;
    }

    /// <summary>
    /// Returns the item currently stored in the slot.
    /// </summary>
    /// <returns>The item object, or null if slot is empty.</returns>
    // COMPLEXITY ANALYSIS: getItem() - O(1)
    public Item getItem()
    {
        return item;
    }

    /// <summary>
    /// Returns the current count of items in the slot.
    /// </summary>
    /// <returns>The number of items in the stack.</returns>
    // COMPLEXITY ANALYSIS: getCount() - O(1)
    public int getCount()
    {
        return count;
    }
    #endregion

    #region Count Management
    /// <summary>
    /// Adds to the item count and updates the display text.
    /// Automatically refreshes the count display for stackable items.
    /// </summary>
    /// <param name="count">Amount to add to the current count.</param>
    // COMPLEXITY ANALYSIS: addCount() - O(1)
    public void addCount(int count)
    {
        // Update count and refresh display
        this.count += count;
        changeCountText();
    }

    /// <summary>
    /// Removes from the item count and updates the display text.
    /// Automatically refreshes the count display for stackable items.
    /// </summary>
    /// <param name="count">Amount to remove from the current count.</param>
    // COMPLEXITY ANALYSIS: removeCount() - O(1)
    public void removeCount(int count)
    {
        // Update count and refresh display
        this.count -= count;
        changeCountText();
    }

    /// <summary>
    /// Updates the slot's count text UI to reflect the current item count.
    /// Only displays count for items that can be stacked (maxStack > 1).
    /// </summary>
    // COMPLEXITY ANALYSIS: changeCountText() - O(1)
    private void changeCountText()
    {
        // Update count text for stackable items
        if (item.getMaxStack() > 1)
            slotCountText.text = count.ToString() + "/" + item.getMaxStack().ToString();
    }
    #endregion

    #region Tooltip Interaction
    /// <summary>
    /// Shows the item tooltip when the pointer enters the slot area.
    /// Displays item information for non-empty slots with valid items.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    // COMPLEXITY ANALYSIS: OnPointerEnter() - O(1)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show tooltip only if slot contains an item
        if (item != null && !isEmpty)
        {
            Vector2 mousePos = Input.mousePosition;
            if (tooltip != null)
                tooltip.ShowTooltip(item, mousePos);
        }
    }

    /// <summary>
    /// Hides the item tooltip when the pointer exits the slot area.
    /// Ensures tooltip is hidden when not hovering over the slot.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    // COMPLEXITY ANALYSIS: OnPointerExit() - O(1)
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide tooltip when leaving slot area
        if (tooltip != null)
            tooltip.HideTooltip();
    }
    #endregion
}
