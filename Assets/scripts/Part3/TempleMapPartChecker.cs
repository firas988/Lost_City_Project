using System.Collections;
using UnityEngine;

public class TempleMapPartChecker : MonoBehaviour
{
    private GameObject dungeonDoor;
    QuestManager questManager;
    private string gameManagerTag = "GameManager";

    void Start()
    {
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        dungeonDoor = GameObject.Find("dungeonEntrance");
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        if (questManager.checkingCompletedStoryQuest(typeof(TempleFindMapPart)))
        {
            dungeonDoor.GetComponent<DungeonDoorAnimateControl>().openBothDoors();
        }
    }
}
