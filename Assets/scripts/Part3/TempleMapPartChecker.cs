using UnityEngine;

public class TempleMapPartChecker : MonoBehaviour
{
    private GameObject dungeonDoor;
    QuestManager questManager;
    private bool isCompleted;
    private string gameManagerTag = "GameManager";

    void Start()
    {
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        isCompleted = false;
        dungeonDoor = GameObject.Find("dungeonEntrance");
    }

    // Update is called once per frame
    void Update()
    {
        if (isCompleted)
        {
            return;
        }

        if (questManager.checkingCompletedStoryQuest(typeof(TempleFindMapPart)))
        {
            isCompleted = true;
            dungeonDoor.GetComponent<DungeonDoorAnimateControl>().openBothDoors();
        }
    }
}
