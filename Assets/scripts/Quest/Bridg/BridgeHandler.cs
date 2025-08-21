using System.Collections;
using UnityEngine;

public class BridgeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject Hologram;

    private QuestManager questManager;

    private string gameManagerTag = "GameManager";
    private bool isQuestIsCompleted = false;

    void Start()
    {
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    void Update()
    {
        if (isQuestIsCompleted)
        {
            return;
        }
        checkIfTheQuestIsGoToBridge();
    }

    private void checkIfTheQuestIsGoToBridge()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();
        if (quest is GoToBridge)
        {
            Destroy(Hologram);
        }
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);
        checkIfTheQuestIsGoToBridgeIsCompleted();
    }

    private void checkIfTheQuestIsGoToBridgeIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToBridge)))
        {
            Destroy(Hologram);

            isQuestIsCompleted = true;
        }
    }
}
