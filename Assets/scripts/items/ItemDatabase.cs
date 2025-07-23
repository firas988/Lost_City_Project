using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField]
    private List<Item> allItems;

    public List<Item> AllItems => allItems;

    public Item GetItem(int id)
    {
        return allItems.Find(item => item.id == id);
    }
}
