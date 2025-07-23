using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ConsumableDatabase",
    menuName = "Items/Item Database/ConsumableDatabase"
)]
public class ConsumableDatabase : ScriptableObject
{
    [SerializeField]
    private List<ConsumableItem> allConsumables;

    public List<ConsumableItem> AllConsumables => allConsumables;
}
