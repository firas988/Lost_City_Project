using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database that contains all available cosmetic items in the game.
/// Provides centralized access to cosmetic data for inventory, crafting, and loot systems.
/// Can be created and configured through the Unity menu system.
/// </summary>
[CreateAssetMenu(fileName = "CosmeticDatabase", menuName = "Items/Item Database/CosmeticDatabase")]
public class CosmeticDatabase : ScriptableObject
{
    #region Cosmetic Collection
    /// <summary>
    /// Collection of all cosmetic items available in the game.
    /// Contains cosmetic data for different types, rarities, and tiers.
    /// </summary>
    [SerializeField]
    private List<CosmeticItem> allCosmetics;

    /// <summary>
    /// Public accessor for the cosmetic collection.
    /// Provides read-only access to all cosmetics in the database.
    /// </summary>
    public List<CosmeticItem> AllCosmetics => allCosmetics;
    #endregion
}
