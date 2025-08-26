using UnityEngine;

/// <summary>
/// Defines the rarity levels for items in the game.
/// Used to categorize items by their power level and drop frequency.
/// Higher rarity items provide better stats but are less common.
/// </summary>
public enum ItemRarity
{
    Common, // Basic items with standard stats
    Rare, // Uncommon items with improved stats
    Epic, // Powerful items with significant stat bonuses
    Legendary, // Exceptional items with maximum stat bonuses
}

/// <summary>
/// Defines the main categories of items in the game.
/// Used to organize items by their function and behavior.
/// Determines how items are stored and used in the inventory system.
/// </summary>
public enum ItemCategory
{
    Weapon, // Items that can be equipped for combat
    Cosmetic, // Items that provide stat bonuses when equipped
    Consumable, // Items that can be used for temporary effects
}

/// <summary>
/// Abstract base class for all items in the game.
/// Provides common properties and methods that all items must implement.
/// Implements ScriptableObject for easy asset creation and management.
/// </summary>
[System.Serializable]
public abstract class Item : ScriptableObject
{
    #region Item Identification
    /// <summary>
    /// Unique identifier for the item.
    /// Used for database lookups and save/load systems.
    /// </summary>
    public int id;

    /// <summary>
    /// Display name of the item shown in UI.
    /// Should be descriptive and user-friendly.
    /// </summary>
    public string itemName;
    #endregion

    #region Item Properties
    /// <summary>
    /// Rarity level of the item.
    /// Determines item power, drop rates, and visual appearance.
    /// </summary>
    public ItemRarity rarity;

    /// <summary>
    /// Category classification of the item.
    /// Determines storage location and usage behavior.
    /// </summary>
    public ItemCategory category;
    #endregion

    #region Visual Representation
    /// <summary>
    /// Icon sprite displayed in inventory and UI.
    /// Should be clear and representative of the item.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// 3D model/prefab used for item spawning and world representation.
    /// Used when dropping items or displaying them in the game world.
    /// </summary>
    public GameObject itemPrefab;
    #endregion

    #region Inventory Management
    /// <summary>
    /// Maximum number of items that can be stacked in a single inventory slot.
    /// Set to 1 for unique items, higher values for stackable consumables.
    /// </summary>
    public int maxStack;
    #endregion

    #region Abstract Methods
    /// <summary>
    /// Abstract method that must be implemented by derived classes.
    /// Returns a formatted description of the item for UI display.
    /// Should include relevant stats and information specific to the item type.
    /// </summary>
    /// <returns>A formatted string describing the item's properties and effects.</returns>
    public abstract string getDescription();
    #endregion
}
