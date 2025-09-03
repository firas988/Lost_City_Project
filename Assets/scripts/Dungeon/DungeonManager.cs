using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages dungeon progression, room transitions, and enemy spawning.
/// Handles boss encounters and dungeon exit accessibility.
/// Coordinates room progression, enemy activation, and boss encounters throughout the dungeon.
/// </summary>
public class DungeonManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Room Configuration")]
    /// <summary>
    /// List of all rooms in the dungeon sequence.
    /// Rooms are processed in order for progressive difficulty.
    /// </summary>
    [SerializeField]
    private List<GameObject> rooms;

    [Header("Room Exit Configuration")]
    /// <summary>
    /// Tag used to identify room exit barriers.
    /// These barriers block player progress until room is cleared.
    /// </summary>
    [SerializeField]
    private string roomExitTag = "RoomExit";

    /// <summary>
    /// Name of the child GameObject containing enemies in each room.
    /// Used to find and manage enemy groups within rooms.
    /// </summary>
    [SerializeField]
    private string enemiesName = "Enemies";

    [Header("Boss Configuration")]
    /// <summary>
    /// GameObject containing the final boss entrance sequence.
    /// Controls the cinematic transition to the final boss encounter.
    /// </summary>
    [SerializeField]
    private GameObject finalBossEnter;

    /// <summary>
    /// GameObject representing the dungeon exit portal.
    /// Becomes accessible after completing all dungeon content.
    /// </summary>
    [SerializeField]
    private GameObject dungeonExit;

    /// <summary>
    /// Reference to the main boss enemy GameObject.
    /// Controls boss spawning and behavior during encounters.
    /// </summary>
    [SerializeField]
    private GameObject boss;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the quest manager for tracking dungeon completion.
    /// Used to update quest progress and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to the player character for position and state management.
    /// Used to control player position during boss encounters.
    /// </summary>
    private Player player;

    /// <summary>
    /// Current room index in the dungeon progression.
    /// Tracks which room the player is currently in (0-based).
    /// </summary>
    private int currentRoomIndex;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the dungeon manager and sets up initial room state.
    /// Prepares all rooms with enemies in dissolved state and disabled.
    /// </summary>
    void Start()
    {
        // Initialize room progression to first room
        currentRoomIndex = 0;

        // Find and store references to required components
        player = GameObject.FindWithTag("Player").GetComponent<StartPlayer>().getPlayer();
        questManager = GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponentInChildren<QuestManager>();

        // Initialize all rooms with enemies in dissolved state
        foreach (GameObject room in rooms)
        {
            // Find the enemies container in each room
            GameObject enemies = room.transform.Find(enemiesName).gameObject;
            if (enemies != null)
            {
                // Process each enemy in the room
                foreach (Transform child in enemies.transform)
                {
                    // Set enemy to fully dissolved state
                    child.gameObject.GetComponent<DissolvingController>().setDissolveAmount(1f);

                    // Get enemy components for state management
                    EnemyMovement enemyMovement = child.gameObject.GetComponent<EnemyMovement>();
                    EnemyHealthBar enemyHealthBar =
                        child.gameObject.GetComponentInChildren<EnemyHealthBar>();

                    if (enemyMovement != null && enemyHealthBar != null)
                    {
                        // Standard enemy - disable movement and hide health bar
                        enemyMovement.setCanMove(false);
                        enemyHealthBar.hideHealthBar();
                    }
                    else
                    {
                        // Special enemy (like boss) - disable NavMeshAgent
                        child.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    }

                    // Deactivate enemy until room is triggered
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Dungeon Progression Methods
    /// <summary>
    /// Starts the current dungeon room and spawns enemies.
    /// Activates enemies and blocks room exits for the current room.
    /// </summary>
    public void StartDungeon()
    {
        if (currentRoomIndex < rooms.Count)
        {
            // Check if this is the final room (boss room)
            if (currentRoomIndex == rooms.Count - 1)
            {
                StartFinallBossScene();
                return;
            }

            // Block room exits to prevent player escape
            blockCurrentRoom();

            // Find and activate enemies in the current room
            GameObject enemies = rooms[currentRoomIndex].transform.Find(enemiesName).gameObject;
            if (enemies != null)
            {
                // Activate each enemy and start dissolve animation
                foreach (Transform child in enemies.transform)
                {
                    child.gameObject.SetActive(true);
                    StartCoroutine(WaitForEnemiesToDeDissolve(child.gameObject));
                }
            }
        }
    }

    /// <summary>
    /// Advances to the next room in the dungeon.
    /// Deactivates current room enemies and removes exit barriers.
    /// </summary>
    public void NextRoom()
    {
        // Deactivate all enemies in the current room
        GameObject enemies = rooms[currentRoomIndex].transform.Find(enemiesName).gameObject;
        enemies.SetActive(false);

        // Remove room exit barriers to allow progression
        foreach (Transform child in rooms[currentRoomIndex].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Move to next room
        currentRoomIndex++;
    }
    #endregion

    #region Enemy Management Methods
    /// <summary>
    /// Coroutine that handles enemy spawning and activation.
    /// Manages the dissolve animation and enables enemy behavior.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to activate.</param>
    /// <returns>Coroutine for managing the spawn sequence.</returns>
    public IEnumerator WaitForEnemiesToDeDissolve(GameObject enemy)
    {
        // Start the dissolve-in animation
        enemy.GetComponent<DissolvingController>().StartDeDissolve();

        // Wait for dissolve animation to complete
        yield return new WaitForSeconds(5f);

        // Get enemy components for activation
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        EnemyHealthBar enemyHealthBar = enemy.GetComponentInChildren<EnemyHealthBar>();

        if (enemyMovement != null && enemyHealthBar != null)
        {
            // Standard enemy - show health bar and enable movement
            enemyHealthBar.showHealthBar();
            enemyMovement.setCanMove(true);
        }
        else
        {
            // Special enemy (like wolf boss) - enable special movement
            WolfBossChasing wolfBossChasing = enemy.GetComponent<WolfBossChasing>();
            if (wolfBossChasing != null)
            {
                wolfBossChasing.setCanMove(true);
            }

            // Enable NavMeshAgent for navigation
            UnityEngine.AI.NavMeshAgent navMeshAgent =
                enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
        }
    }
    #endregion

    #region Room Management Methods
    /// <summary>
    /// Blocks the current room by activating room exit barriers.
    /// Prevents player from leaving until all enemies are defeated.
    /// </summary>
    public void blockCurrentRoom()
    {
        // Activate all room exit barriers in the current room
        foreach (Transform child in rooms[currentRoomIndex].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Boss Management Methods
    /// <summary>
    /// Spawns the boss and blocks the current room.
    /// Activates boss encounter and prevents player escape.
    /// </summary>
    public void spawnBoss()
    {
        // Activate boss and disable movement initially
        boss.SetActive(true);
        boss.GetComponent<WolfBossChasing>().setCanMove(false);

        // Block room exits to contain the boss encounter
        blockCurrentRoom();
    }

    /// <summary>
    /// Starts the boss dissolve animation.
    /// Initiates the spawn sequence for the boss enemy.
    /// </summary>
    public void DeDissolveBoss()
    {
        StartCoroutine(WaitForEnemiesToDeDissolve(boss));
    }

    /// <summary>
    /// Starts the final boss scene.
    /// Positions player and activates boss entrance sequence.
    /// </summary>
    public void StartFinallBossScene()
    {
        // Find player and position them at the boss room entrance
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerController>().transform.position = rooms[rooms.Count - 1]
            .transform.Find("Entrance")
            .position;

        // Deactivate player during boss sequence
        player.SetActive(false);

        // Activate the final boss entrance sequence
        finalBossEnter.SetActive(true);
    }

    /// <summary>
    /// Stops the final boss scene.
    /// Deactivates boss entrance sequence and restores normal gameplay.
    /// </summary>
    public void StopFinallBossScene()
    {
        finalBossEnter.SetActive(false);
    }

    /// <summary>
    /// Closes the final room by activating exit barriers.
    /// Prevents player from leaving the boss room during encounter.
    /// </summary>
    public void closeFinalRoom()
    {
        // Activate all exit barriers in the final room
        foreach (Transform child in rooms[rooms.Count - 1].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Dungeon Exit Methods
    /// <summary>
    /// Opens the dungeon exit for the player.
    /// Makes the exit portal accessible after completing all content.
    /// </summary>
    public void openDungeonExit()
    {
        dungeonExit.SetActive(true);
    }
    #endregion
}
