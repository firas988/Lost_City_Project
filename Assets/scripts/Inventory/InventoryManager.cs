using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages inventory-related actions including item addition, removal, and interaction with the UI slots.
/// Coordinates between the inventory data structure, UI managers, and player systems for comprehensive
/// inventory management including hotbar, armor slots, and item operations.
/// </summary>
[RequireComponent(typeof(SlotManager))]
[RequireComponent(typeof(ArmorSlotManager))]
public class InventoryManager : MonoBehaviour
{
    #region Core Components
    /// <summary>The inventory data structure containing all items and their organization.</summary>
    private Inventory inventory;

    /// <summary>Responsible for displaying and updating the slot UI elements.</summary>
    private SlotManager slotManager;

    /// <summary>Responsible for initializing and managing the player instance.</summary>
    private StartPlayer startPlayer;

    /// <summary>Reference to the current player for inventory and stat operations.</summary>
    private Player player;

    /// <summary>Responsible for managing and displaying armor slot UI elements.</summary>
    private ArmorSlotManager armorSlotManager;
    #endregion

    #region Item Database
    /// <summary>Reference to the complete database of all available items in the game.</summary>
    [SerializeField]
    private ItemDatabase AllItems;
    #endregion

    #region UI Management
    /// <summary>Reference to the NotificationsManager for displaying inventory-related messages.</summary>
    private NotificationsManager notificationsManager;

    /// <summary>Reference to the HotBarHandler for managing the main hotbar UI.</summary>
    private HotBarHandler hotBarHandler;

    /// <summary>Reference to the HotBarHandlerInInventory for managing hotbar display within inventory.</summary>
    private HotBarHandlerInInventory hotBarHandlerInInventory;
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Returns the current Inventory instance for external access.
    /// </summary>
    /// <returns>The inventory object containing all items and organization.</returns>
    public Inventory getInventory()
    {
        return inventory;
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes all component references and sets up the initial inventory state.
    /// Sets the first hotbar item as the player's weapon and establishes connections
    /// between inventory systems.
    /// </summary>
    void Start()
    {
        // Get required components
        slotManager = GetComponent<SlotManager>();
        armorSlotManager = GetComponent<ArmorSlotManager>();

        // Find and connect to player systems
        startPlayer = FindAnyObjectByType<StartPlayer>();
        player = startPlayer.getPlayer();
        inventory = player.getInventory();

        // Get UI management components
        notificationsManager = FindAnyObjectByType<NotificationsManager>();
        hotBarHandler = GetComponent<HotBarHandler>();
        hotBarHandlerInInventory = GetComponent<HotBarHandlerInInventory>();
    }

    /// <summary>
    /// Checks for input keys to simulate item addition, listing, and mouse unlocking.
    /// Currently handles the 'L' key for adding random items to inventory.
    /// </summary>
    void Update()
    {
        // Debug key for testing - add random item to inventory
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddItemToInventory(AllItems.GetRandomItem());
        }
    }
    #endregion

    #region Item Removal Operations
    /// <summary>
    /// Removes one item from a specific inventory slot and updates the slot UI.
    /// Handles both single items and stacked items appropriately.
    /// </summary>
    /// <param name="row">Row index of the item slot.</param>
    /// <param name="column">Column index of the item slot.</param>
    /// <returns>The item that was taken, or null if none.</returns>
    public Item TakeOnItem(int row, int column)
    {
        // Get items from specified slot
        List<Item> items = inventory.GetItem(row, column);
        if (items != null)
        {
            // Take the first item from the stack
            Item item = items[0];

            // Handle stack removal
            if (items.Count == 1)
            {
                // Remove entire stack if only one item remains
                RemoveItemAndUpdateUI(row, column);
            }
            else
            {
                // Remove one item and update stack count
                items.RemoveAt(0);
                slotManager.SetSlot(item, items.Count, row, column);
            }
            return item;
        }
        return null;
    }

