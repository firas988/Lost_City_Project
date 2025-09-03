using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Handles enemy spawning for wave-based quests, managing multiple waves and quest completion.
/// Coordinates with enemy spawners, hologram display, and wave progression UI.
/// Manages wave progression, enemy spawning cycles, and quest completion rewards.
/// </summary>
public class EnemySpawnQuestHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Enemy Management")]
    /// <summary>
    /// List of enemy spawner components that manage wave-based enemy spawning.
    /// Used to control enemy spawning cycles and check completion status.
    /// </summary>
    [SerializeField]
    private List<Enemyspawner> enemySpawners;

    [Header("Quest Visual Elements")]
    /// <summary>
    /// Reference to the hologram handler for quest guidance display.
    /// Controls hologram visibility based on quest state.
    /// </summary>
    [SerializeField]
    private HologramHandler hologramHandler;

    /// <summary>
    /// GameObject containing the main hologram for the wave quest.
    /// Displayed during active wave progression.
    /// </summary>
    [SerializeField]
    private GameObject hologram;

    [Header("Quest Interaction")]
    /// <summary>
    /// Reference to the wave completion trigger collider.
    /// Handles player interaction for quest completion.
    /// </summary>
    [SerializeField]
    private KillAllWaveMapColider killAllWaveMapColider;

    [Header("UI Elements")]
    /// <summary>
    /// List of canvas GameObjects that display wave progress information.
    /// Shows current wave number and total wave count to the player.
    /// </summary>
    [SerializeField]
    private List<GameObject> canvasWave;
    #endregion

    #region Private Fields
    [Header("Quest State Management")]
    /// <summary>
    /// Reference to the current active quest being tracked.
    /// Stores the quest instance for completion management.
    /// </summary>
    private Quest quest;

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    [Header("Wave Configuration")]
    /// <summary>
    /// Total number of waves that must be completed for the quest.
    /// Controls quest progression and completion requirements.
    /// </summary>
    private int numberOfWaves = 3;

    /// <summary>
    /// Current wave index being processed (0-based).
    /// Tracks progress through the wave sequence.
    /// </summary>
    private int currentWave = 0;

    [Header("Spawning State Management")]
    /// <summary>
    /// Flag indicating whether the system is ready to spawn the next wave.
    /// Controls wave progression timing.
    /// </summary>
    private bool isReadyToSpawn = false;

    /// <summary>
    /// Flag indicating whether to check if all spawners are ready to respawn.
    /// Prevents premature wave progression checks.
    /// </summary>
    private bool readyToCheckIfAllReadyToRespawn = true;

    /// <summary>
    /// Flag indicating whether the system is currently in a respawn timer.
    /// Prevents multiple simultaneous spawning operations.
    /// </summary>
    private bool inTimer = false;

    /// <summary>
    /// Flag indicating whether the FinshAllTheWave quest is currently active.
    /// Controls wave progression and quest management.
    /// </summary>
    private bool isQuestIsFinshAllTheWave = false;

    /// <summary>
    /// Flag indicating whether the wave quest has been completed.
    /// Prevents repeated quest processing.
    /// </summary>
    private bool isQuestIsCompleted = false;

    [Header("System Configuration")]
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the enemy spawn quest handler and sets up initial state.
    /// Configures spawners, timers, and quest monitoring.
    /// </summary>
    void Start()
    {
        // Find and store reference to the quest manager
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Set up all enemy spawners to need respawning
        setAllEnemySpawnersToNeedToRespawn(true);

        // Set respawn timer for all spawners
        setTimerForRespawn();

        // Subscribe to wave completion events
        killAllWaveMapColider.subscribeToOnEnter(CompleteQuest);

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompletedLood());
    }

    /// <summary>
    /// Updates the enemy spawn quest handler logic each frame.
    /// Monitors quest state, wave progression, and enemy spawning cycles.
    /// </summary>
    void Update()
    {
        // Skip processing if quest is already completed
        if (isQuestIsCompleted)
        {
            return;
        }

        // Check current quest state if wave quest is not active
        if (!isQuestIsFinshAllTheWave)
        {
            checkThecurrentQuest();
        }

        // Skip processing if no quest or quest is completed
        if (quest == null || quest.isCompleted)
        {
            return;
        }

        // Check quest completion and manage wave progression
        checkIfTheQuestIsCompleted();

        if (!inTimer)
        {
            checkIfAllReadyToRespawn();
        }

        // Spawn next wave if ready and not in timer
        if (isReadyToSpawn && !inTimer && !quest.isCompleted)
        {
            isReadyToSpawn = false;
            currentWave++;
            updateCanvasWave();
            inTimer = true;
            StartCoroutine(spawnEnemies());
        }

        // Debug key for testing - kill all enemies instantly
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.killAllEnemies();
            }
        }
    }
    #endregion

    #region Wave Management Methods
    /// <summary>
    /// Updates the wave display canvas to show current wave progress.
    /// Updates all wave UI elements with current wave information.
    /// </summary>
    private void updateCanvasWave()
    {
        foreach (GameObject canvas in canvasWave)
        {
            // Update wave progress text (e.g., "Wave 2/3")
            canvas.GetComponentInChildren<TextMeshProUGUI>().text =
                "Wave " + (currentWave + 1) + "/" + numberOfWaves;
        }
    }

    /// <summary>
    /// Checks if the quest has been completed and updates UI accordingly.
    /// Manages final wave completion and UI cleanup.
    /// </summary>
    private void checkIfTheQuestIsCompleted()
    {
        if (currentWave == numberOfWaves)
        {
            // All waves completed - stop spawning and clean up UI
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

    /// <summary>
    /// Sets the timer for enemy respawn across all spawners.
    /// Configures consistent respawn timing for wave progression.
    /// </summary>
    private void setTimerForRespawn()
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            enemySpawner.setTimerForRespawn(10f);
        }
    }

    /// <summary>
    /// Coroutine that manages enemy spawning timing and wave progression.
    /// Controls the delay between waves and spawner state management.
    /// </summary>
    /// <returns>Coroutine for managing the spawn sequence.</returns>
    private IEnumerator spawnEnemies()
    {
        // Disable spawner readiness checking during spawn sequence
        readyToCheckIfAllReadyToRespawn = false;

        // Wait for spawn sequence to begin
        yield return new WaitForSeconds(1f);

        // Set all spawners to need respawning
        setAllEnemySpawnersToNeedToRespawn(true);

        // Wait for spawn sequence to complete
        yield return new WaitForSeconds(5f);

        // Re-enable spawner readiness checking
        readyToCheckIfAllReadyToRespawn = true;
        isReadyToSpawn = false;
        inTimer = false;
    }
    #endregion

    #region Enemy Spawner Management Methods
    /// <summary>
    /// Sets all enemy spawners to either need spawning or not.
    /// Controls the spawning state across all spawner components.
    /// </summary>
    /// <param name="isEnemyNeedSpawned">Whether enemies need to be spawned.</param>
    private void setAllEnemySpawnersToNeedToRespawn(bool isEnemyNeedSpawned)
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            // Set spawner state for enemy spawning
            enemySpawner.setIsEnemyNeedSpawned(isEnemyNeedSpawned);
            enemySpawner.setIsTheSpawnerActiveToSpawn(isEnemyNeedSpawned);
        }
    }

    /// <summary>
    /// Checks if all enemy spawners are ready to respawn enemies.
    /// Updates the spawn readiness flag for wave progression.
    /// </summary>
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
                // Not all spawners are ready - keep waiting
                isReadyToSpawn = false;
                return;
            }
        }

        // All spawners are ready - mark as ready to spawn
        isReadyToSpawn = true;
    }
    #endregion

    #region Quest Management Methods
    /// <summary>
    /// Sets the quest for this handler and updates hologram visibility.
    /// Configures the handler for a specific wave-based quest.
    /// </summary>
    /// <param name="quest">The quest to be set for this handler.</param>
    public void setQuest(Quest quest)
    {
        this.quest = quest;

        // Hide hologram and disable handler when quest is set
        hologramHandler.setSphereHologramOut(false);
        hologramHandler.gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks the current quest and sets up wave-based quest if applicable.
    /// Identifies and configures wave-based quests for this handler.
    /// </summary>
    public void checkThecurrentQuest()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest currentQuest = player.getCurrentMainQuest();

        if (currentQuest is FinshAllTheWave)
        {
            if (!isQuestIsFinshAllTheWave)
            {
                // Quest just started - set up wave management
                setQuest(currentQuest);
                isQuestIsFinshAllTheWave = true;
            }
        }
    }

    /// <summary>
    /// Completes the wave-based quest.
    /// Triggers quest completion when all waves are finished.
    /// </summary>
    private void CompleteQuest()
    {
        (quest as FinshAllTheWave)?.CompleteQuest();
    }
    #endregion

    #region Quest Completion Checking Methods
    /// <summary>
    /// Checks if the FinshAllTheWave quest has been completed and cleans up accordingly.
    /// Manages final cleanup, cutscene triggering, and UI removal.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsFinshAllTheWaveIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(FinshAllTheWave)))
        {
            // Quest already completed - clean up all elements
            Destroy(GameObject.FindWithTag("FinshAllTheWaveMapPiece"));

            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.setStopSpawn(true);
                enemySpawner.destroyEnemies();
            }

            // Trigger final cutscene
            PlayableDirector director = GameObject
                .FindWithTag("GhostCutScene")
                .GetComponent<PlayableDirector>();
            director.time = director.duration;
            director.Evaluate();

            // Clean up UI and hologram
            Destroy(hologram);
            foreach (GameObject canvas in canvasWave)
            {
                Destroy(canvas);
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Coroutine that waits for the quest manager to be ready before checking quest completion.
    /// Ensures proper initialization before quest state evaluation.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompletedLood()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if quest was already completed previously
        if (checkIfTheQuestIsFinshAllTheWaveIsCompleted())
        {
            isQuestIsCompleted = true;
        }
    }
    #endregion
}
