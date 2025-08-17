using System.Collections.Generic;
using UnityEngine;

public class ArmorSlots
{
    /// <summary>
    /// List of items stored in the armor slots.
    /// </summary>
    private List<Item> armorSlots;

    /// <summary>
    /// Constructor that initializes the armor slots.
    /// </summary>
    /// <param name="size">The number of slots to initialize.</param>
    public ArmorSlots(int size)
    {
        armorSlots = new List<Item>();
        for (int i = 0; i < size; i++)
        {
            armorSlots.Add(null);
        }
    }

    /// <summary>
    /// Sets the helmet in the armor slots.
    /// </summary>
    /// <param name="item">The item to set in the helmet slot.</param>
    public void setHelmet(Item item)
    {
        armorSlots[0] = item;
    }

    /// <summary>
    /// Sets the chestplate in the armor slots.
    /// </summary>
    /// <param name="item">The item to set in the chestplate slot.</param>
    public void setChestplate(Item item)
    {
        armorSlots[1] = item;
    }

    /// <summary>
    /// Sets the leggings in the armor slots.
    /// </summary>
    /// <param name="item">The item to set in the leggings slot.</param>
    public void setLeggings(Item item)
    {
        armorSlots[2] = item;
    }

    /// <summary>
    /// Sets the boots in the armor slots.
    /// </summary>
    /// <param name="item">The item to set in the boots slot.</param>
    public void setBoots(Item item)
    {
        armorSlots[3] = item;
    }

    /// <summary>
    /// Gets the helmet from the armor slots.
    /// </summary>
    /// <returns>The helmet item.</returns>
    public Item getHelmet()
    {
        return armorSlots[0];
    }

    /// <summary>
    /// Gets the chestplate from the armor slots.
    /// </summary>
    /// <returns>The chestplate item.</returns>
    public Item getChestplate()
    {
        return armorSlots[1];
    }

    /// <summary>
    /// Gets the leggings from the armor slots.
    /// </summary>
    /// <returns>The leggings item.</returns>
    public Item getLeggings()
    {
        return armorSlots[2];
    }

    /// <summary>
    /// Gets the boots from the armor slots.
    /// </summary>
    /// <returns>The boots item.</returns>
    public Item getBoots()
    {
        return armorSlots[3];
    }

    /// <summary>
    /// Removes the helmet from the armor slots.
    /// </summary>
    public void removeHelmet()
    {
        armorSlots[0] = null;
    }

    /// <summary>
    /// Removes the chestplate from the armor slots.
    /// </summary>
    public void removeChestplate()
    {
        armorSlots[1] = null;
    }

    /// <summary>
    /// Removes the leggings from the armor slots.
    /// </summary>
    public void removeLeggings()
    {
        armorSlots[2] = null;
    }

    /// <summary>
    /// Removes the boots from the armor slots.
    /// </summary>
    public void removeBoots()
    {
        armorSlots[3] = null;
    }

    public List<Item> getArmorSlots()
    {
        return armorSlots;
    }

    public float getDefenseBonus()
    {
        float defenseBonus = 0f;
        foreach (Item item in armorSlots)
        {
            defenseBonus +=
                (item is CosmeticItem) && (item != null) ? (item as CosmeticItem).getDefense() : 0f;
        }
        return defenseBonus;
    }

    public float getStrengthBonus()
    {
        float strengthBonus = 0f;
        foreach (Item item in armorSlots)
        {
            strengthBonus +=
                (item is CosmeticItem) && (item != null)
                    ? (item as CosmeticItem).getStrength()
                    : 0f;
        }
        return strengthBonus;
    }
}
