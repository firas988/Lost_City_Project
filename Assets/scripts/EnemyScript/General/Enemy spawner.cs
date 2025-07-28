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

    private GameObject chest;

    private List<Entity> entities;

    private int randomDifficulty;

    private int numberOfEnemiesToSpawn;

    private int enemyCount;

    private float spawnRadius;

    [SerializeField]
    private bool canMultipleRespawn = false;

    private bool inTimer = false;

    void Start()
    {
        entities = new List<Entity>();
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        spawnRadius = patrolZoneTrigger.getPatrolRange() - 5f;
        putChestInPlaceHolder();
        spawnEnemies();
    }

    void Update()
    {
        if (canMultipleRespawn && chest.GetComponent<ObjectInteraction>().getIsOpen() && !inTimer)
        {
            StartCoroutine(respawnTimer());
        }

        checkIfAllEnemiesAreDead();
    }

    private void checkIfAllEnemiesAreDead()
    {
        foreach (Entity entity in entities)
        {
            if (entity.isDead())
            {
                entities.Remove(entity);
                enemyCount--;
            }
        }
        if (enemyCount == 0 && entities.Count == 0)
        {
            chest.GetComponent<ObjectInteraction>().setCanOpen(true);
        }
    }

    private IEnumerator respawnTimer()
    {
        inTimer = true;
        yield return new WaitForSeconds(120f);
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        putChestInPlaceHolder();
        spawnEnemies();
        inTimer = false;
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
        randomDifficulty = Random.Range(0, 4);
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
                    Debug.Log(
                        $"Enemy {i} spawn position found at attempt {attempt}: {spawnPosition}"
                    );
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
            }
            else
            {
                Debug.LogWarning(
                    $"Enemy {i} could not find a valid spawn position on NavMesh after 10 attempts."
                );
            }
        }
    }
}
