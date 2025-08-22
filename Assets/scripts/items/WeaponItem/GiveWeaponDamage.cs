using UnityEngine;

public static class GiveWeaponDamage
{
    private static float[] damageCommon = { 15f, 25f };
    private static float[] damageRare = { 25f, 35f };
    private static float[] damageEpic = { 35f, 45f };
    private static float[] damageLegendary = { 45f, 55f };

    public static float getDamage(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Mathf.Round(Random.Range(damageCommon[0], damageCommon[1]));
            case ItemRarity.Rare:
                return Mathf.Round(Random.Range(damageRare[0], damageRare[1]));
            case ItemRarity.Epic:
                return Mathf.Round(Random.Range(damageEpic[0], damageEpic[1]));
            case ItemRarity.Legendary:
                return Mathf.Round(Random.Range(damageLegendary[0], damageLegendary[1]));
            default:
                return 0f;
        }
    }
}
