using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    /// <summary>
    /// The type of armor that can be equipped in this slot.
    /// </summary>
    private SlotArmorType slotArmorType;

    /// <summary>
    /// UI element shown/hidden based on whether the slot is empty.
    /// </summary>
    [SerializeField]
    private GameObject isEmptyObject;

    /// <summary>
    /// Image component used to display the item icon.
    /// </summary>
    [SerializeField]
    private Image armorImage;

    /// <summary>
    /// The item currently held in the slot.
    /// </summary>
    private Item item;

    /// <summary>
    /// Whether the slot is empty.
    /// </summary>
    private bool isEmpty = true;

    /// <summary>
    /// The tooltip UI component.
    /// </summary>
    private TooltipUI tooltip;

    void Start()
    {
        tooltip = FindAnyObjectByType<TooltipUI>();
    }

    public void setItem(Item item)
    {
        if (item == null)
        {
            removeItem();
            return;
        }
        this.item = item;
        armorImage.sprite = item.icon;
        isEmptyObject.SetActive(true);
        isEmpty = false;
    }

    public Item getItem()
    {
        return item;
    }

    public void removeItem()
    {
        item = null;
        armorImage.sprite = null;
        isEmptyObject.SetActive(false);
        isEmpty = true;
    }

    public SlotArmorType getSlotArmorType()
    {
        return slotArmorType;
    }

    public bool getIsEmpty()
    {
        return isEmpty;
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
            tooltip.ShowTooltip(item, mousePos);
        }
    }

    /// <summary>
    /// Hides the item tooltip when the pointer exits the slot area.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}
