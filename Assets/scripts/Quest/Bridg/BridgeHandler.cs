using System.Collections;
using UnityEngine;

/// <summary>
/// Handles bridge-related quest logic and hologram visibility.
/// Manages the GoToBridge quest completion and hologram destruction.
/// Coordinates bridge access control and quest progression tracking.
/// </summary>
public class BridgeHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Quest Visual Elements")]
    /// <summary>
    /// GameObject containing the bridge hologram that blocks access.
    /// Destroyed when the GoToBridge quest is completed.
    /// </summary>
    [SerializeField]
    private GameObject Hologram;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to the current active quest being tracked.
    /// Stores the quest instance for completion management.
    /// </summary>
    private Quest quest;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    [Header("Quest State Management")]
    /// <summary>
    /// Flag indicating whether the bridge quest has been completed.
    /// Prevents repeated quest completion processing.
    /// </summary>
    private bool isQuestIsCompleted = false;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the bridge handler and starts quest completion checking.
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
    /// Updates the bridge handler logic each frame.
    /// Monitors quest state and manages hologram visibility.
    /// </summary>
    void Update()
    {
        // Skip processing if quest is already completed
        if (isQuestIsCompleted)
        {
            return;
        }

        // Check current quest state and manage hologram
        checkIfTheQuestIsGoToBridge();
    }
    #endregion

    #region Quest Management Methods
    /// <summary>
    /// Checks if the current quest is a GoToBridge quest and manages hologram visibility.
    /// Destroys the hologram when the quest is active to grant bridge access.
    /// </summary>
    private void checkIfTheQuestIsGoToBridge()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        quest = player.getCurrentMainQuest();

        if (quest is GoToBridge)
        {
            // Quest is active - destroy hologram to grant bridge access
            Destroy(Hologram);
        }
        else
        {
            // Quest is not active - clear quest reference
            quest = null;
        }
    }

    /// <summary>
    /// Completes the bridge quest if it's active and not already completed.
    /// Triggers quest completion and prevents repeated processing.
    /// </summary>
    public void completeQuest()
    {
        if (!isQuestIsCompleted && quest != null)
        {
            // Complete the GoToBridge quest and mark as finished
            (quest as GoToBridge)?.CompleteQuest();
            isQuestIsCompleted = true;
        }
    }
    #endregion

    #region Quest Completion Checking Methods
    /// <summary>
    /// Coroutine that waits for the quest manager to be ready before checking quest completion.
    /// Ensures proper initialization before quest state evaluation.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if quest was already completed previously
        checkIfTheQuestIsGoToBridgeIsCompleted();
    }

    /// <summary>
    /// Checks if the GoToBridge quest has been completed and updates the hologram accordingly.
    /// Destroys the hologram and marks quest as completed if previously finished.
    /// </summary>
    private void checkIfTheQuestIsGoToBridgeIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToBridge)))
        {
            // Quest already completed - destroy hologram and mark as finished
            Destroy(Hologram);
            isQuestIsCompleted = true;
        }
    }
    #endregion
}
