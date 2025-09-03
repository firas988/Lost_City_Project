using UnityEngine;

/// <summary>
/// Defines the different types of cosmetic armor pieces available in the game.
/// Used to categorize armor and determine which slot it can be equipped in.
/// </summary>
public enum CosmeticType
{
    Helmet,     // Head protection piece
    Chestplate, // Torso protection piece
    Leggings,   // Leg protection piece
    Boots,      // Foot protection piece
}

/// <summary>
/// Represents a cosmetic armor item that can be equipped by the player.
/// Extends the base Item class with armor-specific properties like defense and strength bonuses.
/// Provides methods for accessing armor statistics and managing cosmetic data.
/// </summary>
[CreateAssetMenu(fileName = "NewCosmetic", menuName = "Items/Item/Cosmetic")]
public class CosmeticItem : Item
{
    #region Armor Properties
    /// <summary>
    /// The defense bonus provided by this armor piece.
    /// Reduces incoming damage when equipped.
    /// </summary>
    [SerializeField]
    private float defence;

    /// <summary>
    /// The strength bonus provided by this armor piece.
    /// Increases damage output when equipped.
    /// </summary>
    [SerializeField]
    private float strength;

    /// <summary>
    /// The specific type/category of this cosmetic armor.
    /// Determines which armor slot this item can be equipped in.
    /// </summary>
    [SerializeField]
    private CosmeticType cosmeticType;
    #endregion

    #region Item Description
    /// <summary>
    /// Generates a formatted description of the cosmetic item for display in UI.
    /// Shows defense and strength bonus values.
    /// </summary>
    /// <returns>A formatted string containing armor statistics.</returns>
    public override string getDescription()
    {
        // Format cosmetic description with defense and strength information
        return "Defence: " + defence + "\nStrength: " + strength;
    }
    #endregion

    #region Armor Statistics Management
    /// <summary>
    /// Sets the defense bonus value of this armor piece.
    /// </summary>
    /// <param name="defence">The new defense value to assign.</param>
    public void setDefense(float defence)
    {
        this.defence = defence;
    }

    /// <summary>
    /// Sets the strength bonus value of this armor piece.
    /// </summary>
    /// <param name="strength">The new strength value to assign.</param>
    public void setStrength(float strength)
    {
        this.strength = strength;
    }

    /// <summary>
    /// Gets the defense bonus value of this armor piece.
    /// </summary>
    /// <returns>The armor's defense bonus value.</returns>
    public float getDefense()
    {
        return defence;
    }

    /// <summary>
    /// Gets the strength bonus value of this armor piece.
    /// </summary>
    /// <returns>The armor's strength bonus value.</returns>
    public float getStrength()
    {
        return strength;
    }
    #endregion

    #region Type Information
    /// <summary>
    /// Gets the type/category of this cosmetic armor.
    /// </summary>
    /// <returns>The CosmeticType enum value.</returns>
    public CosmeticType getCosmeticType()
    {
        return cosmeticType;
    }
    #endregion
}
