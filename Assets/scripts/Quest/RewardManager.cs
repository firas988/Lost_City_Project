using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the distribution of rewards to players, including items and experience points.
/// Handles inventory management and level progression through rewards.
/// Coordinates reward distribution across different reward types and systems.
/// </summary>
public class RewardManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Item Database")]
    /// <summary>
    /// Reference to the item database containing all available items.
    /// Used to look up items by ID for reward distribution.
    /// </summary>
    [SerializeField]
    private ItemDatabase itemDatabase;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// List of all available items from the item database.
    /// Cached for efficient reward lookup and distribution.
    /// </summary>
    private List<Item> allItems;

    /// <summary>
    /// Reference to the inventory manager for adding items to player inventory.
    /// Used when distributing item rewards.
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// Reference to the level manager for adding experience points.
    /// Used when distributing XP rewards.
    /// </summary>
    private LevelManager levelManager;

    /// <summary>
    /// Reference to the notifications manager for displaying reward messages.
    /// Used to inform players about received rewards.
    /// </summary>
    private NotificationsManager notificationsManager;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the reward manager by setting up references to required components.
    /// Caches item database and finds system managers for reward distribution.
    /// </summary>
    void Start()
    {
        // Cache all items from the database for efficient lookup
        allItems = itemDatabase.AllItems;

        // Find and store references to required system managers
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
    #endregion

    #region Reward Distribution Methods
    /// <summary>
    /// Gives an item reward to the player based on the provided item ID.
    /// Adds the item to inventory and displays a notification.
    /// </summary>
    /// <param name="itemId">The ID of the item to give as a reward.</param>
    public void GiveReward(int itemId)
    {
        // Find the item in the database by ID
        Item item = allItems.Find(item => item.id == itemId);

        if (item != null)
        {
            // Add item to player inventory
            inventoryManager.AddItemToInventory(item);

            // Display notification about received item
            notificationsManager.queueTopLeftNotification(
                "You have received " + item.name,
                "notification"
            );
        }
    }

    /// <summary>
    /// Gives experience points as a reward to the player.
    /// Adds XP to player level and displays a notification.
    /// </summary>
    /// <param name="expReward">The amount of experience points to award.</param>
    public void GiveExpReward(int expReward)
    {
        // Add experience points to player level if level manager exists
        if (levelManager != null)
        {
            levelManager.addXP(expReward);
        }

        // Display notification about received XP
        notificationsManager.queueTopLeftNotification(
            "You have received " + expReward + " XP",
            "notification"
        );
    }
    #endregion
}
