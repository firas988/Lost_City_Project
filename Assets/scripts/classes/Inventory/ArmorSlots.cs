using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages armor slot inventory system with individual slots for helmet, chestplate, leggings, and boots
/// </summary>
public class ArmorSlots
{
    #region Private Fields
    /// <summary>
    /// List of items stored in the armor slots (index 0=helmet, 1=chestplate, 2=leggings, 3=boots)
    /// </summary>
    private List<Item> armorSlots;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor that initializes the armor slots with the specified number of slots
    /// </summary>
    /// <param name="size">The number of slots to initialize (typically 4 for standard armor)</param>
    public ArmorSlots(int size)
    {
        // COMPLEXITY ANALYSIS: ArmorSlots() - O(s) where s = size
        // Initialize the armor slots list
        armorSlots = new List<Item>();

        // Fill all slots with null initially
        for (int i = 0; i < size; i++)
        {
            armorSlots.Add(null);
        }
    }
    #endregion

    #region Setter Methods
    /// <summary>
    /// Sets the helmet in the armor slots (index 0)
    /// </summary>
    /// <param name="item">The item to set in the helmet slot</param>
    public void setHelmet(Item item)
    {
        // COMPLEXITY ANALYSIS: setHelmet() - O(1)
        armorSlots[0] = item;
    }

    /// <summary>
    /// Sets the chestplate in the armor slots (index 1)
    /// </summary>
    /// <param name="item">The item to set in the chestplate slot</param>
    public void setChestplate(Item item)
    {
        // COMPLEXITY ANALYSIS: setChestplate() - O(1)
        armorSlots[1] = item;
    }

    /// <summary>
    /// Sets the leggings in the armor slots (index 2)
    /// </summary>
    /// <param name="item">The item to set in the leggings slot</param>
    public void setLeggings(Item item)
    {
        // COMPLEXITY ANALYSIS: setLeggings() - O(1)
        armorSlots[2] = item;
    }

    /// <summary>
    /// Sets the boots in the armor slots (index 3)
    /// </summary>
    /// <param name="item">The item to set in the boots slot</param>
    public void setBoots(Item item)
    {
        // COMPLEXITY ANALYSIS: setBoots() - O(1)
        armorSlots[3] = item;
    }
    #endregion

    #region Getter Methods
    /// <summary>
    /// Gets the helmet from the armor slots (index 0)
    /// </summary>
    /// <returns>The helmet item, or null if no helmet is equipped</returns>
    public Item getHelmet()
    {
        // COMPLEXITY ANALYSIS: getHelmet() - O(1)
        return armorSlots[0];
    }

    /// <summary>
    /// Gets the chestplate from the armor slots (index 1)
    /// </summary>
    /// <returns>The chestplate item, or null if no chestplate is equipped</returns>
    public Item getChestplate()
    {
        // COMPLEXITY ANALYSIS: getChestplate() - O(1)
        return armorSlots[1];
    }

    /// <summary>
    /// Gets the leggings from the armor slots (index 2)
    /// </summary>
    /// <returns>The leggings item, or null if no leggings are equipped</returns>
    public Item getLeggings()
    {
        // COMPLEXITY ANALYSIS: getLeggings() - O(1)
        return armorSlots[2];
    }

    /// <summary>
    /// Gets the boots from the armor slots (index 3)
    /// </summary>
    /// <returns>The boots item, or null if no boots are equipped</returns>
    public Item getBoots()
    {
        // COMPLEXITY ANALYSIS: getBoots() - O(1)
        return armorSlots[3];
    }
    #endregion

    #region Removal Methods
    /// <summary>
    /// Removes the helmet from the armor slots (index 0)
    /// </summary>
    public void removeHelmet()
    {
        // COMPLEXITY ANALYSIS: removeHelmet() - O(1)
        armorSlots[0] = null;
    }

    /// <summary>
    /// Removes the chestplate from the armor slots (index 1)
    /// </summary>
    public void removeChestplate()
    {
        // COMPLEXITY ANALYSIS: removeChestplate() - O(1)
        armorSlots[1] = null;
    }

    /// <summary>
    /// Removes the leggings from the armor slots (index 2)
    /// </summary>
    public void removeLeggings()
    {
        // COMPLEXITY ANALYSIS: removeLeggings() - O(1)
        armorSlots[2] = null;
    }

    /// <summary>
    /// Removes the boots from the armor slots (index 3)
    /// </summary>
    public void removeBoots()
    {
        // COMPLEXITY ANALYSIS: removeBoots() - O(1)
        armorSlots[3] = null;
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Gets the complete list of armor slots for external access
    /// </summary>
    /// <returns>List containing all armor slot items</returns>
    public List<Item> getArmorSlots()
    {
        // COMPLEXITY ANALYSIS: getArmorSlots() - O(1)
        return armorSlots;
    }
    #endregion

    #region Stat Calculation Methods
    /// <summary>
    /// Calculates the total defense bonus from all equipped armor pieces
    /// </summary>
    /// <returns>Total defense bonus value from all equipped cosmetic armor items</returns>
    public float getDefenseBonus()
    {
        // COMPLEXITY ANALYSIS: getDefenseBonus() - O(a) where a = number of armor slots
        float defenseBonus = 0f;

        // Iterate through all armor slots to calculate total defense
        foreach (Item item in armorSlots)
        {
            // Check if item is a cosmetic item and not null, then add its defense value
            defenseBonus +=
                (item is CosmeticItem) && (item != null) ? (item as CosmeticItem).getDefense() : 0f;
        }
        return defenseBonus;
    }

    /// <summary>
    /// Calculates the total strength bonus from all equipped armor pieces
    /// </summary>
    /// <returns>Total strength bonus value from all equipped cosmetic armor items</returns>
    public float getStrengthBonus()
    {
        // COMPLEXITY ANALYSIS: getStrengthBonus() - O(a) where a = number of armor slots
        float strengthBonus = 0f;

        // Iterate through all armor slots to calculate total strength
        foreach (Item item in armorSlots)
        {
            // Check if item is a cosmetic item and not null, then add its strength value
            strengthBonus +=
                (item is CosmeticItem) && (item != null)
                    ? (item as CosmeticItem).getStrength()
                    : 0f;
        }
        return strengthBonus;
    }
    #endregion
}
