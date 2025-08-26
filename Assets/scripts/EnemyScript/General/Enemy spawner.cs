using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyspawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemies;

    [SerializeField]
    private List<GameObject> chests;

    [SerializeField]
    private PatrolZone_Trigger patrolZoneTrigger;

    [SerializeField]
    private GameObject chestPlaceHolder;

    [SerializeField]
    private GameObject enemyPlaceHolder;

    [SerializeField]
    private SphereCollider colliderPlayerRange;

    private GameObject chest;

    private List<Entity> entities;

    private List<GameObject> enemiesToObject;

    private int randomDifficulty;

    private int numberOfEnemiesToSpawn;

    private int enemyCount;

    private float spawnRadius;

    [SerializeField]
    private bool canMultipleRespawn = false;

    private bool inTimer = false;

    private bool allEnemiesDead = true;

    [SerializeField]
    private bool canGetRandomDifficulty = true;

    [SerializeField]
    private int difficulty = 0;

    private bool isReadyToRespawn = false;

    private float timerForRespawn = 120f;

    private bool isEnemyNeedSpawned = true;

    private bool isPlayerInRange = false;

    private bool isTheSpawnerActiveToSpawn = true;

    private bool stopSpawn = false;

    [SerializeField]
    private float extraRadius = 0f;

    void Start()
    {
        enemiesToObject = new List<GameObject>();
        entities = new List<Entity>();
        spawnRadius = patrolZoneTrigger.getPatrolRange() - 5f;
        if (colliderPlayerRange != null)
        {
            colliderPlayerRange.radius = patrolZoneTrigger.getPatrolRange() + 170f + extraRadius;
        }
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        putChestInPlaceHolder();
    }

    void Update()
    {
        if (stopSpawn)
        {
            return;
        }

        if (
            isPlayerInRange
            && (canMultipleRespawn || isEnemyNeedSpawned)
            && !inTimer
            && isTheSpawnerActiveToSpawn
        )
        {
            if (allEnemiesDead)
            {
                isReadyToRespawn = false;
                isTheSpawnerActiveToSpawn = false;
                isEnemyNeedSpawned = false;
                SpawnHandler();
                allEnemiesDead = false;
            }
        }
        else if (!allEnemiesDead && !isPlayerInRange && !isReadyToRespawn)
        {
            isEnemyNeedSpawned = true;
            isTheSpawnerActiveToSpawn = true;
            destroyEnemies();
        }

        readyToRespawn();
        if (canMultipleRespawn && isReadyToRespawn && !inTimer)
        {
            inTimer = true;
            StartCoroutine(respawnTimer());
        }

        checkIfAllEnemiesAreDead();
    }

    private void readyToRespawn()
    {
        if (chest.GetComponent<ObjectInteraction>().getIsOpen())
        {
            isReadyToRespawn = true;
        }
        else
        {
            isReadyToRespawn = false;
        }
    }

    private void SpawnHandler()
    {
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        putChestInPlaceHolder();
        spawnEnemies();
    }

    private void checkIfAllEnemiesAreDead()
    {
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            if (entities[i].isDead())
            {
                entities.RemoveAt(i);
                enemyCount--;
            }
        }
        if (enemyCount == 0 && entities.Count == 0)
        {
            enemiesToObject.Clear();

            allEnemiesDead = true;
            chest.GetComponent<ObjectInteraction>().setCanOpen(true);
        }
    }

    private IEnumerator respawnTimer()
    {
        yield return new WaitForSeconds(timerForRespawn);
        SpawnHandler();
        allEnemiesDead = false;
        inTimer = false;
        isEnemyNeedSpawned = true;
        isTheSpawnerActiveToSpawn = true;
    }

    private void getNumberOfEnemiesToSpawn()
    {
        switch (randomDifficulty)
        {
            case 0:
                numberOfEnemiesToSpawn = Random.Range(1, 3);
                break;
            case 1:
                numberOfEnemiesToSpawn = Random.Range(3, 5);
                break;
            case 2:
                numberOfEnemiesToSpawn = Random.Range(5, 7);
                break;
            case 3:
                numberOfEnemiesToSpawn = Random.Range(7, 9);
                break;
        }
    }

    private void getRandomDifficulty()
    {
        if (canGetRandomDifficulty)
        {
            randomDifficulty = Random.Range(0, 4);
        }
        else
        {
            randomDifficulty = difficulty;
        }
    }

    private void putChestInPlaceHolder()
    {
        if (chest != null)
        {
            Destroy(chest);
        }
        chest = Instantiate(
            chests[randomDifficulty],
            chestPlaceHolder.transform.position,
            Quaternion.identity
        );
        chest.transform.SetParent(chestPlaceHolder.transform, worldPositionStays: true);
        chest.GetComponent<ObjectInteraction>().setCanOpen(false);
    }

    private void spawnEnemies()
    {
        Vector3 center = enemyPlaceHolder.transform.position;

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            bool foundSpot = false;
            Vector3 spawnPosition = Vector3.zero;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector3 randomPos = center + Random.insideUnitSphere * spawnRadius;
                randomPos.y = center.y + 1f;

                if (
                    UnityEngine.AI.NavMesh.SamplePosition(
                        randomPos,
                        out UnityEngine.AI.NavMeshHit hit,
                        5f,
                        UnityEngine.AI.NavMesh.AllAreas
                    )
                )
                {
                    spawnPosition = hit.position;
                    foundSpot = true;
                    break;
                }
            }

            if (foundSpot)
            {
                GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Count)];
                GameObject cloneEnemy = Instantiate(
                    enemyToSpawn,
                    spawnPosition,
                    Quaternion.identity
                );
                cloneEnemy.transform.SetParent(
                    enemyPlaceHolder.transform,
                    worldPositionStays: true
                );
                entities.Add((Entity)cloneEnemy.GetComponent<StartNpc>().GetNpcsInstance());
                enemyCount++;
                enemiesToObject.Add(cloneEnemy);
            }
            else
            {
                Debug.LogWarning(
                    $"Enemy {i} could not find a valid spawn position on NavMesh after 10 attempts."
                );
            }
        }
    }

    public void destroyEnemies()
    {
        foreach (GameObject enemy in enemiesToObject)
        {
            Destroy(enemy);
        }
        enemiesToObject.Clear();
        allEnemiesDead = true;
        // Destroy(enemyPlaceHolder);
        chest.GetComponent<ObjectInteraction>().setCanOpen(false);
        isEnemyNeedSpawned = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    public void killAllEnemies()
    {
        foreach (Entity entity in entities)
        {
            entity.setHealth(0);
        }
        enemiesToObject.Clear();
    }

    public bool getAllEnemiesDead()
    {
        return allEnemiesDead;
    }

    public void setCanMultipleRespawn(bool canMultipleRespawn)
    {
        this.canMultipleRespawn = canMultipleRespawn;
    }

    public bool getIsReadyToRespawn()
    {
        return isReadyToRespawn;
    }

    public void setIsEnemyNeedSpawned(bool isEnemyNeedSpawned)
    {
        this.isEnemyNeedSpawned = isEnemyNeedSpawned;
    }

    public void setIsTheSpawnerActiveToSpawn(bool isTheSpawnerActiveToSpawn)
    {
        this.isTheSpawnerActiveToSpawn = isTheSpawnerActiveToSpawn;
    }

    public void setTimerForRespawn(float timerForRespawn)
    {
        this.timerForRespawn = timerForRespawn;
    }

    public void setStopSpawn(bool stopSpawn)
    {
        this.stopSpawn = stopSpawn;
    }
}
