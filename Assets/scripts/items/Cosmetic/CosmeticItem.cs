using UnityEngine;

public enum CosmeticType
{
    Helmet,
    Chestplate,
    Leggings,
    Boots,
}

[CreateAssetMenu(fileName = "NewCosmetic", menuName = "Items/Item/Cosmetic")]
public class CosmeticItem : Item
{
    [SerializeField]
    private float defence;

    [SerializeField]
    private float strength;

    [SerializeField]
    private CosmeticType cosmeticType;

    public override string getDescription()
    {
        return "Defence: " + defence + "\nStrength: " + strength;
    }

    public void setDefense(float defence)
    {
        this.defence = defence;
    }

    public void setStrength(float strength)
    {
        this.strength = strength;
    }

    public float getDefense()
    {
        return defence;
    }

    public float getStrength()
    {
        return strength;
    }

    public CosmeticType getCosmeticType()
    {
        return cosmeticType;
    }
}
