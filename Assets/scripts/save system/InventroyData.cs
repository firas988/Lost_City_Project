using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventroyData
{
    public List<int> row;
    public List<int> column;
    public List<int> count;
    public List<float?> damage;
    public List<float?> defence;
    public List<float?> strength;
    public List<int> id;

    public InventroyData(Inventory inventory)
    {
        row = new List<int>();
        column = new List<int>();
        count = new List<int>();
        id = new List<int>();
        damage = new List<float?>();
        defence = new List<float?>();
        strength = new List<float?>();

        List<Item>[,] items = inventory.GetItems();

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
}
