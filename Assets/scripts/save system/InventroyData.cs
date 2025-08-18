using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventroyData
{
    /// <summary>
    /// Inventory data
    /// </summary>
    [SerializeField]
    private List<int> row;

    [SerializeField]
    private List<int> column;

    [SerializeField]
    private List<int> count;

    [SerializeField]
    private List<float?> damage;

    [SerializeField]
    private List<float?> defence;

    [SerializeField]
    private List<float?> strength;

    [SerializeField]
    private List<int> id;

    /// <summary>
    /// hotbar data
    /// </summary>
    [SerializeField]
    private List<int> idItemInHotbar;

    [SerializeField]
    private List<int> countItemInHotbar;

    [SerializeField]
    private float? weaponDamage;

    /// <summary>
    /// armor slots data
    /// </summary>
    [SerializeField]
    private List<int> idItemInArmorSlots;

    [SerializeField]
    private List<float?> armorSlotsDefence;

    [SerializeField]
    private List<float?> armorSlotsStrength;

    public InventroyData(Inventory inventory)
    {
        //inventory data
        row = new List<int>();
        column = new List<int>();
        count = new List<int>();
        id = new List<int>();
        damage = new List<float?>();
        defence = new List<float?>();
        strength = new List<float?>();

        //hotbar data
        idItemInHotbar = new List<int>();
        countItemInHotbar = new List<int>();

        //armor slots data
        idItemInArmorSlots = new List<int>();
        armorSlotsDefence = new List<float?>();
        armorSlotsStrength = new List<float?>();

        List<Item>[,] items = inventory.GetItems();
        setInventory(items);

        List<List<Item>> itemsInHotbar = inventory.getHotbar().getItems();
        setHotbar(itemsInHotbar);

        List<Item> armorSlots = inventory.getArmorSlots().getArmorSlots();
        setArmorSlots(armorSlots);
    }

    public void setInventory(List<Item>[,] items)
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                if (items[i, j] != null)
                {
                    row.Add(i);
                    column.Add(j);
                    count.Add(items[i, j].Count);
                    id.Add(items[i, j][0].id);
                    if (items[i, j][0] is WeaponItem)
                    {
                        damage.Add(((WeaponItem)items[i, j][0]).getDamage());
                    }
                    else
                    {
                        damage.Add(null);
                    }
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

    public void setHotbar(List<List<Item>> itemsInHotbar)
    {
        for (int i = 0; i < itemsInHotbar.Count; i++)
        {
            if (itemsInHotbar[i].Count > 0)
            {
                idItemInHotbar.Add(itemsInHotbar[i][0].id);
                countItemInHotbar.Add(itemsInHotbar[i].Count);
                if (itemsInHotbar[i][0] is WeaponItem)
                {
                    weaponDamage = ((WeaponItem)itemsInHotbar[i][0]).getDamage();
                }
            }
            else
            {
                idItemInHotbar.Add(-1);
                countItemInHotbar.Add(0);
            }
        }
    }

    public void setArmorSlots(List<Item> armorSlots)
    {
        for (int i = 0; i < armorSlots.Count; i++)
        {
            if (armorSlots[i] != null)
            {
                idItemInArmorSlots.Add(armorSlots[i].id);
                armorSlotsDefence.Add(((CosmeticItem)armorSlots[i]).getDefense());
                armorSlotsStrength.Add(((CosmeticItem)armorSlots[i]).getStrength());
            }
            else
            {
                idItemInArmorSlots.Add(-1);
                armorSlotsDefence.Add(null);
                armorSlotsStrength.Add(null);
            }
        }
    }

    /// <summary>
    /// getters for inventory data
    /// </summary>
    public List<int> Row => row;
    public List<int> Column => column;
    public List<int> Count => count;
    public List<float?> Damage => damage;
    public List<float?> Defence => defence;
    public List<float?> Strength => strength;
    public List<int> Id => id;

    /// <summary>
    /// getters for hotbar data
    /// </summary>
    public List<int> IdItemInHotbar => idItemInHotbar;
    public List<int> CountItemInHotbar => countItemInHotbar;
    public float? WeaponDamage => weaponDamage;

    /// <summary>
    /// getters for armor slots data
    /// </summary>
    public List<int> IdItemInArmorSlots => idItemInArmorSlots;
    public List<float?> ArmorSlotsDefence => armorSlotsDefence;
    public List<float?> ArmorSlotsStrength => armorSlotsStrength;
}
