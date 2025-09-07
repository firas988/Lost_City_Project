using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning and respawning of enemies and chests in patrol zones.
/// Handles difficulty scaling, enemy count management, and player proximity detection.
/// Integrates with PatrolZone_Trigger for spawn area management.
/// </summary>
public class Enemyspawner : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>List of enemy prefabs available for spawning.</summary>
    [SerializeField]
    private List<GameObject> enemies;

    /// <summary>List of chest prefabs available for spawning based on difficulty.</summary>
    [SerializeField]
    private List<GameObject> chests;

    /// <summary>Reference to the patrol zone trigger for spawn area management.</summary>
    [SerializeField]
    private PatrolZone_Trigger patrolZoneTrigger;

    /// <summary>GameObject marking the position where chests should be spawned.</summary>
    [SerializeField]
    private GameObject chestPlaceHolder;

    /// <summary>GameObject marking the position where enemies should be spawned.</summary>
    [SerializeField]
    private GameObject enemyPlaceHolder;

    /// <summary>Sphere collider that detects when player enters spawn range.</summary>
    [SerializeField]
    private SphereCollider colliderPlayerRange;
    #endregion

    #region Spawned Objects
    /// <summary>Reference to the currently spawned chest.</summary>
    private GameObject chest;

    /// <summary>List of entity components from spawned enemies for health tracking.</summary>
    private List<Entity> entities;

    /// <summary>List of spawned enemy GameObjects for management and cleanup.</summary>
    private List<GameObject> enemiesToObject;
    #endregion

    #region Spawn Configuration
    /// <summary>Randomly selected difficulty level for this spawn cycle.</summary>
    private int randomDifficulty;

    /// <summary>Number of enemies to spawn based on difficulty.</summary>
    private int numberOfEnemiesToSpawn;

    /// <summary>Current count of alive enemies.</summary>
    private int enemyCount;

    /// <summary>Radius within which enemies can be spawned around the spawn point.</summary>
    private float spawnRadius;

    /// <summary>Extra radius to add to player detection range.</summary>
    [SerializeField]
    private float extraRadius = 0f;
    #endregion

    #region Spawn Control Flags
    /// <summary>Whether enemies can respawn multiple times after being defeated.</summary>
    [SerializeField]
    private bool canMultipleRespawn = false;

    /// <summary>Whether the spawner is currently in a respawn timer.</summary>
    private bool inTimer = false;

    /// <summary>Whether all spawned enemies are currently dead.</summary>
    private bool allEnemiesDead = true;

    /// <summary>Whether difficulty should be randomly selected or use fixed value.</summary>
    [SerializeField]
    private bool canGetRandomDifficulty = true;

    /// <summary>Fixed difficulty level when random difficulty is disabled.</summary>
    [SerializeField]
    private int difficulty = 0;

    /// <summary>Whether the spawner is ready to respawn enemies.</summary>
    private bool isReadyToRespawn = false;

    /// <summary>Whether enemies need to be spawned.</summary>
    private bool isEnemyNeedSpawned = true;

    /// <summary>Whether the player is within spawn range.</summary>
    private bool isPlayerInRange = false;

    /// <summary>Whether the spawner is active and can spawn enemies.</summary>
    private bool isTheSpawnerActiveToSpawn = true;

    /// <summary>Whether spawning should be completely stopped.</summary>
    private bool stopSpawn = false;
    #endregion

    #region Timing Configuration
    /// <summary>Time in seconds to wait before respawning enemies.</summary>
    private float timerForRespawn = 120f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the spawner and sets up initial configuration.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(1)
    void Start()
    {
        // Initialize lists
        enemiesToObject = new List<GameObject>();
        entities = new List<Entity>();

        // Set spawn radius based on patrol zone
        spawnRadius = patrolZoneTrigger.getPatrolRange() - 5f;

        // Configure player detection range
        if (colliderPlayerRange != null)
        {
            colliderPlayerRange.radius = patrolZoneTrigger.getPatrolRange() + 170f + extraRadius;
        }

        // Set up initial spawn configuration
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        putChestInPlaceHolder();
    }

    /// <summary>
    /// Called every frame to manage spawning logic and enemy state.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(e) where e = number of enemies
    void Update()
    {
        // Exit if spawning is stopped
        if (stopSpawn)
        {
            return;
        }

        // Check if conditions are met for spawning
        if (
            isPlayerInRange
            && (canMultipleRespawn || isEnemyNeedSpawned)
            && !inTimer
            && isTheSpawnerActiveToSpawn
        )
        {
            if (allEnemiesDead)
            {
                // Prepare for spawning
                isReadyToRespawn = false;
                isTheSpawnerActiveToSpawn = false;
                isEnemyNeedSpawned = false;
                SpawnHandler();
                allEnemiesDead = false;
            }
        }
        else if (!allEnemiesDead && !isPlayerInRange && !isReadyToRespawn)
        {
            // Reset spawner when player leaves and enemies are alive
            isEnemyNeedSpawned = true;
            isTheSpawnerActiveToSpawn = true;
            destroyEnemies();
        }

        // Check chest state and manage respawn timer
        readyToRespawn();
        if (canMultipleRespawn && isReadyToRespawn && !inTimer)
        {
            inTimer = true;
            StartCoroutine(respawnTimer());
        }

        // Update enemy death status
        checkIfAllEnemiesAreDead();
    }
    #endregion

    #region Spawn Management
    /// <summary>
    /// Checks if the chest is open to determine if respawning is allowed.
    /// </summary>
    // COMPLEXITY ANALYSIS: readyToRespawn() - O(1)
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

    /// <summary>
    /// Handles the complete spawning process including difficulty and enemy setup.
    /// </summary>
    // COMPLEXITY ANALYSIS: SpawnHandler() - O(n) where n = numberOfEnemiesToSpawn
    private void SpawnHandler()
    {
        getRandomDifficulty();
        getNumberOfEnemiesToSpawn();
        putChestInPlaceHolder();
        spawnEnemies();
    }

    /// <summary>
    /// Checks if all spawned enemies are dead and updates chest state accordingly.
    /// </summary>
    // COMPLEXITY ANALYSIS: checkIfAllEnemiesAreDead() - O(e) where e = number of entities
    private void checkIfAllEnemiesAreDead()
    {
        // Remove dead enemies from tracking lists
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            if (entities[i].isDead())
            {
                entities.RemoveAt(i);
                enemyCount--;
            }
        }

        // If all enemies are dead, enable chest and reset spawner
        if (enemyCount == 0 && entities.Count == 0)
        {
            enemiesToObject.Clear();
            allEnemiesDead = true;
            chest.GetComponent<ObjectInteraction>().setCanOpen(true);
        }
    }

    /// <summary>
    /// Coroutine that manages the respawn timer for multiple respawn scenarios.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    // COMPLEXITY ANALYSIS: respawnTimer() - O(n) where n = numberOfEnemiesToSpawn
    private IEnumerator respawnTimer()
    {
        yield return new WaitForSeconds(timerForRespawn);
        SpawnHandler();
        allEnemiesDead = false;
        inTimer = false;
        isEnemyNeedSpawned = true;
        isTheSpawnerActiveToSpawn = true;
    }
    #endregion

    #region Difficulty and Enemy Count Management
    /// <summary>
    /// Determines the number of enemies to spawn based on difficulty level.
    /// </summary>
    // COMPLEXITY ANALYSIS: getNumberOfEnemiesToSpawn() - O(1)
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

    /// <summary>
    /// Sets the difficulty level either randomly or from fixed value.
    /// </summary>
    // COMPLEXITY ANALYSIS: getRandomDifficulty() - O(1)
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
    #endregion

    #region Object Spawning
    /// <summary>
    /// Spawns a chest based on the current difficulty level.
    /// </summary>
    // COMPLEXITY ANALYSIS: putChestInPlaceHolder() - O(1)
    private void putChestInPlaceHolder()
    {
        // Destroy existing chest if present
        if (chest != null)
        {
            Destroy(chest);
        }

        // Spawn new chest based on difficulty
        chest = Instantiate(
            chests[randomDifficulty],
            chestPlaceHolder.transform.position,
            Quaternion.identity
        );
        chest.transform.SetParent(chestPlaceHolder.transform, worldPositionStays: true);
        chest.GetComponent<ObjectInteraction>().setCanOpen(false);
    }

    /// <summary>
    /// Spawns enemies at valid NavMesh positions within the spawn radius.
    /// </summary>
    // COMPLEXITY ANALYSIS: spawnEnemies() - O(n) where n = numberOfEnemiesToSpawn
    private void spawnEnemies()
    {
        Vector3 center = enemyPlaceHolder.transform.position;

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            bool foundSpot = false;
            Vector3 spawnPosition = Vector3.zero;

            // Try to find a valid spawn position on NavMesh
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
                spawnEnemy(spawnPosition);
            }
            else
            {
                Debug.LogWarning(
                    $"Enemy {i} could not find a valid spawn position on NavMesh after 10 attempts."
                );
            }
        }
    }

    /// <summary>
    /// Spawns an enemy at the specified position.
    /// </summary>
    /// <param name="spawnPosition">The position to spawn the enemy at.</param>
    // COMPLEXITY ANALYSIS: spawnEnemy() - O(1)
    private void spawnEnemy(Vector3 spawnPosition)
    {
        // Spawn enemy at valid position
        GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Count)];
        GameObject cloneEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        cloneEnemy.transform.SetParent(enemyPlaceHolder.transform, worldPositionStays: true);

        // Track enemy entity and object
        entities.Add((Entity)cloneEnemy.GetComponent<StartNpc>().GetNpcsInstance());
        enemyCount++;
        enemiesToObject.Add(cloneEnemy);
    }
    #endregion

    #region Enemy Management
    /// <summary>
    /// Destroys all spawned enemies and resets spawner state.
    /// </summary>
    // COMPLEXITY ANALYSIS: destroyEnemies() - O(e) where e = number of enemies
    public void destroyEnemies()
    {
        foreach (GameObject enemy in enemiesToObject)
        {
            Destroy(enemy);
        }
        enemiesToObject.Clear();
        allEnemiesDead = true;
        chest.GetComponent<ObjectInteraction>().setCanOpen(false);
        isEnemyNeedSpawned = true;
    }

    /// <summary>
    /// Kills all spawned enemies by setting their health to 0.
    /// </summary>
    // COMPLEXITY ANALYSIS: killAllEnemies() - O(e) where e = number of entities
    public void killAllEnemies()
    {
        foreach (Entity entity in entities)
        {
            entity.setHealth(0);
        }
        enemiesToObject.Clear();
    }
    #endregion

    #region Player Detection
    /// <summary>
    /// Called when player enters the spawn trigger area.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    // COMPLEXITY ANALYSIS: OnTriggerEnter() - O(1)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    /// <summary>
    /// Called when player exits the spawn trigger area.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    // COMPLEXITY ANALYSIS: OnTriggerExit() - O(1)
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
    #endregion

    #region Public Interface Methods
    /// <summary>Gets whether all enemies are currently dead.</summary>
    /// <returns>True if all enemies are dead; otherwise false.</returns>
    // COMPLEXITY ANALYSIS: getAllEnemiesDead() - O(1)
    public bool getAllEnemiesDead()
    {
        return allEnemiesDead;
    }

    /// <summary>Sets whether multiple respawning is allowed.</summary>
    /// <param name="canMultipleRespawn">Whether multiple respawning should be enabled.</param>
    // COMPLEXITY ANALYSIS: setCanMultipleRespawn() - O(1)
    public void setCanMultipleRespawn(bool canMultipleRespawn)
    {
        this.canMultipleRespawn = canMultipleRespawn;
    }

    /// <summary>Gets whether the spawner is ready to respawn.</summary>
    /// <returns>True if ready to respawn; otherwise false.</returns>
    // COMPLEXITY ANALYSIS: getIsReadyToRespawn() - O(1)
    public bool getIsReadyToRespawn()
    {
        return isReadyToRespawn;
    }

    /// <summary>Sets whether enemies need to be spawned.</summary>
    /// <param name="isEnemyNeedSpawned">Whether enemies need spawning.</param>
    // COMPLEXITY ANALYSIS: setIsEnemyNeedSpawned() - O(1)
    public void setIsEnemyNeedSpawned(bool isEnemyNeedSpawned)
    {
        this.isEnemyNeedSpawned = isEnemyNeedSpawned;
    }

    /// <summary>Sets whether the spawner is active for spawning.</summary>
    /// <param name="isTheSpawnerActiveToSpawn">Whether spawning should be active.</param>
    // COMPLEXITY ANALYSIS: setIsTheSpawnerActiveToSpawn() - O(1)
    public void setIsTheSpawnerActiveToSpawn(bool isTheSpawnerActiveToSpawn)
    {
        this.isTheSpawnerActiveToSpawn = isTheSpawnerActiveToSpawn;
    }

    /// <summary>Sets the respawn timer duration.</summary>
    /// <param name="timerForRespawn">Time in seconds to wait before respawning.</param>
    // COMPLEXITY ANALYSIS: setTimerForRespawn() - O(1)
    public void setTimerForRespawn(float timerForRespawn)
    {
        this.timerForRespawn = timerForRespawn;
    }

    /// <summary>Sets whether spawning should be completely stopped.</summary>
    /// <param name="stopSpawn">Whether spawning should be stopped.</param>
    // COMPLEXITY ANALYSIS: setStopSpawn() - O(1)
    public void setStopSpawn(bool stopSpawn)
    {
        this.stopSpawn = stopSpawn;
    }
    #endregion
}
