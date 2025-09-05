using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates the save and load operations for all game systems.
/// Manages references to various managers and coordinates with the SaveSystem for data persistence.
/// Provides centralized save/load functionality accessible through input or external calls.
/// </summary>
public class SaveHandler : MonoBehaviour
{
    #region Manager References
    /// <summary>
    /// Reference to InventoryManager for saving/loading inventory data.
    /// Manages player item storage and equipment state persistence.
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// Reference to StatisticsHandler for saving/loading player statistics.
    /// Manages gameplay metrics and progress tracking persistence.
    /// </summary>
    private StatisticsHandler statisticsHandler;

    /// <summary>
    /// Reference to StartPlayer for saving/loading player position and state.
    /// Manages player transform and cutscene completion persistence.
    /// </summary>
    private StartPlayer startPlayer;

    /// <summary>
    /// Reference to QuestManager for saving/loading quest progress.
    /// Manages story progression and quest completion state persistence.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to SkillTreeManager for saving/loading skill tree progress.
    /// Manages player skill development and progression persistence.
    /// </summary>
    private SkillTreeManager skillTreeManager;

    /// <summary>
    /// Reference to LevelManager for saving/loading level data.
    /// Manages player level and experience persistence.
    /// </summary>
    private LevelManager levelManager;
    #endregion

    #region System References
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Tag used to find the Player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Interval at which the game will automatically save.
    /// </summary>
    private float autoSaveInterval = 300f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the save handler by finding required manager references.
    /// Sets up connections to all systems that need save/load functionality.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(1)
    void Awake()
    {
        // Find GameManager and Player GameObjects
        GameObject gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        // Get references to all manager components from GameManager
        inventoryManager = gameManager.GetComponentInChildren<InventoryManager>();
        skillTreeManager = gameManager.GetComponentInChildren<SkillTreeManager>();
        questManager = gameManager.GetComponentInChildren<QuestManager>();
        levelManager = gameManager.GetComponentInChildren<LevelManager>();

        // Get references to player-specific components
        statisticsHandler = player.GetComponentInChildren<StatisticsHandler>();
        startPlayer = player.GetComponentInChildren<StartPlayer>();

        // Load saved game data on startup
        LoadGame();
        StartCoroutine(AutoSave()); // Start auto save
    }

    #endregion

    #region Save and Load Operations
    /// <summary>
    /// Saves all game data to persistent storage.
    /// Coordinates saving of inventory, statistics, player, quests, skills, and level data.
    /// </summary>
    // COMPLEXITY ANALYSIS: SaveGame() - O(1)
    public void SaveGame()
    {
        // Save all game systems through the SaveSystem
        SaveSystem.SaveInventory(inventoryManager.getInventory());
        SaveSystem.SaveStatistics(statisticsHandler.GetStatisticsHandler());
        SaveSystem.SavePlayer(startPlayer);
        SaveSystem.SaveQuest(questManager);
        SaveSystem.SaveSkills(skillTreeManager);
        SaveSystem.SaveLevel(levelManager);
    }

    /// <summary>
    /// Loads all game data from persistent storage.
    /// Restores the state of all game systems to their saved values.
    /// </summary>
    // COMPLEXITY ANALYSIS: LoadGame() - O(1)
    public void LoadGame()
    {
        // Load all game systems through the SaveSystem
        inventoryManager.LoadInventory(SaveSystem.LoadInventory());
        statisticsHandler.LoadStatistics(SaveSystem.LoadStatistics());
        startPlayer.loadPlayer(SaveSystem.LoadPlayer());
        questManager.initQuestLists(SaveSystem.LoadQuest());
        skillTreeManager.LoadSkills(SaveSystem.LoadSkills());
        levelManager.LoadLevel(SaveSystem.LoadLevel());
    }

    // COMPLEXITY ANALYSIS: AutoSave() - O(1)
    IEnumerator AutoSave()
    {
        yield return new WaitForSeconds(autoSaveInterval);
        SaveGame();
        StartCoroutine(AutoSave());
    }
    #endregion
}
