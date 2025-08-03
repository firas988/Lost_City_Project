using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Drakonit_Handler : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject effectPrefab;

    [SerializeField]
    private GameObject enemyPlaceHolder;

    [SerializeField]
    private GameObject crystal;

    private List<GameObject> enemies;

    private List<GameObject> effects;

    private Vector3 lastEnemyPosition;

    private bool isCrystalSpawned = false;

    private bool isEnemiesSpawned = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = new List<GameObject>();
        effects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnEnemy(10, 10f);
        }

        checkIfAllEnemiesAreDead();
    }


    private void spawnEnemy(int numberOfEnemiesToSpawn, float spawnRadius)
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
                GameObject cloneEnemy = Instantiate(
                    enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );
                cloneEnemy.GetComponent<DissolvingController>().setDissolveAmount();
                cloneEnemy.transform.SetParent(
                    enemyPlaceHolder.transform,
                    worldPositionStays: true
                );
                GameObject cloneEffect = Instantiate(
                    effectPrefab,
                    spawnPosition,
                    Quaternion.identity
                );
                cloneEffect.transform.SetParent(
                    enemyPlaceHolder.transform,
                    worldPositionStays: true
                );
                effects.Add(cloneEffect);
                cloneEnemy.GetComponent<EnemyMovement>().setCanMove(false);
                cloneEnemy.GetComponentInChildren<EnemyHealthBar>().hideHealthBar();
                enemies.Add(cloneEnemy);
            }
            else
            {
                Debug.LogWarning(
                    $"Enemy {i} could not find a valid spawn position on NavMesh after 10 attempts."
                );
            }
        }

        StartCoroutine(startSpawn());
    }

    private IEnumerator startSpawn()
    {
        yield return new WaitForSeconds(1f);
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<DissolvingController>().StartDeDissolve();
        }

        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < effects.Count; i++)
        {
            Destroy(effects[i]);
        }

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMovement>().setCanMove(true);
            enemy.GetComponentInChildren<EnemyHealthBar>().showHealthBar();
        }
        isEnemiesSpawned = true;
    }

     private void checkIfAllEnemiesAreDead()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
          Entity entity = enemies[i].GetComponent<StartNpc>().GetNpcsInstance() as Entity;
            if (entity != null && entity.isDead())

            {
                lastEnemyPosition = enemies[i].transform.position;
                enemies.RemoveAt(i);
            }
        }
        if ( enemies.Count == 0 && !isCrystalSpawned && isEnemiesSpawned)
        {
          Debug.Log("Crystal Spawned");
          Debug.Log(lastEnemyPosition);
          lastEnemyPosition.y += 1f;
          isCrystalSpawned = true;
          GameObject crystalClone = Instantiate(crystal, lastEnemyPosition, Quaternion.identity);
          crystalClone.transform.SetParent(enemyPlaceHolder.transform, worldPositionStays: true);
        }
    }
}
