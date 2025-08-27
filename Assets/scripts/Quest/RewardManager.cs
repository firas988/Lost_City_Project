using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField]
    private ItemDatabase itemDatabase;
    private List<Item> allItems;

    private InventoryManager inventoryManager;

    private LevelManager levelManager;

    private NotificationsManager notificationsManager;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        allItems = itemDatabase.AllItems;
        inventoryManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InventoryManager>();
        levelManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<LevelManager>();
        notificationsManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<NotificationsManager>();
    }

    public void GiveReward(int itemId)
    {
        Item item = allItems.Find(item => item.id == itemId);
        if (item != null)
        {
            inventoryManager.AddItemToInventory(item);
            notificationsManager.queueTopLeftNotification(
                "You have received " + item.name,
                "notification"
            );
        }
    }

    public void GiveExpReward(int expReward)
    {
        levelManager.addXP(expReward);
        notificationsManager.queueTopLeftNotification(
            "You have received " + expReward + " XP",
            "notification"
        );
    }
}
