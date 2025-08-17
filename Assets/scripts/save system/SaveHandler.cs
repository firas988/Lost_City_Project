using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private StatisticsHandler statisticsHandler;

    private StartPlayer startPlayer;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);
        inventoryManager = gameManager.GetComponentInChildren<InventoryManager>();
        statisticsHandler = GameObject.FindAnyObjectByType<StatisticsHandler>();
        startPlayer = GameObject.FindAnyObjectByType<StartPlayer>();
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
    }

    public void LoadGame()
    {
        inventoryManager.LoadInventory(SaveSystem.LoadInventory());
        statisticsHandler.LoadStatistics(SaveSystem.LoadStatistics());
        startPlayer.loadPlayer(SaveSystem.LoadPlayer());
    }
}
