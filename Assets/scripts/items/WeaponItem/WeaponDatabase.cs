using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Items/Item Database/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField]
    private List<WeaponItem> allWeapons;

    public List<WeaponItem> AllWeapons => allWeapons;
}
