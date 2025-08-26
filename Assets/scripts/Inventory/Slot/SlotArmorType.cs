using UnityEngine;

/// <summary>
/// ScriptableObject that defines the type of armor that can be equipped in a specific armor slot.
/// Provides a reusable way to configure armor slot restrictions and validation.
/// Can be created through the Unity menu system for easy armor slot configuration.
/// </summary>
[CreateAssetMenu(fileName = "New SlotArmorType", menuName = "Inventory/SlotArmorType")]
public class SlotArmorType : ScriptableObject
{
    #region Armor Type Configuration
    /// <summary>
    /// The specific type of cosmetic armor that this slot accepts.
    /// Defines what category of armor can be equipped (helmet, chestplate, leggings, boots).
    /// </summary>
    [SerializeField]
    private CosmeticType armorType;
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Gets the armor type that this slot accepts.
    /// </summary>
    /// <returns>The CosmeticType defining what can be equipped in this slot.</returns>
    public CosmeticType getArmorType()
    {
        return armorType;
    }

    /// <summary>
    /// Sets the armor type that this slot accepts.
    /// </summary>
    /// <param name="armorType">The CosmeticType to assign to this slot.</param>
    public void setArmorType(CosmeticType armorType)
    {
        this.armorType = armorType;
    }
    #endregion
}
