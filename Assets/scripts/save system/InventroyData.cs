using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable data structure for storing complete inventory system state.
/// Captures main inventory grid, hotbar, and armor slots with item properties.
/// Used by the save system to persist player inventory across game sessions.
/// </summary>
[System.Serializable]
public class InventroyData
{
    #region Main Inventory Data
    /// <summary>
    /// List of row indices for items in the main inventory grid.
    /// Used to reconstruct item positions when loading the game.
    /// </summary>
    [SerializeField]
    private List<int> row;

    /// <summary>
    /// List of column indices for items in the main inventory grid.
    /// Used to reconstruct item positions when loading the game.
    /// </summary>
    [SerializeField]
    private List<int> column;

    /// <summary>
    /// List of item counts for items in the main inventory grid.
    /// Represents stack sizes for stackable items.
    /// </summary>
    [SerializeField]
    private List<int> count;

    /// <summary>
    /// List of damage values for weapon items in the main inventory.
    /// Null for non-weapon items to preserve item type information.
    /// </summary>
    [SerializeField]
    private List<float?> damage;

    /// <summary>
    /// List of defense values for armor items in the main inventory.
    /// Null for non-armor items to preserve item type information.
    /// </summary>
    [SerializeField]
    private List<float?> defence;

    /// <summary>
    /// List of strength values for armor items in the main inventory.
    /// Null for non-armor items to preserve item type information.
    /// </summary>
    [SerializeField]
    private List<float?> strength;

    /// <summary>
    /// List of item IDs for items in the main inventory grid.
    /// Used to identify and restore specific items when loading.
    /// </summary>
    [SerializeField]
    private List<int> id;
    #endregion

    #region Hotbar Data
    /// <summary>
    /// List of item IDs for items in the hotbar slots.
    /// Represents quick-access items for immediate use.
    /// </summary>
    [SerializeField]
    private List<int> idItemInHotbar;

    /// <summary>
    /// List of item counts for items in the hotbar slots.
    /// Represents stack sizes for stackable hotbar items.
    /// </summary>
    [SerializeField]
    private List<int> countItemInHotbar;

    /// <summary>
    /// Damage value of the weapon currently in the hotbar.
    /// Null if no weapon is equipped in the hotbar.
    /// </summary>
    [SerializeField]
    private float? weaponDamage;
    #endregion

    #region Armor Slots Data
    /// <summary>
    /// List of item IDs for items equipped in armor slots.
    /// Represents helmet, chestplate, leggings, and boots.
    /// </summary>
    [SerializeField]
    private List<int> idItemInArmorSlots;

    /// <summary>
    /// List of defense values for equipped armor items.
    /// Null for empty armor slots.
    /// </summary>
    [SerializeField]
    private List<float?> armorSlotsDefence;

    /// <summary>
    /// List of strength values for equipped armor items.
    /// Null for empty armor slots.
    /// </summary>
    [SerializeField]
    private List<float?> armorSlotsStrength;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new InventroyData instance by extracting data from an Inventory.
    /// Captures complete inventory state including grid, hotbar, and armor slots.
    /// </summary>
    /// <param name="inventory">The Inventory component to extract data from.</param>
    public InventroyData(Inventory inventory)
    {
        // Initialize main inventory data lists
        row = new List<int>();
        column = new List<int>();
        count = new List<int>();
        id = new List<int>();
        damage = new List<float?>();
        defence = new List<float?>();
        strength = new List<float?>();

        // Initialize hotbar data lists
        idItemInHotbar = new List<int>();
        countItemInHotbar = new List<int>();

        // Initialize armor slots data lists
        idItemInArmorSlots = new List<int>();
        armorSlotsDefence = new List<float?>();
        armorSlotsStrength = new List<float?>();

        // Extract and set main inventory data
        List<Item>[,] items = inventory.GetItems();
        setInventory(items);

        // Extract and set hotbar data
        List<List<Item>> itemsInHotbar = inventory.getHotbar().getItems();
        setHotbar(itemsInHotbar);

        // Extract and set armor slots data
        List<Item> armorSlots = inventory.getArmorSlots().getArmorSlots();
        setArmorSlots(armorSlots);
    }
    #endregion

