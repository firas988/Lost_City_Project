using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the hotbar display within the inventory interface, providing a synchronized
/// view of the hotbar items. Ensures consistency between the main hotbar and inventory
/// hotbar representations.
/// </summary>
public class HotBarHandlerInInventory : MonoBehaviour
{
    #region UI Components
    /// <summary>List of UI slot components that make up the inventory hotbar interface.</summary>
    [SerializeField]
    private List<Slot> slots;
    #endregion

    #region Component References
    /// <summary>Reference to the InventoryManager for accessing hotbar data and operations.</summary>
    private InventoryManager inventoryManager;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes component references and establishes connection to the inventory manager.
    /// </summary>
    void Start()
    {
        // Get reference to inventory manager for hotbar operations
        inventoryManager = GetComponent<InventoryManager>();
    }
    #endregion

    #region UI Update Methods
    /// <summary>
    /// Updates the entire inventory hotbar UI to reflect the current hotbar state.
    /// Iterates through all slots and updates them with current item data.
    /// </summary>
    public void updateHotBarInInventory()
    {
        // Update each slot in the inventory hotbar
        for (int i = 0; i < slots.Count; i++)
        {
            // Get items for current slot
            List<Item> items = inventoryManager.GetItemFromHotBar(slots[i].getHotBarIndex());

            if (items.Count > 0)
            {
                // Set slot with item and count
                slots[i].SetItem(items[0], items.Count);
            }
            else
            {
                // Clear slot if no items
                slots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// Updates a specific inventory hotbar slot UI based on the provided index.
    /// Finds the corresponding slot and updates it with current item data.
    /// </summary>
    /// <param name="index">The hotbar slot index to update.</param>
    public void updateHotBarInInventoryUI(int index)
    {
        // Get items for specified slot
        List<Item> items = inventoryManager.GetItemFromHotBar(index);

        // Find the corresponding UI slot
        Slot slot = slots.Find(slot => slot.getHotBarIndex() == index);

        if (items.Count > 0)
        {
            // Set slot with item and count
            slot.SetItem(items[0], items.Count);
        }
        else
        {
            // Clear slot if no items
            slot.ClearSlot();
        }
    }
    #endregion
}
