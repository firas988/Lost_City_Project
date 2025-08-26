using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the inventory system, including a grid-based inventory and a hotbar.
/// </summary>
public class Inventory
{
    #region Static Configuration
    /// <summary>
    /// Total number of item slots in the inventory.
    /// </summary>
    private static readonly int capacity = 20;

    /// <summary>
    /// Number of columns in the inventory grid.
    /// </summary>
    private static readonly int columnInventory = 5;

    /// <summary>
    /// Number of rows in the inventory grid.
    /// </summary>
    private static readonly int rowInventory = 4;

    /// <summary>
    /// Number of slots available in the hotbar.
    /// </summary>
    private static readonly int hotbarSize = 4;

    /// <summary>
    /// Number of slots available in the armor slots.
    /// </summary>
    private static readonly int armorSlotSize = 4;
    #endregion

    #region Instance Variables
    /// <summary>
    /// 2D grid of item stacks in the inventory.
    /// </summary>
    private List<Item>[,] items;

    /// <summary>
    /// List of items stored in the hotbar.
    /// </summary>
    private HotBar hotbar;

    /// <summary>
    /// List of items stored in the armor slots.
    /// </summary>
    private ArmorSlots armorSlots;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor that initializes the inventory and hotbar.
    /// </summary>
    public Inventory()
    {
        // Initialize the 2D grid for items
        items = new List<Item>[rowInventory, columnInventory];

        // Create new hotbar and armor slots instances
        hotbar = new HotBar(hotbarSize);
        armorSlots = new ArmorSlots(armorSlotSize);
    }
    #endregion

    #region Inventory Information
    /// <summary>
    /// Returns the capacity of the inventory.
    /// </summary>
    /// <returns>The capacity of the inventory.</returns>
    public int getCapacity()
    {
        return capacity;
    }
    #endregion

    #region Item Stacking
    /// <summary>
    /// Attempts to add the item to an existing stack if it matches and there's space.
    /// </summary>
    /// <param name="item">The item to stack.</param>
    /// <param name="row">Output row index where item was stacked.</param>
    /// <param name="column">Output column index where item was stacked.</param>
    /// <returns>True if item was stacked, false otherwise.</returns>
    public bool TryStackItem(Item item, out int row, out int column)
    {
        // Search through the entire inventory grid
        for (int i = 0; i < rowInventory; i++)
        {
            for (int j = 0; j < columnInventory; j++)
            {
                // Check if slot has items
                if (items[i, j] != null)
                {
                    // Check if item type matches
                    if (items[i, j][0].id == item.id)
                    {
                        // Check if there's space in the stack
                        if (items[i, j].Count < item.maxStack)
                        {
                            // Add item to existing stack
                            items[i, j].Add(item);
                            row = i;
                            column = j;
                            return true;
                        }
                    }
                }
            }
        }

        // No suitable stack found
        row = -1;
        column = -1;
        return false;
    }
    #endregion

    #region Item Addition
    /// <summary>
    /// Tries to add an item to the inventory, either by stacking or in a new slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Output row index of added item.</param>
    /// <param name="column">Output column index of added item.</param>
    /// <returns>True if the item was added successfully, false if inventory is full.</returns>
    public bool TryAddItem(Item item, out int row, out int column)
    {
        // First try to stack with existing items
        if (TryStackItem(item, out row, out column))
        {
            return true;
        }

        // If stacking failed, find an empty slot
        for (int i = 0; i < rowInventory; i++)
        {
            for (int j = 0; j < columnInventory; j++)
            {
                if (items[i, j] == null)
                {
                    // Create new item list and add item
                    items[i, j] = new List<Item>();
                    items[i, j].Add(item);
                    row = i;
                    column = j;
                    return true;
                }
            }
        }

        // Inventory is full
        row = -1;
        column = -1;
        return false;
    }

