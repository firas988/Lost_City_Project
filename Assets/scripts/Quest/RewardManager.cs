using UnityEngine;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    [SerializeField]
    private ItemDatabase itemDatabase;
    private List<Item> allItems;

    private InventoryManager inventoryManager;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        allItems = itemDatabase.AllItems;
        inventoryManager = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponentInChildren<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GiveReward(int itemId)
    {
        Item item = allItems.Find(item => item.id == itemId);
        if (item != null)
        {
            inventoryManager.AddItemToInventory(item);
        }
    }
}