    /// <summary>
    /// Removes one item from a specific hotbar slot and updates the UI accordingly.
    /// Handles both weapon and consumable items with appropriate logic.
    /// </summary>
    /// <param name="index">Index of the hotbar slot (0 for weapon, 1-3 for consumables).</param>
    /// <param name="slot">The UI slot component to update.</param>
    /// <returns>The item that was taken, or null if none.</returns>
    public Item TakeOnItemFromHotBar(int index, Slot slot)
    {
        List<Item> items = null;

        // Get items based on slot type
        if (index == 0)
        {
            // Weapon slot
            items = inventory.getHotbar().getWeapon();
            player.calculateStrengthAndDefenseBonus();
        }
        else
        {
            // Consumable slot
            items = inventory.getHotbar().getConsumable(index);
        }

        // Handle item removal
        if (items != null && items.Count > 0)
        {
            Item item = items[0];

            // Handle stack removal
            if (items.Count == 1)
            {
                // Remove entire stack and clear UI
                RemoveItemFromHotBar(index);
                slot.ClearSlot();
            }
            else
            {
                // Remove one item and update stack count
                items.RemoveAt(0);
                slot.SetItem(item, items.Count);
            }
            return item;
        }
        return null;
    }

    /// <summary>
    /// Completely removes all items from a given slot and clears its UI.
    /// </summary>
    /// <param name="row">Row index of the slot.</param>
    /// <param name="column">Column index of the slot.</param>
    public void RemoveItemAndUpdateUI(int row, int column)
    {
        // Remove from inventory data and clear UI
        inventory.RemoveItem(row, column);
        slotManager.ClearSlot(row, column);
    }

    /// <summary>
    /// Removes an item from a specific hotbar slot and updates player stats accordingly.
    /// </summary>
    /// <param name="index">Index of the hotbar slot to clear.</param>
    public void RemoveItemFromHotBar(int index)
    {
        if (index == 0)
        {
            // Remove weapon and recalculate player stats
            inventory.getHotbar().removeWeapon();
            player.removeWeapon();
            player.calculateStrengthAndDefenseBonus();
        }
        else
        {
            // Remove consumable item
            inventory.getHotbar().removeConsumable(index);
        }

        // Update hotbar UI
        hotBarHandler.updateHotBar();
    }
    #endregion

    #region Item Addition Operations
    /// <summary>
    /// Adds items to an empty inventory slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Row index of the target slot.</param>
    /// <param name="column">Column index of the target slot.</param>
    /// <param name="count">The number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToEmptySlot(Item item, int row, int column, int count)
    {
        return inventory.AddItemToEmptySlot(item, row, column, count);
    }

    /// <summary>
    /// Adds items to a slot that already contains the same item type.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Row index of the target slot.</param>
    /// <param name="column">Column index of the target slot.</param>
    /// <param name="count">The number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToNotEmptySlot(Item item, int row, int column, int count)
    {
        return inventory.AddItemToNotEmptySlot(item, row, column, count);
    }

    /// <summary>
    /// Adds a new item to the inventory with proper instantiation and stat calculation.
    /// Handles different item types (weapons, cosmetics) with appropriate stat assignment.
    /// </summary>
    /// <param name="item">The item template to add to inventory.</param>
    public void AddItemToInventory(Item item)
    {
        // Create a new instance of the item
        Item newItem = ScriptableObject.Instantiate(item);

        // Handle weapon items - assign damage based on rarity
        if (newItem is WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)newItem;
            weaponItem.setDamage(GiveWeaponDamage.getDamage(weaponItem.getRarity()));
        }

        // Handle cosmetic items - assign stats based on rarity
        if (newItem is CosmeticItem)
        {
            CosmeticItem cosmeticItem = (CosmeticItem)newItem;
            cosmeticItem.setStrength(
                GiveCosmeticStrengthDefense.getStrength(cosmeticItem.getRarity())
            );
            cosmeticItem.setDefense(
                GiveCosmeticStrengthDefense.getDefense(cosmeticItem.getRarity())
            );
        }

