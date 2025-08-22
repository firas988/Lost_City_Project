using UnityEngine;

public static class GiveCosmeticStrengthDefense
{
    private static float[] defenceCommon = { 0.11f, 0.15f };
    private static float[] defenceRare = { 0.15f, 0.2f };
    private static float[] defenceEpic = { 0.2f, 0.3f };
    private static float[] defenceLegendary = { 0.3f, 0.6f };

    private static float[] strengthCommon = { 0.1f, 0.2f };
    private static float[] strengthRare = { 0.2f, 0.3f };
    private static float[] strengthEpic = { 0.3f, 0.4f };
    private static float[] strengthLegendary = { 0.4f, 0.5f };

    public static float getStrength(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Mathf.Floor(Random.Range(strengthCommon[0], strengthCommon[1]) * 1000f)
                    / 1000f;
            case ItemRarity.Rare:
                return Mathf.Floor(Random.Range(strengthRare[0], strengthRare[1]) * 1000f) / 1000f;
            case ItemRarity.Epic:
                return Mathf.Floor(Random.Range(strengthEpic[0], strengthEpic[1]) * 1000f) / 1000f;
            case ItemRarity.Legendary:
                return Mathf.Floor(Random.Range(strengthLegendary[0], strengthLegendary[1]) * 1000f)
                    / 1000f;
            default:
                return 0f;
        }
    }

    public static float getDefense(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Mathf.Floor(Random.Range(defenceCommon[0], defenceCommon[1]) * 1000f)
                    / 1000f;
            case ItemRarity.Rare:
                return Mathf.Floor(Random.Range(defenceRare[0], defenceRare[1]) * 1000f) / 1000f;
            case ItemRarity.Epic:
                return Mathf.Floor(Random.Range(defenceEpic[0], defenceEpic[1]) * 1000f) / 1000f;
            case ItemRarity.Legendary:
                return Mathf.Floor(Random.Range(defenceLegendary[0], defenceLegendary[1]) * 1000f)
                    / 1000f;
            default:
                return 0f;
        }
    }
}
