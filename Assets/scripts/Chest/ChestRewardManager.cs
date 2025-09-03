using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages chest rewards and item distribution with configurable drop rates and rarity systems
/// </summary>
public class ChestRewardManager : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Database containing all available weapons for chest rewards
    /// </summary>
    [SerializeField]
    private WeaponDatabase weaponDatabase;

    /// <summary>
    /// Database containing all available cosmetic items for chest rewards
    /// </summary>
    [SerializeField]
    private CosmeticDatabase cosmeticDatabase;

    /// <summary>
    /// Database containing all available consumable items for chest rewards
    /// </summary>
    [SerializeField]
    private ConsumableDatabase consumableDatabase;

    /// <summary>
    /// Minimum XP amount that can be gained from opening a chest
    /// </summary>
    [SerializeField]
    private int minXP = 250;

    /// <summary>
    /// Maximum XP amount that can be gained from opening a chest
    /// </summary>
    [SerializeField]
    private int maxXP = 1000;

    /// <summary>
    /// Probability of receiving a weapon when opening a chest (0.0 to 1.0)
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField]
    private float weaponDropChance = 0.7f;

    /// <summary>
    /// Probability of receiving a cosmetic item when opening a chest (0.0 to 1.0)
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField]
    private float cosmeticDropChance = 0.5f;

    /// <summary>
    /// Probability of receiving a consumable item when opening a chest (0.0 to 1.0)
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField]
    public float consumableDropChance = 0.8f;

    /// <summary>
    /// List of rarity drop rates for weighted item selection
    /// </summary>
    [SerializeField]
    private List<RarityDropRate> rarityChances;
    #endregion

    #region Component References
    /// <summary>
    /// Reference to the inventory manager for adding items
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// Reference to the level manager for XP management
    /// </summary>
    private LevelManager levelManager;
    #endregion

    #region Configuration
    /// <summary>
    /// Tag for finding the GameManager GameObject
    /// </summary>
    private string GameManagerTag = "GameManager";
    #endregion

    #region Nested Classes
    /// <summary>
    /// Defines the drop rate for a specific item rarity
    /// </summary>
    [System.Serializable]
    public class RarityDropRate
    {
        /// <summary>
        /// The rarity level of the item
        /// </summary>
        public ItemRarity rarity;

        /// <summary>
        /// Drop rate multiplier for this rarity (0.0 to 1.0)
        /// </summary>
        [Range(0f, 1f)]
        public float dropRate;
    }
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        // Find and store references to required managers
        inventoryManager = GameObject
            .FindWithTag(GameManagerTag)
            .GetComponentInChildren<InventoryManager>();
        levelManager = GameObject
            .FindWithTag(GameManagerTag)
            .GetComponentInChildren<LevelManager>();
    }
    #endregion

    #region Chest Opening
    /// <summary>
    /// Opens a chest and distributes rewards based on configured drop chances
    /// </summary>
    public void OpenChest()
    {
        // Generate random value for determining drops
        float random = Random.value;

        // Award random XP within configured range
        int xpGained = Random.Range(minXP, maxXP + 1);
        levelManager.addXP(xpGained);

        // Try to give weapon if drop chance is met
        if (weaponDatabase != null && random <= weaponDropChance)
            TryGiveItem<WeaponItem>(weaponDatabase.AllWeapons);

        // Try to give cosmetic if drop chance is met
        if (cosmeticDatabase != null && random <= cosmeticDropChance)
            TryGiveItem<CosmeticItem>(cosmeticDatabase.AllCosmetics);

        // Try to give consumable if drop chance is met
        if (consumableDatabase != null && random <= consumableDropChance)
            TryGiveItem<ConsumableItem>(consumableDatabase.AllConsumables);
    }
    #endregion

    #region Item Distribution
    /// <summary>
    /// Attempts to give an item of the specified type based on rarity-weighted selection
    /// </summary>
    /// <typeparam name="T">Type of item to give (must inherit from Item)</typeparam>
    /// <param name="sourceList">List of available items to choose from</param>
    private void TryGiveItem<T>(List<T> sourceList)
        where T : Item
    {
        // Validate source list exists and has items
        if (sourceList == null || sourceList.Count == 0)
        {
            Debug.LogWarning($"there are no {typeof(T).Name} items");
            return;
        }

        // Create weighted list based on rarity drop chances
        List<T> weightedList = new List<T>();
        foreach (var item in sourceList)
        {
            // Get drop chance for this item's rarity
            float chance = GetRarityDropChance(item.getRarity());

            // Convert chance to weight (multiply by 100 for integer weights)
            int weight = Mathf.RoundToInt(chance * 100);

            // Add item to weighted list multiple times based on weight
            for (int i = 0; i < weight; i++)
                weightedList.Add(item);
        }

        // Check if weighted list has any items
        if (weightedList.Count == 0)
        {
            Debug.LogWarning($"there are no {typeof(T).Name} items with the right drop chances.");
            return;
        }

        // Select random item from weighted list and add to inventory
        T selected = weightedList[Random.Range(0, weightedList.Count)];
        inventoryManager.AddItemToInventory(selected);
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Gets the drop chance multiplier for a specific item rarity
    /// </summary>
    /// <param name="rarity">The rarity to get the drop chance for</param>
    /// <returns>Drop chance multiplier (0.0 if not configured)</returns>
    private float GetRarityDropChance(ItemRarity rarity)
    {
        // Find matching rarity configuration
        RarityDropRate match = rarityChances.FirstOrDefault(r => r.rarity == rarity);

        // Return configured drop rate or 0 if not found
        return match != null ? match.dropRate : 0f;
    }
    #endregion
}
