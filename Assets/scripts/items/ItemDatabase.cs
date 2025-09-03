using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database that contains all available items in the game.
/// Provides centralized access to item data for inventory, crafting, and loot systems.
/// Offers methods for retrieving items by ID, category, or random selection.
/// Can be created and configured through the Unity menu system.
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    #region Item Collection
    /// <summary>
    /// Collection of all items available in the game.
    /// Contains items of all types, rarities, and categories.
    /// Populated through the Unity Inspector for easy configuration.
    /// </summary>
    [SerializeField]
    private List<Item> allItems;
    #endregion

    #region Public Access
    /// <summary>
    /// Public accessor for the complete item collection.
    /// Provides read-only access to all items in the database.
    /// </summary>
    public List<Item> AllItems => allItems;
    #endregion

    #region Item Retrieval Methods
    /// <summary>
    /// Retrieves a specific item by its unique ID.
    /// Useful for save/load systems and specific item references.
    /// </summary>
    /// <param name="id">The unique identifier of the item to find.</param>
    /// <returns>The item with the matching ID, or null if not found.</returns>
    public Item GetItem(int id)
    {
        // Search through all items to find one with matching ID
        return allItems.Find(item => item.getId() == id);
    }

    /// <summary>
    /// Retrieves a random item from the database.
    /// Can optionally filter by item category for more specific random selection.
    /// Useful for loot generation and random rewards.
    /// </summary>
    /// <param name="itemCategory">Optional category filter. If null, selects from all items.</param>
    /// <returns>A randomly selected item from the database or filtered category.</returns>
    public Item GetRandomItem(ItemCategory? itemCategory = null)
    {
        if (itemCategory == null)
        {
            // Return random item from entire database
            return allItems[Random.Range(0, allItems.Count)];
        }
        else
        {
            // Filter items by category and return random selection
            List<Item> items = allItems.FindAll(item => item.getCategory() == itemCategory);
            return items[Random.Range(0, items.Count)];
        }
    }
    #endregion
}
