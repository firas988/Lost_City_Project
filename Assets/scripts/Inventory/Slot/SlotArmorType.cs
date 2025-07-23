using UnityEngine;

[CreateAssetMenu(fileName = "New SlotArmorType", menuName = "Inventory/SlotArmorType")]
public class SlotArmorType : ScriptableObject
{
    [SerializeField]
    private CosmeticType armorType;

    public CosmeticType getArmorType()
    {
        return armorType;
    }

    public void setArmorType(CosmeticType armorType)
    {
        this.armorType = armorType;
    }
}
