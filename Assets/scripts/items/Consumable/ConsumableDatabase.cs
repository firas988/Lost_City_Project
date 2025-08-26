using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database that contains all available consumable items in the game.
/// Provides centralized access to consumable data for inventory, crafting, and loot systems.
/// Can be created and configured through the Unity menu system.
/// </summary>
[CreateAssetMenu(
    fileName = "ConsumableDatabase",
    menuName = "Items/Item Database/ConsumableDatabase"
)]
public class ConsumableDatabase : ScriptableObject
{
    #region Consumable Collection
    /// <summary>
    /// Collection of all consumable items available in the game.
    /// Contains consumable data for different types, rarities, and tiers.
    /// </summary>
    [SerializeField]
    private List<ConsumableItem> allConsumables;

    /// <summary>
    /// Public accessor for the consumable collection.
    /// Provides read-only access to all consumables in the database.
    /// </summary>
    public List<ConsumableItem> AllConsumables => allConsumables;
    #endregion
}
