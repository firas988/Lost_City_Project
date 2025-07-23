using System.Collections.Generic;
using UnityEngine;

public class HotBar
{
    private List<List<Item>> items;
    private int weaponIndex = 0;

    public HotBar(int size)
    {
        items = new List<List<Item>>();
        for (int i = 0; i < size; i++)
        {
            items.Add(new List<Item>());
        }
    }

    public void setWeapon(Item item )
    {
        items[weaponIndex].Add(item);
    }

    public List<Item> getWeapon()
    {
        return items[weaponIndex];
    }

    public void removeWeapon( )
    {
        items[weaponIndex] = new List<Item>();
    }

    public void setConsumable(Item item, int count, int index)
    {
        if (index > 0 && index < items.Count)
        {
            for (int i = 0; i < count; i++)
            {
                items[index].Add(item);
            }
        }
    }

    public bool addToConsumable(Item item, int count, int index)
    {
        if (index > 0 && index < items.Count)
        {
            if (items[index].Count + count > item.maxStack)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                items[index].Add(item);
            }
            return true;
        }
        return false;
    }

    public List<Item> getConsumable(int index)
    {
        if (index > 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    public void removeConsumable(int index)
    {
        if (index > 0 && index < items.Count)
        {
            items[index] = new List<Item>();
        }
    }
}
