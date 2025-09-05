using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the main hotbar UI and handles input for using consumable items.
/// Coordinates between input system, inventory manager, and UI slots for seamless
/// hotbar functionality including potion usage and UI updates.
/// </summary>
public class HotBarHandler : MonoBehaviour
{
    #region UI Components
    /// <summary>List of UI slot components that make up the hotbar interface.</summary>
    [SerializeField]
    private List<Slot> slots;
    #endregion

    #region Component References
    /// <summary>Reference to the InventoryManager for accessing hotbar data and operations.</summary>
    private InventoryManager inventoryManager;

    /// <summary>Reference to the HotBarHandlerInInventory for synchronizing inventory hotbar display.</summary>
    private HotBarHandlerInInventory hotBarHandlerInInventory;

    /// <summary>Reference to the InputListener for detecting hotbar key presses.</summary>
    private InputListener inputListener;

    /// <summary>Reference to the PotionHandler for processing potion usage effects.</summary>
    private PotionHandler potionHandler;
    #endregion

    #region Configuration
    /// <summary>Tag identifier for finding the player GameObject.</summary>
    private string playerTag = "Player";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes component references and establishes connections to required systems.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(1)
    void Start()
    {
        // Get required components
        inventoryManager = GetComponent<InventoryManager>();
        inputListener = transform.parent.GetComponentInChildren<InputListener>();
        potionHandler = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PotionHandler>();
        hotBarHandlerInInventory = GetComponent<HotBarHandlerInInventory>();
    }

    /// <summary>
    /// Checks for hotbar key presses and attempts to use corresponding consumable items.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Check for potion usage keys (P1, P2, P3)
        if (inputListener.isPressingP1())
        {
            tryUsePotion(1);
        }
        if (inputListener.isPressingP2())
        {
            tryUsePotion(2);
        }
        if (inputListener.isPressingP3())
        {
            tryUsePotion(3);
        }
    }
    #endregion

    #region Potion Usage
    /// <summary>
    /// Attempts to use a potion from the specified hotbar slot index.
    /// Updates both the main hotbar and inventory hotbar UI after usage.
    /// </summary>
    /// <param name="index">The hotbar slot index (1-3) to use potion from.</param>
    // COMPLEXITY ANALYSIS: tryUsePotion() - O(1)
    private void tryUsePotion(int index)
    {
        // Get consumable item from hotbar
        ConsumableItem item = inventoryManager.UseConsumableFromHotBar(index);

        if (item != null)
        {
            // Use the potion and update UI
            potionHandler.UsePotion(item);
            updateHotBarUI(index);
            hotBarHandlerInInventory.updateHotBarInInventoryUI(index);
        }
        else
        {
            // No item in hotbar slot - could add notification here if needed
        }
    }
    #endregion

    #region UI Update Methods
    /// <summary>
    /// Updates the entire hotbar UI to reflect the current hotbar state.
    /// Iterates through all slots and updates them with current item data.
    /// </summary>
    // COMPLEXITY ANALYSIS: updateHotBar() - O(s) where s = number of slots
    public void updateHotBar()
    {
        // Update each slot in the hotbar
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
    /// Updates a specific hotbar slot UI based on the provided index.
    /// Finds the corresponding slot and updates it with current item data.
    /// </summary>
    /// <param name="index">The hotbar slot index to update.</param>
    // COMPLEXITY ANALYSIS: updateHotBarUI() - O(s) where s = number of slots
    public void updateHotBarUI(int index)
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
