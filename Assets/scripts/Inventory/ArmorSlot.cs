using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents an individual armor slot in the inventory system, managing the display
/// and interaction of equipped cosmetic items. Handles tooltip display and visual
/// state management for armor slots including helmet, chestplate, leggings, and boots.
/// </summary>
public class ArmorSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Configuration
    /// <summary>
    /// The type of armor that can be equipped in this slot (helmet, chestplate, leggings, boots).
    /// Determines which cosmetic items can be placed in this slot.
    /// </summary>
    [SerializeField]
    private SlotArmorType slotArmorType;
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
    /// Shows the visual representation of the equipped armor item.
    /// </summary>
    [SerializeField]
    private Image armorImage;

    /// <summary>
    /// The tooltip UI component for displaying item information on hover.
    /// </summary>
    private TooltipUI tooltip;
    #endregion

    #region Item State
    /// <summary>
    /// The item currently held in the slot.
    /// Contains the equipped cosmetic item data and properties.
    /// </summary>
    private Item item;

    /// <summary>
    /// Whether the slot is currently empty.
    /// Used to determine visual state and interaction behavior.
    /// </summary>
    private bool isEmpty = true;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes component references and sets up the tooltip system.
    /// </summary>
    void Start()
    {
        // Find tooltip component for item information display
        tooltip = FindAnyObjectByType<TooltipUI>();
    }
    #endregion

    #region Item Management
    /// <summary>
    /// Sets an item in the armor slot, updating the visual display and state.
    /// If null is passed, the item is removed from the slot.
    /// </summary>
    /// <param name="item">The item to place in the slot, or null to remove.</param>
    public void setItem(Item item)
    {
        if (item == null)
        {
            // Remove item if null is passed
            removeItem();
            return;
        }

        // Set item and update visual state
        this.item = item;
        armorImage.sprite = item.icon;
        isEmptyObject.SetActive(true);
        isEmpty = false;
    }

    /// <summary>
    /// Gets the item currently equipped in this armor slot.
    /// </summary>
    /// <returns>The equipped item, or null if slot is empty.</returns>
    public Item getItem()
    {
        return item;
    }

    /// <summary>
    /// Removes the item from the armor slot, clearing the visual display and state.
    /// Resets the slot to empty status.
    /// </summary>
    public void removeItem()
    {
        // Clear item data and reset visual state
        item = null;
        armorImage.sprite = null;
        isEmptyObject.SetActive(false);
        isEmpty = true;
    }
    #endregion

    #region State Queries
    /// <summary>
    /// Gets the armor type that this slot accepts.
    /// </summary>
    /// <returns>The SlotArmorType defining what can be equipped in this slot.</returns>
    public SlotArmorType getSlotArmorType()
    {
        return slotArmorType;
    }

    /// <summary>
    /// Checks if the armor slot is currently empty.
    /// </summary>
    /// <returns>True if the slot contains no item, false otherwise.</returns>
    public bool getIsEmpty()
    {
        return isEmpty;
    }
    #endregion

    #region Tooltip Interaction
    /// <summary>
    /// Shows the item tooltip when the pointer enters the slot area.
    /// Displays item information for equipped armor items.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show tooltip only if slot contains an item
        if (item != null && !isEmpty)
        {
            Vector2 mousePos = Input.mousePosition;
            tooltip.ShowTooltip(item, mousePos);
        }
    }

    /// <summary>
    /// Hides the item tooltip when the pointer exits the slot area.
    /// Ensures tooltip is hidden when not hovering over the slot.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
    #endregion
}
