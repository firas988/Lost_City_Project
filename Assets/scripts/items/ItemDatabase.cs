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

    public Item GetRandomItem(ItemCategory? itemCategory = null)
    {
        if (itemCategory == null)
        {
            return allItems[Random.Range(0, allItems.Count)];
        }
        else
        {
            List<Item> items = allItems.FindAll(item => item.category == itemCategory);
            return items[Random.Range(0, items.Count)];
        }
    }
}
