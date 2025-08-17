using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);
        inventoryManager = gameManager.GetComponentInChildren<InventoryManager>();

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
    }

    public void LoadGame()
    {
        inventoryManager.LoadInventory(SaveSystem.LoadInventory());
    }
}
