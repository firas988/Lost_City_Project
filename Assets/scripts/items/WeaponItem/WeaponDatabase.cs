using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database that contains all available weapons in the game.
/// Provides centralized access to weapon data for inventory, crafting, and loot systems.
/// Can be created and configured through the Unity menu system.
/// </summary>
[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Items/Item Database/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    #region Weapon Collection
    /// <summary>
    /// Collection of all weapon items available in the game.
    /// Contains weapon data for different types, rarities, and tiers.
    /// </summary>
    [SerializeField]
    private List<WeaponItem> allWeapons;

    /// <summary>
    /// Public accessor for the weapon collection.
    /// Provides read-only access to all weapons in the database.
    /// </summary>
    public List<WeaponItem> AllWeapons => allWeapons;
    #endregion
}
