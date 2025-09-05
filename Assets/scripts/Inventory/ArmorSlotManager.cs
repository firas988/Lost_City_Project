using UnityEngine;

/// <summary>
/// Manages the collection of armor slots in the inventory system, providing a centralized
/// interface for equipping and removing armor items. Coordinates between the inventory
/// data structure and the individual armor slot UI components.
/// </summary>
public class ArmorSlotManager : MonoBehaviour
{
    #region Armor Slot References
    /// <summary>Reference to the helmet armor slot component.</summary>
    [SerializeField]
    private ArmorSlot helmetSlot;

    /// <summary>Reference to the chestplate armor slot component.</summary>
    [SerializeField]
    private ArmorSlot chestplateSlot;

    /// <summary>Reference to the leggings armor slot component.</summary>
    [SerializeField]
    private ArmorSlot leggingsSlot;

    /// <summary>Reference to the boots armor slot component.</summary>
    [SerializeField]
    private ArmorSlot bootsSlot;
    #endregion

    #region Armor Equipping Methods
    /// <summary>
    /// Equips an item to the helmet slot.
    /// </summary>
    /// <param name="item">The helmet item to equip.</param>
    // COMPLEXITY ANALYSIS: setHelmet() - O(1)
    public void setHelmet(Item item)
    {
        helmetSlot.setItem(item);
    }

    /// <summary>
    /// Equips an item to the chestplate slot.
    /// </summary>
    /// <param name="item">The chestplate item to equip.</param>
    // COMPLEXITY ANALYSIS: setChestplate() - O(1)
    public void setChestplate(Item item)
    {
        chestplateSlot.setItem(item);
    }

    /// <summary>
    /// Equips an item to the leggings slot.
    /// </summary>
    /// <param name="item">The leggings item to equip.</param>
    // COMPLEXITY ANALYSIS: setLeggings() - O(1)
    public void setLeggings(Item item)
    {
        leggingsSlot.setItem(item);
    }

    /// <summary>
    /// Equips an item to the boots slot.
    /// </summary>
    /// <param name="item">The boots item to equip.</param>
    // COMPLEXITY ANALYSIS: setBoots() - O(1)
    public void setBoots(Item item)
    {
        bootsSlot.setItem(item);
    }
    #endregion

    #region Armor Removal Methods
    /// <summary>
    /// Removes the item from the helmet slot.
    /// </summary>
    // COMPLEXITY ANALYSIS: removeHelmet() - O(1)
    public void removeHelmet()
    {
        helmetSlot.removeItem();
    }

    /// <summary>
    /// Removes the item from the chestplate slot.
    /// </summary>
    // COMPLEXITY ANALYSIS: removeChestplate() - O(1)
    public void removeChestplate()
    {
        chestplateSlot.removeItem();
    }

    /// <summary>
    /// Removes the item from the leggings slot.
    /// </summary>
    // COMPLEXITY ANALYSIS: removeLeggings() - O(1)
    public void removeLeggings()
    {
        leggingsSlot.removeItem();
    }

    /// <summary>
    /// Removes the item from the boots slot.
    /// </summary>
    // COMPLEXITY ANALYSIS: removeBoots() - O(1)
    public void removeBoots()
    {
        bootsSlot.removeItem();
    }
    #endregion
}
