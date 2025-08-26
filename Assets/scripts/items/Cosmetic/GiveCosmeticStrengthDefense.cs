using UnityEngine;

/// <summary>
/// Static utility class that provides cosmetic item statistics based on item rarity.
/// Generates random defense and strength values within predefined ranges for each rarity tier.
/// Used for procedural cosmetic generation and balancing.
/// </summary>
public static class GiveCosmeticStrengthDefense
{
    #region Defense Ranges by Rarity
    /// <summary>
    /// Defense range for Common rarity cosmetic items.
    /// Provides minimal protection bonuses for new players.
    /// </summary>
    private static float[] defenceCommon = { 0.11f, 0.15f };

    /// <summary>
    /// Defense range for Rare rarity cosmetic items.
    /// Offers improved protection over Common items.
    /// </summary>
    private static float[] defenceRare = { 0.15f, 0.2f };

    /// <summary>
    /// Defense range for Epic rarity cosmetic items.
    /// Provides significant protection increase for experienced players.
    /// </summary>
    private static float[] defenceEpic = { 0.2f, 0.3f };

    /// <summary>
    /// Defense range for Legendary rarity cosmetic items.
    /// Highest tier protection values for end-game content.
    /// </summary>
    private static float[] defenceLegendary = { 0.3f, 0.6f };
    #endregion

    #region Strength Ranges by Rarity
    /// <summary>
    /// Strength range for Common rarity cosmetic items.
    /// Provides minimal damage bonuses for new players.
    /// </summary>
    private static float[] strengthCommon = { 0.1f, 0.2f };

    /// <summary>
    /// Strength range for Rare rarity cosmetic items.
    /// Offers improved damage bonuses over Common items.
    /// </summary>
    private static float[] strengthRare = { 0.2f, 0.3f };

    /// <summary>
    /// Strength range for Epic rarity cosmetic items.
    /// Provides significant damage increase for experienced players.
    /// </summary>
    private static float[] strengthEpic = { 0.3f, 0.4f };

    /// <summary>
    /// Strength range for Legendary rarity cosmetic items.
    /// Highest tier damage values for end-game content.
    /// </summary>
    private static float[] strengthLegendary = { 0.4f, 0.5f };
    #endregion

    #region Strength Generation
    /// <summary>
    /// Generates a random strength value based on the specified item rarity.
    /// Uses predefined strength ranges for each rarity tier.
    /// </summary>
    /// <param name="rarity">The rarity tier of the cosmetic item.</param>
    /// <returns>A rounded random strength value within the rarity's range.</returns>
    public static float getStrength(ItemRarity rarity)
    {
        // Generate random strength based on rarity tier
        switch (rarity)
        {
            case ItemRarity.Common:
                // Return random strength between 0.1-0.2 for Common items
                return Mathf.Floor(Random.Range(strengthCommon[0], strengthCommon[1]) * 1000f)
                    / 1000f;
            case ItemRarity.Rare:
                // Return random strength between 0.2-0.3 for Rare items
                return Mathf.Floor(Random.Range(strengthRare[0], strengthRare[1]) * 1000f) / 1000f;
            case ItemRarity.Epic:
                // Return random strength between 0.3-0.4 for Epic items
                return Mathf.Floor(Random.Range(strengthEpic[0], strengthEpic[1]) * 1000f) / 1000f;
            case ItemRarity.Legendary:
                // Return random strength between 0.4-0.5 for Legendary items
                return Mathf.Floor(Random.Range(strengthLegendary[0], strengthLegendary[1]) * 1000f)
                    / 1000f;
            default:
                // Return 0 for unknown rarity types
                return 0f;
        }
    }
    #endregion

    #region Defense Generation
    /// <summary>
    /// Generates a random defense value based on the specified item rarity.
    /// Uses predefined defense ranges for each rarity tier.
    /// </summary>
    /// <param name="rarity">The rarity tier of the cosmetic item.</param>
    /// <returns>A rounded random defense value within the rarity's range.</returns>
    public static float getDefense(ItemRarity rarity)
    {
        // Generate random defense based on rarity tier
        switch (rarity)
        {
            case ItemRarity.Common:
                // Return random defense between 0.11-0.15 for Common items
                return Mathf.Floor(Random.Range(defenceCommon[0], defenceCommon[1]) * 1000f)
                    / 1000f;
            case ItemRarity.Rare:
                // Return random defense between 0.15-0.2 for Rare items
                return Mathf.Floor(Random.Range(defenceRare[0], defenceRare[1]) * 1000f) / 1000f;
            case ItemRarity.Epic:
                // Return random defense between 0.2-0.3 for Epic items
                return Mathf.Floor(Random.Range(defenceEpic[0], defenceEpic[1]) * 1000f) / 1000f;
            case ItemRarity.Legendary:
                // Return random defense between 0.3-0.6 for Legendary items
                return Mathf.Floor(Random.Range(defenceLegendary[0], defenceLegendary[1]) * 1000f)
                    / 1000f;
            default:
                // Return 0 for unknown rarity types
                return 0f;
        }
    }
    #endregion
}
