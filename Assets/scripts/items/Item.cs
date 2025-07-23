using UnityEngine;

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
}

public enum ItemCategory
{
    Weapon,
    Cosmetic,
    Consumable,
}

[System.Serializable]
public abstract class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemRarity rarity;
    public ItemCategory category;
    public Sprite icon;
    public int maxStack;
    public GameObject itemPrefab;

    public abstract string getDescription();
}
