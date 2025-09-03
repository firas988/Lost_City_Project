using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning and management of Drakonit enemies and their effects during boss fights
/// </summary>
public class Spawn_Drakonit_Handler : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Prefab of the enemy to spawn
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;

    /// <summary>
    /// Visual effect prefab to spawn with enemies
    /// </summary>
    [SerializeField]
    private GameObject effectPrefab;

    /// <summary>
    /// Parent GameObject to organize spawned enemies and effects
    /// </summary>
    [SerializeField]
    private GameObject enemyPlaceHolder;

    /// <summary>
    /// Crystal prefab to spawn after all enemies are defeated
    /// </summary>
    [SerializeField]
    private GameObject crystal;
    #endregion

    #region Private Fields
    /// <summary>
    /// List of all spawned enemy GameObjects
    /// </summary>
    private List<GameObject> enemies;

    /// <summary>
    /// List of all spawned effect GameObjects
    /// </summary>
    private List<GameObject> effects;

    /// <summary>
    /// Position of the last defeated enemy for crystal spawning
    /// </summary>
    private Vector3 lastEnemyPosition;

    /// <summary>
    /// Whether the crystal has been spawned
    /// </summary>
    private bool isCrystalSpawned = false;

    /// <summary>
    /// Whether enemies have been spawned and are active
    /// </summary>
    private bool isEnemiesSpawned = false;
    #endregion

    #region Unity Lifecycle
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize lists to store spawned objects
        enemies = new List<GameObject>();
        effects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if all enemies are defeated to potentially spawn crystal
        checkIfAllEnemiesAreDead();
    }
    #endregion

    #region Public Getters
    /// <summary>
    /// Get whether the crystal has been spawned
    /// </summary>
    /// <returns>True if crystal is spawned, false otherwise</returns>
    public bool getIsCrystalSpawned()
    {
        return isCrystalSpawned;
    }

    /// <summary>
    /// Get whether enemies have been spawned and are active
    /// </summary>
    /// <returns>True if enemies are spawned, false otherwise</returns>
    public bool getIsEnemiesSpawned()
    {
        return isEnemiesSpawned;
    }
    #endregion

    #region Enemy Spawning
    /// <summary>
    /// Start spawning enemies at specified locations
    /// </summary>
    /// <param name="numberOfEnemiesToSpawn">Number of enemies to spawn</param>
    /// <param name="spawnRadius">Radius around center point to spawn enemies</param>
    public void startSpawnEnemies(int numberOfEnemiesToSpawn, float spawnRadius)
    {
        spawnEnemy(numberOfEnemiesToSpawn, spawnRadius);
    }

    /// <summary>
    /// Spawn enemies at random positions within the specified radius
    /// </summary>
    /// <param name="numberOfEnemiesToSpawn">Number of enemies to spawn</param>
    /// <param name="spawnRadius">Radius around center point to spawn enemies</param>
    private void spawnEnemy(int numberOfEnemiesToSpawn, float spawnRadius)
    {
        // Use the placeholder position as the center for spawning
        Vector3 center = enemyPlaceHolder.transform.position;

        // Spawn each enemy
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            Debug.Log("spawnEnemy " + i);
            bool foundSpot = false;
            Vector3 spawnPosition = Vector3.zero;

            // Try to find a valid spawn position on NavMesh
            for (int attempt = 0; attempt < 10; attempt++)
            {
                // Generate random position within spawn radius
                Vector3 randomPos = center + Random.insideUnitSphere * spawnRadius;
                randomPos.y = center.y + 1f;

                // Check if position is valid on NavMesh
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
                // Spawn enemy at valid position
                GameObject cloneEnemy = Instantiate(
                    enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                // Set dissolve effect for spawn animation
                cloneEnemy.GetComponent<DissolvingController>().setDissolveAmount();

                // Parent enemy to placeholder for organization
                cloneEnemy.transform.SetParent(
                    enemyPlaceHolder.transform,
                    worldPositionStays: true
                );

                // Spawn visual effect at enemy position
                // GameObject cloneEffect = Instantiate(
                //     effectPrefab,
                //     spawnPosition,
                //     Quaternion.identity
                // );

                // Parent effect to placeholder
                // cloneEffect.transform.SetParent(
                //     enemyPlaceHolder.transform,
                //     worldPositionStays: true
                // );

                // Store effect reference and disable enemy movement initially
                // effects.Add(cloneEffect);
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

        // Start the spawn sequence coroutine
        StartCoroutine(startSpawn());
    }
    #endregion

    #region Spawn Sequence
    /// <summary>
    /// Coroutine that handles the spawn sequence timing and activation
    /// </summary>
    private IEnumerator startSpawn()
    {
        // Wait before starting dissolve-in animation
        yield return new WaitForSeconds(1f);

        // Start dissolve-in animation for all enemies
        foreach (GameObject enemy in enemies)
        {
            Debug.Log("startSpawn " + enemy.name);
            enemy.GetComponent<DissolvingController>().StartDeDissolve();
        }

        // Wait for dissolve animation to complete
        yield return new WaitForSeconds(2.5f);

        // Clean up spawn effects
        for (int i = 0; i < effects.Count; i++)
        {
            Destroy(effects[i]);
        }

        // Enable enemy movement and show health bars
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMovement>().setCanMove(true);
            enemy.GetComponentInChildren<EnemyHealthBar>().showHealthBar();
        }

        // Mark enemies as fully spawned and active
        isEnemiesSpawned = true;
    }
    #endregion

    #region Enemy Management
    /// <summary>
    /// Check if all enemies are defeated and spawn crystal if needed
    /// </summary>
    private void checkIfAllEnemiesAreDead()
    {
        // Iterate backwards through enemy list to safely remove dead enemies
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            // Get the entity component to check if enemy is dead
            Entity entity = enemies[i].GetComponent<StartNpc>().GetNpcsInstance() as Entity;
            if (entity != null && entity.isDead())
            {
                // Store position of last defeated enemy for crystal spawning
                lastEnemyPosition = enemies[i].transform.position;
                enemies.RemoveAt(i);
            }
        }

        // If all enemies are defeated and crystal hasn't been spawned yet
        if (enemies.Count == 0 && !isCrystalSpawned && isEnemiesSpawned)
        {
            // Reset enemy spawn state and prepare crystal spawn
            isEnemiesSpawned = false;
            lastEnemyPosition.y += 1f;
            isCrystalSpawned = true;

            // Spawn crystal at last enemy position
            GameObject crystalClone = Instantiate(crystal, lastEnemyPosition, Quaternion.identity);
            crystalClone.transform.SetParent(enemyPlaceHolder.transform, worldPositionStays: true);

            // Subscribe to crystal removal event
            crystalClone.GetComponent<DashToTarget>().subscribeToCrystal(removeCrystal);
        }
    }

    /// <summary>
    /// Reset crystal spawn state when crystal is removed
    /// </summary>
    public void removeCrystal()
    {
        isCrystalSpawned = false;
    }

    /// <summary>
    /// Set the placeholder GameObject for organizing spawned enemies
    /// </summary>
    /// <param name="enemiesPlaceHolder">GameObject to use as parent for enemies</param>
    public void setEnemiesPlaceHolder(GameObject enemiesPlaceHolder)
    {
        enemyPlaceHolder = enemiesPlaceHolder;
    }

    /// <summary>
    /// Force kill all spawned enemies and reset spawn states
    /// </summary>
    public void killAllEnemies()
    {
        // Destroy all enemy GameObjects
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Destroy(enemies[i]);
        }

        // Clear lists and reset spawn states
        enemies.Clear();
        isEnemiesSpawned = false;
        isCrystalSpawned = false;
    }
    #endregion
}
