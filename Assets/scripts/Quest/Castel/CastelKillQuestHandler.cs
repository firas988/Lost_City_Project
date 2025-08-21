using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastelKillQuestHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> outDoor;

    [SerializeField]
    private GameObject inDoor;

    [SerializeField]
    private List<Enemyspawner> enemySpawners;

    [SerializeField]
    private GameObject mapParts;

    private Quest currentQuest;
    private QuestManager questManager;

    private bool isQuestIsGoToCastel = false;

    private bool isQuestIsKillAllTheEnemyInTheCastel = false;

    private bool isAllSpawnersAreDone = false;

    private bool isQuestIsFindTheMapPart = false;

    private bool isQuestIsCompleted = false;

    private string gameManagerTag = "GameManager";

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
        checkIfTheQuestIsGotToTheCastel();
        checkIfTheQuestIsKillAllTheEnemyInTheCastel();
        checkIfTheQuestIsFindTheMapPart();
        if (!isAllSpawnersAreDone)
        {
            checkIfAllSpawnersAreDone();
        }
        else
        {
            TryCompleteTheKillAllTheEnemyInTheCastelQuest();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.killAllEnemies();
            }
        }
    }

    private void TryCompleteTheFindTheMapPartQuest()
    {
        if (currentQuest is FindTheMapPart && isQuestIsFindTheMapPart)
        {
            (currentQuest as FindTheMapPart).CompleteQuest();
            Destroy(mapParts);
        }
    }

    private void checkIfAllSpawnersAreDone()
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            if (!enemySpawner.getIsReadyToRespawn())
            {
                isAllSpawnersAreDone = false;
                return;
            }
        }
        isAllSpawnersAreDone = true;
    }

    private void openTheOutDoor()
    {
        foreach (GameObject outDoor in outDoor)
        {
            outDoor.GetComponent<CastelDoorHandler>().openTheDoor();
        }
    }

    private void TryCompleteTheGoToCastelQuest()
    {
        if (currentQuest is GoToCastel && isQuestIsGoToCastel)
        {
            (currentQuest as GoToCastel).CompleteQuest();
            isQuestIsGoToCastel = false;
        }
    }

    private void TryCompleteTheKillAllTheEnemyInTheCastelQuest()
    {
        if (currentQuest is KillAllTheEnemyInTheCastel && isQuestIsKillAllTheEnemyInTheCastel)
        {
            (currentQuest as KillAllTheEnemyInTheCastel).CompleteQuest();
            inDoor.GetComponent<CastelDoorHandler>().openTheDoor();
            isQuestIsKillAllTheEnemyInTheCastel = false;
        }
    }

    private void subscribeToTheQuest()
    {
        foreach (GameObject outDoor in outDoor)
        {
            outDoor
                .GetComponent<CastelDoorHandler>()
                .subscribeToOnTriggerEnter(TryCompleteTheGoToCastelQuest);
        }
    }

    private void checkIfTheQuestIsGotToTheCastel()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();
        if (quest is GoToCastel)
        {
            if (!isQuestIsGoToCastel)
            {
                currentQuest = quest;
                isQuestIsGoToCastel = true;
                openTheOutDoor();
                subscribeToTheQuest();
            }
        }
    }

    private void checkIfTheQuestIsKillAllTheEnemyInTheCastel()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();
        if (quest is KillAllTheEnemyInTheCastel)
        {
            if (!isQuestIsKillAllTheEnemyInTheCastel)
            {
                currentQuest = quest;
                isQuestIsKillAllTheEnemyInTheCastel = true;
            }
        }
    }

    private void checkIfTheQuestIsFindTheMapPart()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();
        if (quest is FindTheMapPart)
        {
            if (!isQuestIsFindTheMapPart)
            {
                currentQuest = quest;
                isQuestIsFindTheMapPart = true;
                mapParts
                    .GetComponent<MapColiderHandler>()
                    .subscribeToOnTriggerEnter(TryCompleteTheFindTheMapPartQuest);
            }
        }
    }

    private void CheckIfGoToCastelIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToCastel)))
        {
            isQuestIsGoToCastel = true;
            openTheOutDoor();
            subscribeToTheQuest();
        }
    }

    private void CheckIfKillAllTheEnemyInTheCastelIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(KillAllTheEnemyInTheCastel)))
        {
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.destroyEnemies();
            }
            inDoor.GetComponent<CastelDoorHandler>().openTheDoor();
        }
    }

    private void CheckIfFindTheMapPartIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(FindTheMapPart)))
        {
            Destroy(mapParts);
            isQuestIsCompleted = true;
        }
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);
        CheckIfGoToCastelIsCompleted();
        CheckIfKillAllTheEnemyInTheCastelIsCompleted();
        CheckIfFindTheMapPartIsCompleted();
    }
}
