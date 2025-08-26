using System.Collections;
using UnityEngine;

public class DungeonQuestsChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject enterCutScene;
    private QuestManager questManager;

    void Awake()
    {
        questManager = GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponentInChildren<QuestManager>();

        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        Debug.Log(
            "DungeonQuestsChecker"
                + questManager.checkingCompletedStoryQuest(typeof(DungeonLevelFinal))
        );

        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevelFinal)))
        {
            GameObject
                .FindGameObjectWithTag("GameManager")
                .GetComponentInChildren<SceneHandler>()
                .LoadScene(4);
        }

        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevel1)))
        {
            enterCutScene.SetActive(false);
            GetComponent<DungeonManager>().NextRoom();
        }

        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevel2)))
        {
            enterCutScene.SetActive(false);
            GetComponent<DungeonManager>().NextRoom();
        }
    }
}
