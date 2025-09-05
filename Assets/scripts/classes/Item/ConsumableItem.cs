using UnityEngine;

/// <summary>
/// Defines the different types of consumable items available in the game.
/// Used to categorize consumables and determine their effects and behavior.
/// </summary>
public enum ConsumableType
{
    HealthRegenerationPotion, // Gradually restores health over time
    HealthInstantPotion, // Instantly restores a fixed amount of health
    StrengthPotion, // Temporarily increases player strength
    SpeedPotion, // Temporarily increases player movement speed
}

/// <summary>
/// Represents a consumable item that can be used by the player for various effects.
/// Extends the base Item class with consumable-specific properties like effect duration and potion effects.
/// Provides properties for accessing consumable statistics and effect data.
/// </summary>
[CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Item/Consumable")]
public class ConsumableItem : Item
{
    #region Consumable Properties
    /// <summary>
    /// The specific type/category of this consumable item.
    /// Determines the effect and behavior when consumed.
    /// </summary>
    [SerializeField]
    private ConsumableType consumableType;

    /// <summary>
    /// The duration of the consumable's effect in seconds.
    /// How long the effect lasts after consumption.
    /// </summary>
    [SerializeField]
    private float effectDuration;

    /// <summary>
    /// A text description of the consumable's effect.
    /// Used for UI display and player information.
    /// </summary>
    [SerializeField]
    private string effectDescription;
    #endregion

    #region Effect Values
    /// <summary>
    /// The amount of health regenerated per second for regeneration potions.
    /// Only applies to HealthRegenerationPotion type.
    /// </summary>
    [SerializeField]
    private float healthRegenerationAmount;

    /// <summary>
    /// The strength bonus amount provided by strength potions.
    /// Only applies to StrengthPotion type.
    /// </summary>
    [SerializeField]
    private float strengthAmount;

    /// <summary>
    /// The speed bonus amount provided by speed potions.
    /// Only applies to SpeedPotion type.
    /// </summary>
    [SerializeField]
    private float speedAmount;
    #endregion

    #region Item Description
    /// <summary>
    /// Generates a formatted description of the consumable item for display in UI.
    /// Shows consumable type and effect description.
    /// </summary>
    /// <returns>A formatted string containing consumable information.</returns>
    // COMPLEXITY ANALYSIS: getDescription() - O(1)
    public override string getDescription()
    {
        // Format consumable description with type and effect information
        return "Consumable Type: " + consumableType + "\nEffect: " + effectDescription;
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the type of this consumable item.
    /// </summary>
    public ConsumableType ConsumableType => consumableType;

    /// <summary>
    /// Gets the duration of the consumable's effect.
    /// </summary>
    public float EffectDuration => effectDuration;

    /// <summary>
    /// Gets the description of the consumable's effect.
    /// </summary>
    public string EffectDescription => effectDescription;

    /// <summary>
    /// Gets the health regeneration amount for regeneration potions.
    /// </summary>
    public float HealthRegenerationAmount => healthRegenerationAmount;

    /// <summary>
    /// Gets the strength bonus amount for strength potions.
    /// </summary>
    public float StrengthAmount => strengthAmount;

    /// <summary>
    /// Gets the speed bonus amount for speed potions.
    /// </summary>
    public float SpeedAmount => speedAmount;
    #endregion
}
