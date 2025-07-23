using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CosmeticDatabase", menuName = "Items/Item Database/CosmeticDatabase")]
public class CosmeticDatabase : ScriptableObject
{
    [SerializeField]
    private List<CosmeticItem> allCosmetics;

    public List<CosmeticItem> AllCosmetics => allCosmetics;
}
