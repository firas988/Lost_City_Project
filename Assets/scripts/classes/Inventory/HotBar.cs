using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the hotbar system for quick access to weapons and consumable items
/// </summary>
public class HotBar
{
    #region Private Fields
    /// <summary>
    /// List of item lists, where each inner list represents a hotbar slot
    /// </summary>
    private List<List<Item>> items;

    /// <summary>
    /// Index of the weapon slot (typically slot 0)
    /// </summary>
    private int weaponIndex = 0;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor that initializes the hotbar with the specified number of slots
    /// </summary>
    /// <param name="size">The number of hotbar slots to create</param>
    public HotBar(int size)
    {
        // COMPLEXITY ANALYSIS: HotBar() - O(s) where s = size
        // Initialize the main items list
        items = new List<List<Item>>();

        // Create empty item lists for each slot
        for (int i = 0; i < size; i++)
        {
            items.Add(new List<Item>());
        }
    }
    #endregion

    #region Weapon Management
    /// <summary>
    /// Sets a weapon item in the weapon slot
    /// </summary>
    /// <param name="item">The weapon item to add</param>
    public void setWeapon(Item item)
    {
        // COMPLEXITY ANALYSIS: setWeapon() - O(1)
        items[weaponIndex].Add(item);
    }

    /// <summary>
    /// Gets all items in the weapon slot
    /// </summary>
    /// <returns>List of items in the weapon slot</returns>
    public List<Item> getWeapon()
    {
        // COMPLEXITY ANALYSIS: getWeapon() - O(1)
        return items[weaponIndex];
    }

    /// <summary>
    /// Removes all items from the weapon slot
    /// </summary>
    public void removeWeapon()
    {
        // COMPLEXITY ANALYSIS: removeWeapon() - O(1)
        items[weaponIndex] = new List<Item>();
    }
    #endregion

    #region Consumable Management
    /// <summary>
    /// Sets consumable items in a specific slot with the specified count
    /// </summary>
    /// <param name="item">The consumable item to add</param>
    /// <param name="count">Number of items to add</param>
    /// <param name="index">Slot index to add items to</param>
    public void setConsumable(Item item, int count, int index)
    {
        // COMPLEXITY ANALYSIS: setConsumable() - O(count) where count = number of items to add
        // Validate index is within bounds
        if (index > 0 && index < items.Count)
        {
            // Add the specified number of items to the slot
            for (int i = 0; i < count; i++)
            {
                items[index].Add(item);
            }
        }
    }

    /// <summary>
    /// Adds consumable items to a specific slot if there's enough space
    /// </summary>
    /// <param name="item">The consumable item to add</param>
    /// <param name="count">Number of items to add</param>
    /// <param name="index">Slot index to add items to</param>
    /// <returns>True if items were added successfully, false if not enough space</returns>
    public bool addToConsumable(Item item, int count, int index)
    {
        // COMPLEXITY ANALYSIS: addToConsumable() - O(count) where count = number of items to add
        // Validate index is within bounds
        if (index > 0 && index < items.Count)
        {
            // Check if adding items would exceed max stack size
            if (items[index].Count + count > item.getMaxStack())
            {
                return false;
            }

            // Add the specified number of items to the slot
            for (int i = 0; i < count; i++)
            {
                items[index].Add(item);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all items in a specific consumable slot
    /// </summary>
    /// <param name="index">Slot index to get items from</param>
    /// <returns>List of items in the specified slot, or null if index is invalid</returns>
    public List<Item> getConsumable(int index)
    {
        // COMPLEXITY ANALYSIS: getConsumable() - O(1)
        // Validate index is within bounds
        if (index > 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    /// <summary>
    /// Removes all items from a specific consumable slot
    /// </summary>
    /// <param name="index">Slot index to clear</param>
    public void removeConsumable(int index)
    {
        // COMPLEXITY ANALYSIS: removeConsumable() - O(1)
        // Validate index is within bounds
        if (index > 0 && index < items.Count)
        {
            items[index] = new List<Item>();
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Gets the complete hotbar items structure
    /// </summary>
    /// <returns>List of all hotbar slots with their items</returns>
    public List<List<Item>> getItems()
    {
        // COMPLEXITY ANALYSIS: getItems() - O(1)
        return items;
    }
    #endregion

    #region Item Usage
    /// <summary>
    /// Uses a consumable item from a specific slot (removes one item and returns it)
    /// </summary>
    /// <param name="index">Slot index to use item from</param>
    /// <returns>The consumed item, or null if slot is empty or index is invalid</returns>
    public ConsumableItem useConsumable(int index)
    {
        // COMPLEXITY ANALYSIS: useConsumable() - O(1)
        // Validate index is within bounds
        if (index > 0 && index < items.Count)
        {
            // Check if slot has items
            if (items[index].Count > 0)
            {
                // Get the first item and remove it from the slot
                ConsumableItem item = (ConsumableItem)items[index][0];
                items[index].RemoveAt(0);
                return item;
            }
            else
            {
                // Clear empty slot
                items[index] = new List<Item>();
            }
        }
        return null;
    }
    #endregion
}
