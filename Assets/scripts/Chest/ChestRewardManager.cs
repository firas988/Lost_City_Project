using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestRewardManager : MonoBehaviour
{
    [SerializeField]
    private WeaponDatabase weaponDatabase;

    [SerializeField]
    private CosmeticDatabase cosmeticDatabase;

    [SerializeField]
    private ConsumableDatabase consumableDatabase;

    [SerializeField]
    private int minXP = 50;

    [SerializeField]
    private int maxXP = 150;

    [Range(0f, 1f)]
    [SerializeField]
    private float weaponDropChance = 0.7f;

    [Range(0f, 1f)]
    [SerializeField]
    private float cosmeticDropChance = 0.5f;

    [Range(0f, 1f)]
    [SerializeField]
    public float consumableDropChance = 0.8f;

    private InventoryManager inventoryManager;

    private string GameManagerTag = "GameManager";

    private LevelManager levelManager;

    [SerializeField]
    private List<RarityDropRate> rarityChances;

    [System.Serializable]
    public class RarityDropRate
    {
        public ItemRarity rarity;

        [Range(0f, 1f)]
        public float dropRate;
    }

    private void Start()
    {
        inventoryManager = GameObject
            .FindWithTag(GameManagerTag)
            .GetComponentInChildren<InventoryManager>();
        levelManager = GameObject
            .FindWithTag(GameManagerTag)
            .GetComponentInChildren<LevelManager>();
    }

    public void OpenChest()
    {
        float random = Random.value;
        int xpGained = Random.Range(minXP, maxXP + 1);
        levelManager.addXP(xpGained);
        if (weaponDatabase != null && random <= weaponDropChance)
            TryGiveItem<WeaponItem>(weaponDatabase.AllWeapons);
        if (cosmeticDatabase != null && random <= cosmeticDropChance)
            TryGiveItem<CosmeticItem>(cosmeticDatabase.AllCosmetics);
        if (consumableDatabase != null && random <= consumableDropChance)
            TryGiveItem<ConsumableItem>(consumableDatabase.AllConsumables);
    }

    private void TryGiveItem<T>(List<T> sourceList)
        where T : Item
    {
        if (sourceList == null || sourceList.Count == 0)
        {
            Debug.LogWarning($"there are no {typeof(T).Name} items");
            return;
        }

        List<T> weightedList = new List<T>();
        foreach (var item in sourceList)
        {
            float chance = GetRarityDropChance(item.rarity);
            int weight = Mathf.RoundToInt(chance * 100);
            for (int i = 0; i < weight; i++)
                weightedList.Add(item);
        }

        if (weightedList.Count == 0)
        {
            Debug.LogWarning($"there are no {typeof(T).Name} items with the right drop chances.");
            return;
        }

        T selected = weightedList[Random.Range(0, weightedList.Count)];
        inventoryManager.AddItemToInventory(selected);
    }

    private float GetRarityDropChance(ItemRarity rarity)
    {
        RarityDropRate match = rarityChances.FirstOrDefault(r => r.rarity == rarity);
        return match != null ? match.dropRate : 0f;
    }
}