    /// <summary>
    /// Adds an item to an empty slot with a specified count.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="row">Row index of target slot.</param>
    /// <param name="column">Column index of target slot.</param>
    /// <param name="count">Number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToEmptySlot(Item item, int row, int column, int count)
    {
        // Check if target slot is empty
        if (items[row, column] == null)
        {
            // Create new item list and add specified count of items
            items[row, column] = new List<Item>();
            for (int i = 0; i < count; i++)
            {
                items[row, column].Add(item);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds a count of items to a non-empty slot if it has the same item type and enough space.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="row">Row index of target slot.</param>
    /// <param name="column">Column index of target slot.</param>
    /// <param name="count">Number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToNotEmptySlot(Item item, int row, int column, int count)
    {
        // Check if target slot has items
        if (items[row, column] != null)
        {
            // Check if item types match
            if (items[row, column][0].id == item.id)
            {
                // Check if adding items won't exceed max stack size
                if (items[row, column].Count + count <= items[row, column][0].maxStack)
                {
                    // Add specified count of items to existing stack
                    for (int i = 0; i < count; i++)
                    {
                        items[row, column].Add(item);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region Item Management
    /// <summary>
    /// Returns the item list at a specific slot.
    /// </summary>
    /// <param name="rowIndex">Row index.</param>
    /// <param name="colIndex">Column index.</param>
    /// <returns>List of items in the slot, or null if empty.</returns>
    public List<Item> GetItem(int rowIndex, int colIndex)
    {
        return items[rowIndex, colIndex];
    }

    /// <summary>
    /// Removes all items from the specified inventory slot.
    /// </summary>
    /// <param name="rowIndex">Row index.</param>
    /// <param name="colIndex">Column index.</param>
    public void RemoveItem(int rowIndex, int colIndex)
    {
        items[rowIndex, colIndex] = null;
    }
    #endregion

    #region Accessor Methods
    /// <summary>
    /// Gets the armor slots component.
    /// </summary>
    /// <returns>Reference to the armor slots.</returns>
    public ArmorSlots getArmorSlots()
    {
        return armorSlots;
    }

    /// <summary>
    /// Gets the hotbar component.
    /// </summary>
    /// <returns>Reference to the hotbar.</returns>
    public HotBar getHotbar()
    {
        return hotbar;
    }

    /// <summary>
    /// Gets the complete inventory grid.
    /// </summary>
    /// <returns>2D array of item lists representing the inventory.</returns>
    public List<Item>[,] GetItems()
    {
        return items;
    }
    #endregion

    #region Data Loading
    /// <summary>
    /// Loads the complete inventory from saved data.
    /// </summary>
    /// <param name="inventroyData">Saved inventory data.</param>
    /// <param name="allItems">Database of all available items.</param>
    /// <returns>True if all components loaded successfully.</returns>
    public bool LoadInventory(InventroyData inventroyData, ItemDatabase allItems)
    {
        // Load each component separately
        bool inventoryLoaded = LoadJustInventory(inventroyData, allItems);
        bool hotbarLoaded = LoadHotbar(inventroyData, allItems);
        bool armorSlotsLoaded = LoadArmorSlots(inventroyData, allItems);

        // Return true only if all components loaded successfully
        return inventoryLoaded && hotbarLoaded && armorSlotsLoaded;
    }

    /// <summary>
    /// Loads the main inventory grid from saved data.
    /// </summary>
    /// <param name="inventroyData">Saved inventory data.</param>
    /// <param name="allItems">Database of all available items.</param>
    /// <returns>True if inventory loaded successfully.</returns>
    public bool LoadJustInventory(InventroyData inventroyData, ItemDatabase allItems)
    {
        // Iterate through all saved inventory entries
        for (int i = 0; i < inventroyData.Row.Count; i++)
        {
            // Create new item list at the specified position
            this.items[inventroyData.Row[i], inventroyData.Column[i]] = new List<Item>();

            // Instantiate the item from the database
            Item item = ScriptableObject.Instantiate(allItems.GetItem(inventroyData.Id[i]));

            // Set specific properties based on item type
            if (item is WeaponItem)
            {
                ((WeaponItem)item).setDamage(inventroyData.Damage[i].Value);
            }
            else if (item is CosmeticItem)
            {
                ((CosmeticItem)item).setDefense(inventroyData.Defence[i].Value);
                ((CosmeticItem)item).setStrength(inventroyData.Strength[i].Value);
            }

            // Add the specified count of items to the slot
            for (int j = 0; j < inventroyData.Count[i]; j++)
            {
                this.items[inventroyData.Row[i], inventroyData.Column[i]].Add(item);
            }
        }
        return true;
    }

    /// <summary>
    /// Loads the hotbar from saved data.
    /// </summary>
    /// <param name="inventroyData">Saved inventory data.</param>
    /// <param name="allItems">Database of all available items.</param>
    /// <returns>True if hotbar loaded successfully.</returns>
    public bool LoadHotbar(InventroyData inventroyData, ItemDatabase allItems)
    {
        // Check if hotbar data exists
        if (inventroyData.IdItemInHotbar == null)
        {
            return true;
        }

        // Load each hotbar slot
        for (int i = 0; i < inventroyData.IdItemInHotbar.Count; i++)
        {
            if (i == 0 && inventroyData.IdItemInHotbar[i] != -1)
            {
                // Slot 0 is weapon slot
                WeaponItem weapon = ScriptableObject.Instantiate(
                    allItems.GetItem(inventroyData.IdItemInHotbar[i]) as WeaponItem
                );
                weapon.setDamage(inventroyData.WeaponDamage.Value);
                this.hotbar.setWeapon(weapon);
            }
            else if (inventroyData.IdItemInHotbar[i] != -1)
            {
                // Other slots are consumable slots
                ConsumableItem consumable = ScriptableObject.Instantiate(
                    allItems.GetItem(inventroyData.IdItemInHotbar[i]) as ConsumableItem
                );
                this.hotbar.setConsumable(consumable, inventroyData.CountItemInHotbar[i], i);
            }
        }
        return true;
    }

    /// <summary>
    /// Loads the armor slots from saved data.
    /// </summary>
    /// <param name="inventroyData">Saved inventory data.</param>
    /// <param name="allItems">Database of all available items.</param>
    /// <returns>True if armor slots loaded successfully.</returns>
    public bool LoadArmorSlots(InventroyData inventroyData, ItemDatabase allItems)
    {
        // Check if armor slot data exists
        if (inventroyData.IdItemInArmorSlots == null)
        {
            return true;
        }

        // Load each armor slot
        for (int i = 0; i < inventroyData.IdItemInArmorSlots.Count; i++)
        {
            if (inventroyData.IdItemInArmorSlots[i] != -1)
            {
                // Instantiate cosmetic item and set properties
                CosmeticItem cosmetic = ScriptableObject.Instantiate(
                    allItems.GetItem(inventroyData.IdItemInArmorSlots[i]) as CosmeticItem
                );
                cosmetic.setDefense(inventroyData.ArmorSlotsDefence[i].Value);
                cosmetic.setStrength(inventroyData.ArmorSlotsStrength[i].Value);

                // Place item in appropriate armor slot based on type
                switch (cosmetic.getCosmeticType())
                {
                    case CosmeticType.Helmet:
                        this.armorSlots.setHelmet(cosmetic);
                        break;
                    case CosmeticType.Chestplate:
                        this.armorSlots.setChestplate(cosmetic);
                        break;
                    case CosmeticType.Leggings:
                        this.armorSlots.setLeggings(cosmetic);
                        break;
                    case CosmeticType.Boots:
                        this.armorSlots.setBoots(cosmetic);
                        break;
                }
            }
        }
        return true;
    }
    #endregion
}
