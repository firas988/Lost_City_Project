using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles castle-related quest logic including enemy spawning, door management, and quest progression.
/// Manages multiple quest types: GoToCastel, KillAllTheEnemyInTheCastel, and FindTheMapPart.
/// Coordinates castle access control, enemy management, and quest completion rewards.
/// </summary>
public class CastelKillQuestHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Castle Access Control")]
    /// <summary>
    /// List of outer door GameObjects that control castle entry.
    /// Opened when the GoToCastel quest is active.
    /// </summary>
    [SerializeField]
    private List<GameObject> outDoor;

    /// <summary>
    /// Inner door GameObject that controls access to deeper castle areas.
    /// Opened when the KillAllTheEnemyInTheCastel quest is completed.
    /// </summary>
    [SerializeField]
    private GameObject inDoor;

    [Header("Enemy Management")]
    /// <summary>
    /// List of enemy spawner components that manage castle enemies.
    /// Used to control enemy spawning and check completion status.
    /// </summary>
    [SerializeField]
    private List<Enemyspawner> enemySpawners;

    [Header("Quest Objectives")]
    /// <summary>
    /// GameObject containing map parts that must be collected.
    /// Destroyed when the FindTheMapPart quest is completed.
    /// </summary>
    [SerializeField]
    private GameObject mapParts;
    #endregion

    #region Private Fields
    [Header("Quest State Management")]
    /// <summary>
    /// Reference to the current active quest being tracked.
    /// Stores the quest instance for completion management.
    /// </summary>
    private Quest currentQuest;

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Flag indicating whether the GoToCastel quest is currently active.
    /// Controls castle access and door management.
    /// </summary>
    private bool isQuestIsGoToCastel = false;

    /// <summary>
    /// Flag indicating whether the KillAllTheEnemyInTheCastel quest is currently active.
    /// Controls enemy spawning and inner door access.
    /// </summary>
    private bool isQuestIsKillAllTheEnemyInTheCastel = false;

    /// <summary>
    /// Flag indicating whether all enemy spawners have finished their spawning cycles.
    /// Used to determine when to check quest completion.
    /// </summary>
    private bool isAllSpawnersAreDone = false;

    /// <summary>
    /// Flag indicating whether the FindTheMapPart quest is currently active.
    /// Controls map part interaction and collection.
    /// </summary>
    private bool isQuestIsFindTheMapPart = false;

    /// <summary>
    /// Flag indicating whether any castle quest has been completed.
    /// Prevents repeated quest processing.
    /// </summary>
    private bool isQuestIsCompleted = false;

    /// <summary>
    /// Flag indicating whether the KillAllTheEnemyInTheCastel quest has been completed.
    /// Prevents repeated quest completion processing.
    /// </summary>
    private bool isQuestKillAllTheEnemyInTheCastelIsCompleted = false;

    [Header("System Configuration")]
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the castle quest handler and starts quest completion checking.
    /// Sets up component references and begins monitoring quest progress.
    /// </summary>
    void Start()
    {
        // Find and store reference to the quest manager
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    /// <summary>
    /// Updates the castle quest handler logic each frame.
    /// Monitors quest states and manages castle progression.
    /// </summary>
    void Update()
    {
        // Skip processing if any quest is already completed
        if (isQuestIsCompleted)
        {
            return;
        }

        // Check current quest states and manage progression
        checkIfTheQuestIsGotToTheCastel();

        if (!isQuestKillAllTheEnemyInTheCastelIsCompleted)
        {
            checkIfTheQuestIsKillAllTheEnemyInTheCastel();
        }

        checkIfTheQuestIsFindTheMapPart();

        if (!isAllSpawnersAreDone)
        {
            checkIfAllSpawnersAreDone();
        }
        else
        {
            TryCompleteTheKillAllTheEnemyInTheCastelQuest();
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

    #region Quest Completion Methods
    /// <summary>
    /// Attempts to complete the FindTheMapPart quest and destroys the map parts.
    /// Triggers quest completion when player collects the map parts.
    /// </summary>
    private void TryCompleteTheFindTheMapPartQuest()
    {
        if (currentQuest is FindTheMapPart && isQuestIsFindTheMapPart)
        {
            // Complete the quest and destroy the map parts
            (currentQuest as FindTheMapPart)?.CompleteQuest();
            Destroy(mapParts);
        }
    }

    /// <summary>
    /// Attempts to complete the GoToCastel quest.
    /// Marks the quest as completed when player reaches the castle.
    /// </summary>
    private void TryCompleteTheGoToCastelQuest()
    {
        if (currentQuest is GoToCastel && isQuestIsGoToCastel)
        {
            // Complete the quest and reset the flag
            (currentQuest as GoToCastel)?.CompleteQuest();
            isQuestIsGoToCastel = false;
        }
    }

    /// <summary>
    /// Attempts to complete the KillAllTheEnemyInTheCastel quest and opens the inner door.
    /// Triggers quest completion and grants access to deeper castle areas.
    /// </summary>
    private void TryCompleteTheKillAllTheEnemyInTheCastelQuest()
    {
        if (
            currentQuest is KillAllTheEnemyInTheCastel
            && isQuestIsKillAllTheEnemyInTheCastel
            && !isQuestKillAllTheEnemyInTheCastelIsCompleted
        )
        {
            // Complete the quest and open the inner door
            (currentQuest as KillAllTheEnemyInTheCastel)?.CompleteQuest();
            inDoor.GetComponent<CastelDoorHandler>().openTheDoor();
            isQuestIsKillAllTheEnemyInTheCastel = false;
            isQuestKillAllTheEnemyInTheCastelIsCompleted = true;
        }
    }
    #endregion

    #region Quest State Checking Methods
    /// <summary>
    /// Checks if all enemy spawners have finished spawning enemies.
    /// Updates the spawner completion flag for quest progression.
    /// </summary>
    private void checkIfAllSpawnersAreDone()
    {
        foreach (Enemyspawner enemySpawner in enemySpawners)
        {
            if (!enemySpawner.getIsReadyToRespawn())
            {
                // Not all spawners are ready - keep checking
                isAllSpawnersAreDone = false;
                return;
            }
        }
        // All spawners are ready - mark as complete
        isAllSpawnersAreDone = true;
    }

    /// <summary>
    /// Checks if the current quest is GoToCastel and manages castle access.
    /// Opens outer doors and sets up quest completion triggers.
    /// </summary>
    private void checkIfTheQuestIsGotToTheCastel()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();

        if (quest is GoToCastel)
        {
            if (!isQuestIsGoToCastel)
            {
                // Quest just started - set up castle access
                currentQuest = quest;
                isQuestIsGoToCastel = true;
                openTheOutDoor();
                subscribeToTheQuest();
            }
        }
    }

    /// <summary>
    /// Checks if the current quest is KillAllTheEnemyInTheCastel.
    /// Sets up enemy management for the castle clearing quest.
    /// </summary>
    private void checkIfTheQuestIsKillAllTheEnemyInTheCastel()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();

        if (quest is KillAllTheEnemyInTheCastel)
        {
            if (!isQuestIsKillAllTheEnemyInTheCastel)
            {
                // Quest just started - set up enemy management
                currentQuest = quest;
                isQuestIsKillAllTheEnemyInTheCastel = true;
            }
        }
    }

    /// <summary>
    /// Checks if the current quest is FindTheMapPart and sets up map part interaction.
    /// Subscribes to map part collection events for quest completion.
    /// </summary>
    private void checkIfTheQuestIsFindTheMapPart()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();

        if (quest is FindTheMapPart)
        {
            if (!isQuestIsFindTheMapPart)
            {
                // Quest just started - set up map part interaction
                currentQuest = quest;
                isQuestIsFindTheMapPart = true;
                mapParts
                    .GetComponent<MapColiderHandler>()
                    .subscribeToOnTriggerEnter(TryCompleteTheFindTheMapPartQuest);
            }
        }
    }
    #endregion

    #region Quest Completion Checking Methods
    /// <summary>
    /// Checks if the GoToCastel quest has been completed and updates castle access accordingly.
    /// Opens outer doors and sets up quest completion if previously finished.
    /// </summary>
    private void CheckIfGoToCastelIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToCastel)))
        {
            // Quest already completed - grant castle access
            isQuestIsGoToCastel = true;
            openTheOutDoor();
            subscribeToTheQuest();
        }
    }

    /// <summary>
    /// Checks if the KillAllTheEnemyInTheCastel quest has been completed and cleans up enemies.
    /// Destroys remaining enemies and opens inner door if previously finished.
    /// </summary>
    private void CheckIfKillAllTheEnemyInTheCastelIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(KillAllTheEnemyInTheCastel)))
        {
            // Quest already completed - clean up enemies and open inner door
            foreach (Enemyspawner enemySpawner in enemySpawners)
            {
                enemySpawner.destroyEnemies();
            }
            inDoor.GetComponent<CastelDoorHandler>().openTheDoor();
        }
    }

    /// <summary>
    /// Checks if the FindTheMapPart quest has been completed and removes map parts.
    /// Destroys map parts and marks quest as completed if previously finished.
    /// </summary>
    private void CheckIfFindTheMapPartIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(FindTheMapPart)))
        {
            // Quest already completed - remove map parts and mark as finished
            Destroy(mapParts);
            isQuestIsCompleted = true;
        }
    }
    #endregion

    #region Door Management Methods
    /// <summary>
    /// Opens all outer doors of the castle.
    /// Grants access to the castle when the GoToCastel quest is active.
    /// </summary>
    private void openTheOutDoor()
    {
        foreach (GameObject outDoor in outDoor)
        {
            outDoor.GetComponent<CastelDoorHandler>().openTheDoor();
        }
    }

    /// <summary>
    /// Subscribes to door trigger events for quest completion.
    /// Sets up quest completion triggers when players interact with castle doors.
    /// </summary>
    private void subscribeToTheQuest()
    {
        foreach (GameObject outDoor in outDoor)
        {
            outDoor
                .GetComponent<CastelDoorHandler>()
                .subscribeToOnTriggerEnter(TryCompleteTheGoToCastelQuest);
        }
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Coroutine that waits for the quest manager to be ready before checking quest completion.
    /// Ensures proper initialization before quest state evaluation.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check all quest completion states
        CheckIfGoToCastelIsCompleted();
        CheckIfKillAllTheEnemyInTheCastelIsCompleted();
        CheckIfFindTheMapPartIsCompleted();
    }
    #endregion
}
