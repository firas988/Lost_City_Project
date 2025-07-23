using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the inventory system, including a grid-based inventory and a hotbar.
/// </summary>
public class Inventory
{
    /// ===== STATIC VARIABLES =====
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

    /// ===== INSTANCE VARIABLES =====
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

    /// <summary>
    /// Constructor that initializes the inventory and hotbar.
    /// </summary>
    public Inventory()
    {
        items = new List<Item>[rowInventory, columnInventory];
        hotbar = new HotBar(hotbarSize);
        armorSlots = new ArmorSlots(armorSlotSize);
    }

    /// <summary>
    /// Returns the capacity of the inventory.
    /// </summary>
    /// <returns>The capacity of the inventory.</returns>
    public int getCapacity()
    {
        return capacity;
    }

    /// <summary>
    /// Attempts to add the item to an existing stack if it matches and there's space.
    /// </summary>
    /// <param name="item">The item to stack.</param>
    /// <param name="row">Output row index where item was stacked.</param>
    /// <param name="column">Output column index where item was stacked.</param>
    /// <returns>True if item was stacked, false otherwise.</returns>
    public bool TryStackItem(Item item, out int row, out int column)
    {
        for (int i = 0; i < rowInventory; i++)
        {
            for (int j = 0; j < columnInventory; j++)
            {
                if (items[i, j] != null)
                {
                    if (items[i, j][0].id == item.id)
                    {
                        if (items[i, j].Count < item.maxStack)
                        {
                            items[i, j].Add(item);
                            row = i;
                            column = j;
                            return true;
                        }
                    }
                }
            }
        }
        row = -1;
        column = -1;
        return false;
    }

    /// <summary>
    /// Tries to add an item to the inventory, either by stacking or in a new slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Output row index of added item.</param>
    /// <param name="column">Output column index of added item.</param>
    /// <returns>True if the item was added successfully, false if inventory is full.</returns>
    public bool TryAddItem(Item item, out int row, out int column)
    {
        if (TryStackItem(item, out row, out column))
        {
            return true;
        }

        for (int i = 0; i < rowInventory; i++)
        {
            for (int j = 0; j < columnInventory; j++)
            {
                if (items[i, j] == null)
                {
                    items[i, j] = new List<Item>();
                    items[i, j].Add(item);
                    row = i;
                    column = j;
                    return true;
                }
            }
        }

        Debug.Log("Inventory full!");
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
        if (items[row, column] == null)
        {
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
        if (items[row, column] != null)
        {
            if (items[row, column][0].id == item.id)
            {
                if (items[row, column].Count + count <= items[row, column][0].maxStack)
                {
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

    public ArmorSlots getArmorSlots()
    {
        return armorSlots;
    }

    public HotBar getHotbar()
    {
        return hotbar;
    }
    public List<Item>[,] GetItems()
    {
        return items;
    }

    public void SaveInventory()
    {
        SaveSystem.SaveInventory(this);
    }

    public bool LoadInventory(InventroyData inventroyData, ItemDatabase allItems)
    {
        for (int i = 0; i < inventroyData.row.Count; i++)
        {
            this.items[inventroyData.row[i], inventroyData.column[i]] = new List<Item>();

            Item item = ScriptableObject.Instantiate(allItems.GetItem(inventroyData.id[i]));
            if (item is WeaponItem)
            {
                ((WeaponItem)item).setDamage(inventroyData.damage[i].Value);
            }
            else if (item is CosmeticItem)
            {
                ((CosmeticItem)item).setDefense(inventroyData.defence[i].Value);
                ((CosmeticItem)item).setStrength(inventroyData.strength[i].Value);
            }

            for (int j = 0; j < inventroyData.count[i]; j++)
            {
                this.items[inventroyData.row[i], inventroyData.column[i]].Add(item);
            }
        }
        return true;
    }
}
