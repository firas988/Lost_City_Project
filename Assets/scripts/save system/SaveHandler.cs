using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private StatisticsHandler statisticsHandler;

    private StartPlayer startPlayer;

    private QuestManager questManager;
    private SkillTreeManager skillTreeManager;
    private LevelManager levelManager;

    private string gameManagerTag = "GameManager";

    private string playerTag = "Player";

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        inventoryManager = gameManager.GetComponentInChildren<InventoryManager>();
        skillTreeManager = gameManager.GetComponentInChildren<SkillTreeManager>();
        questManager = gameManager.GetComponentInChildren<QuestManager>();
        levelManager = gameManager.GetComponentInChildren<LevelManager>();
        statisticsHandler = player.GetComponentInChildren<StatisticsHandler>();
        startPlayer = player.GetComponentInChildren<StartPlayer>();
        LoadGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveInventory(inventoryManager.getInventory());
        SaveSystem.SaveStatistics(statisticsHandler.GetStatisticsHandler());
        SaveSystem.SavePlayer(startPlayer);
        SaveSystem.SaveQuest(questManager);
        SaveSystem.SaveSkills(skillTreeManager);
        SaveSystem.SaveLevel(levelManager);
    }

    public void LoadGame()
    {
        inventoryManager.LoadInventory(SaveSystem.LoadInventory());
        statisticsHandler.LoadStatistics(SaveSystem.LoadStatistics());
        startPlayer.loadPlayer(SaveSystem.LoadPlayer());
        questManager.initQuestLists(SaveSystem.LoadQuest());
        skillTreeManager.LoadSkills(SaveSystem.LoadSkills());
        levelManager.LoadLevel(SaveSystem.LoadLevel());
    }
}
