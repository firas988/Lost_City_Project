using UnityEngine;

/// <summary>
/// Defines the different types of weapons available in the game.
/// Used to categorize weapons and determine their behavior and appearance.
/// </summary>
public enum WeaponType
{
    Sword, // Melee weapon with balanced damage and range
    Axe, // Heavy melee weapon with high damage but lower range
}

/// <summary>
/// Represents a weapon item that can be equipped by the player.
/// Extends the base Item class with weapon-specific properties like damage, range, and type.
/// Provides methods for accessing weapon statistics and managing weapon data.
/// </summary>
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Item/Weapon")]
public class WeaponItem : Item
{
    #region Weapon Properties
    /// <summary>
    /// The base damage dealt by this weapon when attacking.
    /// Higher values result in more powerful attacks.
    /// </summary>
    [SerializeField]
    private float damage;

    /// <summary>
    /// The effective range of this weapon for attacking.
    /// Determines how far the player can attack from.
    /// </summary>
    [SerializeField]
    private float range;

    /// <summary>
    /// The specific type/category of this weapon.
    /// Determines weapon behavior, animations, and visual appearance.
    /// </summary>
    [SerializeField]
    private WeaponType weaponType;
    #endregion

    #region Item Description
    /// <summary>
    /// Generates a formatted description of the weapon for display in UI.
    /// Shows damage value and weapon type information.
    /// </summary>
    /// <returns>A formatted string containing weapon statistics.</returns>
    public override string getDescription()
    {
        // Format weapon description with damage and type information
        return "Damage: " + damage + "\nWeapon Type: " + weaponType;
    }
    #endregion

    #region Weapon Statistics Access
    /// <summary>
    /// Gets the base damage value of this weapon.
    /// </summary>
    /// <returns>The weapon's damage value.</returns>
    public float getDamage()
    {
        return damage;
    }

    /// <summary>
    /// Sets the base damage value of this weapon.
    /// </summary>
    /// <param name="damage">The new damage value to assign.</param>
    public void setDamage(float damage)
    {
        this.damage = damage;
    }

    /// <summary>
    /// Gets the effective range of this weapon.
    /// </summary>
    /// <returns>The weapon's range value.</returns>
    public float getRange()
    {
        return range;
    }

    /// <summary>
    /// Gets the type/category of this weapon.
    /// </summary>
    /// <returns>The WeaponType enum value.</returns>
    public WeaponType getWeaponType()
    {
        return weaponType;
    }
    #endregion

    #region Prefab Access
    /// <summary>
    /// Gets the 3D model/prefab associated with this weapon.
    /// Used for visual representation and weapon spawning.
    /// </summary>
    /// <returns>The weapon's GameObject prefab.</returns>
    public GameObject getWeaponPrefab()
    {
        return getItemPrefab();
    }
    #endregion
}
