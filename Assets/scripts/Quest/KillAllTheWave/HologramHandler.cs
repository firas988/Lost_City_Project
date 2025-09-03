using UnityEngine;

/// <summary>
/// Handles hologram visibility and quest interaction for the GoToFinshAllTheWave quest.
/// Manages sphere hologram display based on player proximity and quest state.
/// Coordinates hologram visibility with quest progression and completion.
/// </summary>
public class HologramHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Quest Visual Elements")]
    /// <summary>
    /// GameObject containing the sphere hologram that guides players to the wave completion area.
    /// Displayed when player is near the quest objective.
    /// </summary>
    [SerializeField]
    private GameObject SphereHologramOut;
    #endregion

    #region Private Fields
    [Header("Quest State Management")]
    /// <summary>
    /// Reference to the current active quest being tracked.
    /// Stores the quest instance for completion management.
    /// </summary>
    private Quest currentQuest;

    /// <summary>
    /// Flag indicating whether the GoToFinshAllTheWave quest is currently active.
    /// Controls hologram visibility and quest interaction.
    /// </summary>
    private bool isQuestIsGoToFinshAllTheWave = false;

    /// <summary>
    /// Flag indicating whether the quest has been completed.
    /// Prevents repeated quest processing and hologram display.
    /// </summary>
    private bool isQuestCompleted = false;

    [Header("System References")]
    /// <summary>
    /// Tag used to identify the Player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the hologram handler by finding the quest manager.
    /// Sets up the quest system reference for state management.
    /// </summary>
    private void Awake()
    {
        // Find and store reference to the quest manager
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
    }

    /// <summary>
    /// Updates the hologram handler logic each frame.
    /// Monitors quest state and manages hologram visibility.
    /// </summary>
    private void Update()
    {
        // Skip processing if quest is completed or already active
        if (isQuestCompleted || isQuestIsGoToFinshAllTheWave)
            return;

        // Check current quest state and update hologram accordingly
        checkThecurrentQuest();
    }
    #endregion

    #region Unity Trigger Methods
    /// <summary>
    /// Handles player entry into the hologram trigger area.
    /// Manages quest completion and hologram visibility based on quest state.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isQuestCompleted && currentQuest != null)
        {
            // Check if quest should be completed or hologram should be shown
            if (isQuestIsGoToFinshAllTheWave || checkIfTheQuestIsCompleted())
            {
                // Complete the quest and hide hologram
                (currentQuest as GoToFinshAllTheWave)?.CompleteQuest();
                isQuestCompleted = true;
                setSphereHologramOut(false);
                return;
            }

            // Show hologram to guide player to quest objective
            setSphereHologramOut(true);
        }
    }

    /// <summary>
    /// Handles player exit from the hologram trigger area.
    /// Hides the hologram when player leaves the quest area.
    /// </summary>
    /// <param name="other">The collider that exited the trigger area.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && !isQuestCompleted)
        {
            // Hide hologram when player leaves the quest area
            setSphereHologramOut(false);
        }
    }
    #endregion

    #region Hologram Management Methods
    /// <summary>
    /// Sets the sphere hologram visibility.
    /// Controls whether the quest guidance hologram is displayed.
    /// </summary>
    /// <param name="isActive">Whether the hologram should be active and visible.</param>
    public void setSphereHologramOut(bool isActive)
    {
        SphereHologramOut.SetActive(isActive);
    }
    #endregion

    #region Quest Management Methods
    /// <summary>
    /// Checks the current quest and updates hologram state accordingly.
    /// Sets up wave-based quest if applicable and manages hologram visibility.
    /// </summary>
    public void checkThecurrentQuest()
    {
        // Get current player quest
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();

        if (currentQuest is GoToFinshAllTheWave)
        {
            if (!isQuestIsGoToFinshAllTheWave)
            {
                // Quest just started - hide hologram and mark as active
                setSphereHologramOut(false);
                isQuestIsGoToFinshAllTheWave = true;
            }
        }
    }

    /// <summary>
    /// Checks if the GoToFinshAllTheWave quest has been completed.
    /// Verifies quest completion status with the quest manager.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToFinshAllTheWave)))
        {
            return true;
        }
        return false;
    }
    #endregion
}
