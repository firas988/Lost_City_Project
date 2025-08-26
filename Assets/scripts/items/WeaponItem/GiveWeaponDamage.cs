using UnityEngine;

/// <summary>
/// Static utility class that provides weapon damage values based on item rarity.
/// Generates random damage values within predefined ranges for each rarity tier.
/// Used for procedural weapon generation and balancing.
/// </summary>
public static class GiveWeaponDamage
{
    #region Damage Ranges by Rarity
    /// <summary>
    /// Damage range for Common rarity weapons.
    /// Provides balanced starting damage for new players.
    /// </summary>
    private static float[] damageCommon = { 15f, 25f };

    /// <summary>
    /// Damage range for Rare rarity weapons.
    /// Offers improved damage over Common weapons.
    /// </summary>
    private static float[] damageRare = { 25f, 35f };

    /// <summary>
    /// Damage range for Epic rarity weapons.
    /// Provides significant damage increase for experienced players.
    /// </summary>
    private static float[] damageEpic = { 35f, 45f };

    /// <summary>
    /// Damage range for Legendary rarity weapons.
    /// Highest tier damage values for end-game content.
    /// </summary>
    private static float[] damageLegendary = { 45f, 55f };
    #endregion

    #region Damage Generation
    /// <summary>
    /// Generates a random damage value based on the specified item rarity.
    /// Uses predefined damage ranges for each rarity tier.
    /// </summary>
    /// <param name="rarity">The rarity tier of the weapon.</param>
    /// <returns>A rounded random damage value within the rarity's range.</returns>
    public static float getDamage(ItemRarity rarity)
    {
        // Generate random damage based on rarity tier
        switch (rarity)
        {
            case ItemRarity.Common:
                // Return random damage between 15-25 for Common weapons
                return Mathf.Round(Random.Range(damageCommon[0], damageCommon[1]));
            case ItemRarity.Rare:
                // Return random damage between 25-35 for Rare weapons
                return Mathf.Round(Random.Range(damageRare[0], damageRare[1]));
            case ItemRarity.Epic:
                // Return random damage between 35-45 for Epic weapons
                return Mathf.Round(Random.Range(damageEpic[0], damageEpic[1]));
            case ItemRarity.Legendary:
                // Return random damage between 45-55 for Legendary weapons
                return Mathf.Round(Random.Range(damageLegendary[0], damageLegendary[1]));
            default:
                // Return 0 for unknown rarity types
                return 0f;
        }
    }
    #endregion
}
