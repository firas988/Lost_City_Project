using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private int numberOfWaves = 3;
    private int currentWave = 0;

    private bool isReadyToSpawn = false;

    private bool readyToCheckIfAllReadyToRespawn = true;

    private bool inTimer = false;

    private bool isQuestIsFinshAllTheWave = false;

    [SerializeField]
    private List<GameObject> canvasWave;

    void Start()
    {
        setAllEnemySpawnersToCanMultipleRespawn(false);
        setTimerForRespawn();
        killAllWaveMapColider.subscribeToOnEnter(CompleteQuest);
    }

    void Update()
    {
        if (!isQuestIsFinshAllTheWave)
        {
            checkThecurrentQuest();
        }
        if (quest == null || quest.isCompleted)
        {
            return;
        }

        checkIfTheQuestIsCompleted();

        checkIfAllReadyToRespawn();

        if (isReadyToSpawn && !inTimer && !quest.isCompleted)
        {
            currentWave++;
            updateCanvasWave();
            inTimer = true;
            StartCoroutine(spawnEnemies());
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
        setAllEnemySpawnersToCanMultipleRespawn(true);
        yield return new WaitForSeconds(5f);
        readyToCheckIfAllReadyToRespawn = true;
        isReadyToSpawn = false;
        inTimer = false;
    }

    private void setAllEnemySpawnersToCanMultipleRespawn(bool canMultipleRespawn)
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            enemySpawner.setCanMultipleRespawn(canMultipleRespawn);
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
                setAllEnemySpawnersToCanMultipleRespawn(false);
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
        (quest as FinshAllTheWave).CompleteQuest();
    }
}