        // Attempt to add item to inventory
        if (inventory.TryAddItem(newItem, out int row, out int column))
        {
            // Successfully added - update UI and show notification
            List<Item> items = inventory.GetItem(row, column);
            slotManager.SetSlot(newItem, items.Count, row, column);
            notificationsManager.ShowBottomLeftNotificationInventory(
                $"You have added {newItem.getItemName()} to your inventory."
            );
        }
        else
        {
            // Inventory is full - show notification
            notificationsManager.ShowBottomLeftNotificationInventory("Inventory is full");
        }
    }
    #endregion

    #region Armor Slot Management
    /// <summary>
    /// Attempts to place a cosmetic item into the appropriate armor slot.
    /// Updates both the inventory data and UI, and recalculates player stats.
    /// </summary>
    /// <param name="item">The cosmetic item to equip.</param>
    /// <param name="cosmeticType">The type of armor slot to equip to.</param>
    /// <returns>True if successfully equipped, false otherwise.</returns>
    public bool TryPutItemToArmorSlot(Item item, CosmeticType cosmeticType)
    {
        if (item is CosmeticItem)
        {
            // Equip item based on cosmetic type and update systems
            switch (cosmeticType)
            {
                case CosmeticType.Helmet:
                    inventory.getArmorSlots().setHelmet(item);
                    armorSlotManager.setHelmet(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Chestplate:
                    inventory.getArmorSlots().setChestplate(item);
                    armorSlotManager.setChestplate(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Leggings:
                    inventory.getArmorSlots().setLeggings(item);
                    armorSlotManager.setLeggings(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Boots:
                    inventory.getArmorSlots().setBoots(item);
                    armorSlotManager.setBoots(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes an item from a specific armor slot and updates all related systems.
    /// Recalculates player stats after removal.
    /// </summary>
    /// <param name="cosmeticType">The type of armor slot to remove from.</param>
    public void TryRemoveItemFromArmorSlot(CosmeticType cosmeticType)
    {
        // Remove item based on cosmetic type and update systems
        switch (cosmeticType)
        {
            case CosmeticType.Helmet:
                armorSlotManager.removeHelmet();
                inventory.getArmorSlots().removeHelmet();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Chestplate:
                armorSlotManager.removeChestplate();
                inventory.getArmorSlots().removeChestplate();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Leggings:
                armorSlotManager.removeLeggings();
                inventory.getArmorSlots().removeLeggings();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Boots:
                armorSlotManager.removeBoots();
                inventory.getArmorSlots().removeBoots();
                player.calculateStrengthAndDefenseBonus();
                break;
        }
    }
    #endregion

    #region Hotbar Management
    /// <summary>
    /// Attempts to move an item to an empty hotbar slot.
    /// Handles weapon placement in slot 0 and consumables in slots 1-3.
    /// </summary>
    /// <param name="item">The item to place in the hotbar.</param>
    /// <param name="count">The number of items to place.</param>
    /// <param name="index">The hotbar slot index (0 for weapon, 1-3 for consumables).</param>
    /// <returns>True if successfully placed, false otherwise.</returns>
    public bool TryMoveItemToEmptyHotBar(Item item, int count, int index)
    {
        if (index == 0 && item is WeaponItem)
        {
            // Place weapon in slot 0 and update player stats
            inventory.getHotbar().setWeapon(item);
            player.setWeapon();
            player.calculateStrengthAndDefenseBonus();
            hotBarHandler.updateHotBar();
            return true;
        }
        else if (index > 0 && index < 4 && item is ConsumableItem)
        {
            // Place consumable in slots 1-3
            inventory.getHotbar().setConsumable(item, count, index);
            hotBarHandler.updateHotBar();
            return true;
        }

        // Update hotbar UI and return false if placement failed
        hotBarHandler.updateHotBar();
        return false;
    }

    /// <summary>
    /// Adds items to a hotbar slot that already contains items of the same type.
    /// Only works for consumable items in slots 1-3.
    /// </summary>
    /// <param name="item">The item to add to the existing stack.</param>
    /// <param name="count">The number of items to add.</param>
    /// <param name="index">The hotbar slot index (must be 1-3 for consumables).</param>
    /// <returns>True if successfully added, false otherwise.</returns>
    public bool AddItemToNotEmptyHotBar(Item item, int count, int index)
    {
        if (index == 0)
        {
            return false; // Cannot add to weapon slot
        }

        // Update hotbar UI and add to existing consumable stack
        hotBarHandler.updateHotBar();
        return inventory.getHotbar().addToConsumable(item, count, index);
    }

    /// <summary>
    /// Retrieves items from a specific hotbar slot.
    /// Handles weapon retrieval from slot 0 and consumables from slots 1-3.
    /// </summary>
    /// <param name="index">The hotbar slot index to retrieve from.</param>
    /// <returns>List of items in the specified slot.</returns>
    public List<Item> GetItemFromHotBar(int index)
    {
        if (index == 0)
        {
            // Get weapon and update player stats
            player.setWeapon();
            player.calculateStrengthAndDefenseBonus();
            return inventory.getHotbar().getWeapon();
        }
        else
        {
            // Get consumable items
            return inventory.getHotbar().getConsumable(index);
        }
    }

    /// <summary>
    /// Uses a consumable item from the hotbar, consuming one item from the stack.
    /// </summary>
    /// <param name="index">The hotbar slot index of the consumable to use.</param>
    /// <returns>The consumable item that was used, or null if none available.</returns>
    public ConsumableItem UseConsumableFromHotBar(int index)
    {
        return inventory.getHotbar().useConsumable(index);
    }
    #endregion

    #region Data Retrieval
    /// <summary>
    /// Retrieves items from a specific inventory slot.
    /// </summary>
    /// <param name="row">Row index of the slot.</param>
    /// <param name="column">Column index of the slot.</param>
    /// <returns>List of items in the specified slot.</returns>
    public List<Item> GetItemFromInventory(int row, int column)
    {
        return inventory.GetItem(row, column);
    }
    #endregion

    #region Save/Load System
    /// <summary>
    /// Loads the inventory from saved data, updating all UI elements accordingly.
    /// </summary>
    /// <param name="inventroyData">The saved inventory data to load from.</param>
    public void LoadInventory(InventroyData inventroyData)
    {
        if (inventroyData != null)
        {
            StartCoroutine(WaitForInventoryAndLoad(inventroyData));
        }
    }

    /// <summary>
    /// Coroutine that waits for inventory initialization before loading data.
    /// Updates all UI elements and recalculates player stats after loading.
    /// </summary>
    /// <param name="inventroyData">The saved inventory data to load from.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator WaitForInventoryAndLoad(InventroyData inventroyData)
    {
        // Wait until inventory is properly initialized
        yield return new WaitUntil(() => inventory != null);

        // Load inventory data and update all systems
        if (inventory.LoadInventory(inventroyData, AllItems))
        {
            updateInventoryUI();
            hotBarHandler.updateHotBar();
            hotBarHandlerInInventory.updateHotBarInInventory();
            updateArmorSlotsUI();
            player.calculateStrengthAndDefenseBonus();
        }
    }
    #endregion

    #region UI Update Methods
    /// <summary>
    /// Updates the inventory UI to reflect the current inventory state.
    /// Iterates through all inventory slots and updates their display.
    /// </summary>
    public void updateInventoryUI()
    {
        // Get all items from inventory
        List<Item>[,] items = inventory.GetItems();

        // Update each slot's UI
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                if (items[i, j] != null)
                {
                    slotManager.SetSlot(items[i, j][0], items[i, j].Count, i, j);
                }
            }
        }
    }

    /// <summary>
    /// Updates the armor slots UI to reflect the current equipped armor.
    /// Synchronizes the UI with the inventory's armor slot data.
    /// </summary>
    public void updateArmorSlotsUI()
    {
        // Update each armor slot UI with current inventory data
        armorSlotManager.setHelmet(inventory.getArmorSlots().getHelmet());
        armorSlotManager.setChestplate(inventory.getArmorSlots().getChestplate());
        armorSlotManager.setLeggings(inventory.getArmorSlots().getLeggings());
        armorSlotManager.setBoots(inventory.getArmorSlots().getBoots());
    }
    #endregion
}
