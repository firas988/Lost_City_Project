using UnityEngine;

public enum ConsumableType
{
    HealthPotion,
    ManaPotion,
    Buff,
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Item/Consumable")]
public class ConsumableItem : Item
{
    [SerializeField]
    private ConsumableType consumableType;

    [SerializeField]
    private string effectDescription;

    public override string getDescription()
    {
        return "Consumable Type: " + consumableType + "\nEffect: " + effectDescription;
    }
}
