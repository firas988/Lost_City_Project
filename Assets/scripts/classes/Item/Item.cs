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
    [SerializeField]
    private int id;

    /// <summary>
    /// Display name of the item shown in UI.
    /// Should be descriptive and user-friendly.
    /// </summary>
    [SerializeField]
    private string itemName;
    #endregion

    #region Item Properties
    /// <summary>
    /// Rarity level of the item.
    /// Determines item power, drop rates, and visual appearance.
    /// </summary>
    [SerializeField]
    private ItemRarity rarity;

    /// <summary>
    /// Category classification of the item.
    /// Determines storage location and usage behavior.
    /// </summary>
    [SerializeField]
    private ItemCategory category;
    #endregion

    #region Visual Representation
    /// <summary>
    /// Icon sprite displayed in inventory and UI.
    /// Should be clear and representative of the item.
    /// </summary>
    [SerializeField]
    private Sprite icon;

    /// <summary>
    /// 3D model/prefab used for item spawning and world representation.
    /// Used when dropping items or displaying them in the game world.
    /// </summary>
    [SerializeField]
    private GameObject itemPrefab;
    #endregion

    #region Inventory Management
    /// <summary>
    /// Maximum number of items that can be stacked in a single inventory slot.
    /// Set to 1 for unique items, higher values for stackable consumables.
    /// </summary>
    [SerializeField]
    private int maxStack;
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


    #region Getters and Setters
    /// <summary>
    /// Returns the unique identifier for the item.
    /// </summary>
    /// <returns>The unique identifier for the item.</returns>
    public int getId()
    {
        return id;
    }

    /// <summary>
    /// Returns the display name of the item.
    /// </summary>
    /// <returns>The display name of the item.</returns>
    public string getItemName()
    {
        return itemName;
    }

    /// <summary>
    /// Returns the rarity level of the item.
    /// </summary>
    /// <returns>The rarity level of the item.</returns>
    public ItemRarity getRarity()
    {
        return rarity;
    }

    /// <summary>
    /// Returns the category classification of the item.
    /// </summary>
    /// <returns>The category classification of the item.</returns>
    public ItemCategory getCategory()
    {
        return category;
    }

    /// <summary>
    /// Returns the icon sprite displayed in inventory and UI.
    /// </summary>
    /// <returns>The icon sprite displayed in inventory and UI.</returns>
    public Sprite getIcon()
    {
        return icon;
    }

    /// <summary>
    /// Returns the 3D model/prefab used for item spawning and world representation.
    /// </summary>
    /// <returns>The 3D model/prefab used for item spawning and world representation.</returns>
    public GameObject getItemPrefab()
    {
        return itemPrefab;
    }

    /// <summary>
    /// Returns the maximum number of items that can be stacked in a single inventory slot.
    /// </summary>
    /// <returns>The maximum number of items that can be stacked.</returns>
    public int getMaxStack()
    {
        return maxStack;
    }
    #endregion
}