    #region Data Setting Methods
    /// <summary>
    /// Sets main inventory data by processing the 2D item grid.
    /// Extracts position, count, ID, and item-specific properties for each item.
    /// </summary>
    /// <param name="items">2D array of item lists representing the inventory grid.</param>
    public void setInventory(List<Item>[,] items)
    {
        // Iterate through the entire inventory grid
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                if (items[i, j] != null)
                {
                    // Add basic item information
                    row.Add(i);
                    column.Add(j);
                    count.Add(items[i, j].Count);
                    id.Add(items[i, j][0].id);

                    // Extract weapon-specific damage if applicable
                    if (items[i, j][0] is WeaponItem)
                    {
                        damage.Add(((WeaponItem)items[i, j][0]).getDamage());
                    }
                    else
                    {
                        damage.Add(null);
                    }

                    // Extract armor-specific defense and strength if applicable
                    if (items[i, j][0] is CosmeticItem)
                    {
                        defence.Add(((CosmeticItem)items[i, j][0]).getDefense());
                        strength.Add(((CosmeticItem)items[i, j][0]).getStrength());
                    }
                    else
                    {
                        defence.Add(null);
                        strength.Add(null);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets hotbar data by processing the list of hotbar item lists.
    /// Extracts item IDs, counts, and weapon damage for hotbar items.
    /// </summary>
    /// <param name="itemsInHotbar">List of item lists representing hotbar slots.</param>
    public void setHotbar(List<List<Item>> itemsInHotbar)
    {
        for (int i = 0; i < itemsInHotbar.Count; i++)
        {
            if (itemsInHotbar[i].Count > 0)
            {
                // Add item information for occupied hotbar slots
                idItemInHotbar.Add(itemsInHotbar[i][0].id);
                countItemInHotbar.Add(itemsInHotbar[i].Count);

                // Extract weapon damage if the item is a weapon
                if (itemsInHotbar[i][0] is WeaponItem)
                {
                    weaponDamage = ((WeaponItem)itemsInHotbar[i][0]).getDamage();
                }
            }
            else
            {
                // Mark empty hotbar slots with -1 ID and 0 count
                idItemInHotbar.Add(-1);
                countItemInHotbar.Add(0);
            }
        }
    }

    /// <summary>
    /// Sets armor slots data by processing the list of equipped armor items.
    /// Extracts item IDs, defense, and strength values for equipped armor.
    /// </summary>
    /// <param name="armorSlots">List of items representing equipped armor slots.</param>
    public void setArmorSlots(List<Item> armorSlots)
    {
        for (int i = 0; i < armorSlots.Count; i++)
        {
            if (armorSlots[i] != null)
            {
                // Add armor information for occupied slots
                idItemInArmorSlots.Add(armorSlots[i].id);
                armorSlotsDefence.Add(((CosmeticItem)armorSlots[i]).getDefense());
                armorSlotsStrength.Add(((CosmeticItem)armorSlots[i]).getStrength());
            }
            else
            {
                // Mark empty armor slots with -1 ID and null stats
                idItemInArmorSlots.Add(-1);
                armorSlotsDefence.Add(null);
                armorSlotsStrength.Add(null);
            }
        }
    }
    #endregion

    #region Public Properties - Inventory Data
    /// <summary>
    /// Gets the list of row indices for main inventory items.
    /// </summary>
    public List<int> Row => row;

    /// <summary>
    /// Gets the list of column indices for main inventory items.
    /// </summary>
    public List<int> Column => column;

    /// <summary>
    /// Gets the list of item counts for main inventory items.
    /// </summary>
    public List<int> Count => count;

    /// <summary>
    /// Gets the list of damage values for main inventory items.
    /// </summary>
    public List<float?> Damage => damage;

    /// <summary>
    /// Gets the list of defense values for main inventory items.
    /// </summary>
    public List<float?> Defence => defence;

    /// <summary>
    /// Gets the list of strength values for main inventory items.
    /// </summary>
    public List<float?> Strength => strength;

    /// <summary>
    /// Gets the list of item IDs for main inventory items.
    /// </summary>
    public List<int> Id => id;
    #endregion

    #region Public Properties - Hotbar Data
    /// <summary>
    /// Gets the list of item IDs for hotbar items.
    /// </summary>
    public List<int> IdItemInHotbar => idItemInHotbar;

    /// <summary>
    /// Gets the list of item counts for hotbar items.
    /// </summary>
    public List<int> CountItemInHotbar => countItemInHotbar;

    /// <summary>
    /// Gets the damage value of the weapon in the hotbar.
    /// </summary>
    public float? WeaponDamage => weaponDamage;
    #endregion

    #region Public Properties - Armor Slots Data
    /// <summary>
    /// Gets the list of item IDs for equipped armor items.
    /// </summary>
    public List<int> IdItemInArmorSlots => idItemInArmorSlots;

    /// <summary>
    /// Gets the list of defense values for equipped armor items.
    /// </summary>
    public List<float?> ArmorSlotsDefence => armorSlotsDefence;

    /// <summary>
    /// Gets the list of strength values for equipped armor items.
    /// </summary>
    public List<float?> ArmorSlotsStrength => armorSlotsStrength;
    #endregion
}
