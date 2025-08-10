using UnityEngine;

public enum ConsumableType
{
    HealthRegenerationPotion,
    HealthInstantPotion,
    StrengthPotion,
    SpeedPotion,

}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Item/Consumable")]
public class ConsumableItem : Item
{
    [SerializeField]
    private ConsumableType consumableType;

    [SerializeField]
    private float effectDuration;

    [SerializeField]
    private string effectDescription;

    [SerializeField]
    private float healthRegenerationAmount;

    [SerializeField]
    private float strengthAmount;

    [SerializeField]
    private float speedAmount;

    public override string getDescription()
    {
        return "Consumable Type: " + consumableType + "\nEffect: " + effectDescription;
    }

    public ConsumableType ConsumableType => consumableType;
    public float EffectDuration => effectDuration;
    public string EffectDescription => effectDescription;
    public float HealthRegenerationAmount => healthRegenerationAmount;
    public float StrengthAmount => strengthAmount;
    public float SpeedAmount => speedAmount;
}
