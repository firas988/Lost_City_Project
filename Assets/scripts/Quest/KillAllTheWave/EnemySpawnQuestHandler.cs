using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySpawnQuestHandler : MonoBehaviour
{
    [SerializeField]
    private List<Enemyspawner> enemySpawners;

    [SerializeField]
    private HologramHandler hologramHandler;

    [SerializeField]
    private GameObject hologram;

    [SerializeField]
    private KillAllWaveMapColider killAllWaveMapColider;

    private Quest quest;
    private QuestManager questManager;

    private int numberOfWaves = 3;
    private int currentWave = 0;

    private bool isReadyToSpawn = false;

    private bool readyToCheckIfAllReadyToRespawn = true;

    private bool inTimer = false;

    private bool isQuestIsFinshAllTheWave = false;

    private bool isQuestIsCompleted = false;

    [SerializeField]
    private List<GameObject> canvasWave;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        setAllEnemySpawnersToNeedToRespawn(true);
        setTimerForRespawn();
        killAllWaveMapColider.subscribeToOnEnter(CompleteQuest);
        StartCoroutine(checkIfTheQuestIsCompletedLood());
    }

    void Update()
    {
        if (isQuestIsCompleted)
        {
            return;
        }
        if (!isQuestIsFinshAllTheWave)
        {
            checkThecurrentQuest();
        }
        if (quest == null || quest.isCompleted)
        {
            return;
        }

        checkIfTheQuestIsCompleted();
        if (!inTimer)
        {
            checkIfAllReadyToRespawn();
        }

        if (isReadyToSpawn && !inTimer && !quest.isCompleted)
        {
            isReadyToSpawn = false;
            currentWave++;
            updateCanvasWave();
            inTimer = true;
            StartCoroutine(spawnEnemies());
        }

        //test
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.killAllEnemies();
            }
        }
    }

    private void updateCanvasWave()
    {
        foreach (GameObject canvas in canvasWave)
        {
            canvas.GetComponentInChildren<TextMeshProUGUI>().text =
                "Wave " + (currentWave + 1) + "/" + numberOfWaves;
        }
    }

    private void checkIfTheQuestIsCompleted()
    {
        if (currentWave == numberOfWaves)
        {
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.setStopSpawn(true);
            }
            foreach (GameObject canvas in canvasWave)
            {
                canvas.SetActive(false);
            }
            hologram.SetActive(false);
        }
    }

    private void setTimerForRespawn()
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            enemySpawner.setTimerForRespawn(10f);
        }
    }

    private IEnumerator spawnEnemies()
    {
        readyToCheckIfAllReadyToRespawn = false;
        yield return new WaitForSeconds(1f);
        setAllEnemySpawnersToNeedToRespawn(true);
        yield return new WaitForSeconds(5f);
        readyToCheckIfAllReadyToRespawn = true;
        isReadyToSpawn = false;
        inTimer = false;
    }

    private void setAllEnemySpawnersToNeedToRespawn(bool isEnemyNeedSpawned)
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            enemySpawner.setIsEnemyNeedSpawned(isEnemyNeedSpawned);
            enemySpawner.setIsTheSpawnerActiveToSpawn(isEnemyNeedSpawned);
        }
    }

    private void checkIfAllReadyToRespawn()
    {
        if (!readyToCheckIfAllReadyToRespawn)
        {
            return;
        }
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            if (!enemySpawner.getIsReadyToRespawn())
            {
                isReadyToSpawn = false;
                // setAllEnemySpawnersToNeedToRespawn(false);
                return;
            }
        }
        isReadyToSpawn = true;
    }

    public void setQuest(Quest quest)
    {
        this.quest = quest;
        hologramHandler.setSphereHologramOut(false);
        hologramHandler.gameObject.SetActive(false);
    }

    public void checkThecurrentQuest()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest currentQuest = player.getCurrentMainQuest();
        if (currentQuest is FinshAllTheWave)
        {
            if (!isQuestIsFinshAllTheWave)
            {
                setQuest(currentQuest);
                isQuestIsFinshAllTheWave = true;
            }
        }
    }

    private void CompleteQuest()
    {
        (quest as FinshAllTheWave)?.CompleteQuest();
    }

    private bool checkIfTheQuestIsFinshAllTheWaveIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(FinshAllTheWave)))
        {
            Destroy(GameObject.FindWithTag("FinshAllTheWaveMapPiece"));
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.setStopSpawn(true);
                enemySpawner.destroyEnemies();
            }
            PlayableDirector director = GameObject
                .FindWithTag("GhostCutScene")
                .GetComponent<PlayableDirector>();
            director.time = director.duration;
            director.Evaluate();
            Destroy(hologram);
            foreach (GameObject canvas in canvasWave)
            {
                Destroy(canvas);
            }
            return true;
        }
        return false;
    }

    private IEnumerator checkIfTheQuestIsCompletedLood()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);
        if (checkIfTheQuestIsFinshAllTheWaveIsCompleted())
        {
            isQuestIsCompleted = true;
        }
    }
}
