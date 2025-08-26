using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the special crystal item that opens the middle area of the game.
/// Handles quest progression, crystal movement, and hologram visibility based on quest states.
/// Coordinates with quest system to unlock progression and provide visual feedback.
/// </summary>
public class ItemToFindTopenTheMiddel_Hnadler : MonoBehaviour
{
    #region Visual Elements
    /// <summary>
    /// The crystal GameObject that moves upward when activated.
    /// Represents the key item needed to open the middle area.
    /// </summary>
    [SerializeField]
    private GameObject crystal;

    /// <summary>
    /// Hologram GameObject that provides visual guidance to the player.
    /// Hidden when quests are completed or activated.
    /// </summary>
    [SerializeField]
    private GameObject hologram;
    #endregion

    #region Component References
    /// <summary>
    /// Reference to the quest manager for checking quest completion status.
    /// Used to coordinate quest progression and completion.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to the object interaction component for managing interaction states.
    /// Controls canvas visibility and interaction completion status.
    /// </summary>
    private ObjectInteraction objectInteraction;
    #endregion

    #region Movement State
    /// <summary>
    /// Indicates whether the crystal is currently moving upward.
    /// Prevents multiple movement attempts while animation is in progress.
    /// </summary>
    private bool isMovingUp = false;

    /// <summary>
    /// The target position for the crystal's upward movement.
    /// Calculated as 15 units above the current position.
    /// </summary>
    private Vector3 targetPosition;
    #endregion

    #region Quest State Variables
    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Indicates if the "Go To Activate The Key" quest is currently active.
    /// Controls trigger behavior and quest progression.
    /// </summary>
    private bool isQuestIsGoToActivateTheKey = false;

    /// <summary>
    /// Indicates if the "Activate The Key" quest is currently active.
    /// Controls crystal activation and movement.
    /// </summary>
    private bool isQuestIsActivateTheKey = false;

    /// <summary>
    /// Indicates if the overall quest sequence has been completed.
    /// Prevents further quest processing once complete.
    /// </summary>
    private bool isQuestIsCompleted = false;

    /// <summary>
    /// Reference to the current main quest from the player.
    /// Used to check quest type and completion status.
    /// </summary>
    private Quest currentQuest;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by setting up references and starting quest monitoring.
    /// Hides the crystal initially and begins quest state checking.
    /// </summary>
    void Start()
    {
        // Hide crystal until quest is activated
        crystal.SetActive(false);

        // Find quest manager for quest coordination
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Get object interaction component for state management
        objectInteraction = GetComponent<ObjectInteraction>();

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    /// <summary>
    /// Updates quest states and handles crystal movement each frame.
    /// Manages quest progression and crystal animation when appropriate.
    /// </summary>
    void Update()
    {
        // Skip processing if quest is already completed
        if (isQuestIsCompleted)
        {
            return;
        }

        // Handle crystal movement animation
        moveUp();

        // Check quest state changes
        checkIfTheQuestIsGoToActivateTheKey();
        checkIfTheQuestIsActivateTheKey();

        // Skip further processing if no current quest
        if (currentQuest == null)
        {
            return;
        }
    }
    #endregion

    #region Crystal Movement
    /// <summary>
    /// Handles the upward movement animation of the crystal.
    /// Uses smooth lerping to move the crystal to its target position.
    /// Completes quest when movement finishes if conditions are met.
    /// </summary>
    public void moveUp()
    {
        if (isMovingUp)
        {
            // Smoothly move crystal toward target position
            crystal.transform.position = Vector3.Lerp(
                crystal.transform.position,
                targetPosition,
                Time.deltaTime * 1
            );

            // Check if crystal has reached target position
            if (Vector3.Distance(crystal.transform.position, targetPosition) < 0.01f)
            {
                // Snap to exact target position and stop movement
                crystal.transform.position = targetPosition;
                isMovingUp = false;

                // Complete quest if all conditions are met
                if (
                    currentQuest is ActivateTheKey
                    && isQuestIsActivateTheKey
                    && !currentQuest.isCompleted
                )
                {
                    (currentQuest as ActivateTheKey).CompleteQuest();
                    isQuestIsActivateTheKey = false;
                }

                // Mark overall quest sequence as completed
                isQuestIsCompleted = true;
            }
        }
    }

    /// <summary>
    /// Activates the crystal and starts its upward movement.
    /// Only works when the "Activate The Key" quest is active and crystal isn't moving.
    /// </summary>
    public void foundIT()
    {
        // Only activate if quest is active and crystal isn't already moving
        if (!isQuestIsActivateTheKey || isMovingUp)
        {
            return;
        }

        // Show crystal and calculate target position
        crystal.SetActive(true);
        targetPosition = new Vector3(
            crystal.transform.position.x,
            crystal.transform.position.y + 15f,
            crystal.transform.position.z
        );

        // Start upward movement animation
        isMovingUp = true;
    }
    #endregion

    #region Quest Trigger Handling
    /// <summary>
    /// Handles player entering the trigger zone.
    /// Completes the "Go To Activate The Key" quest when player reaches the location.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered and quest is active but not completed
        if (other.CompareTag(playerTag) && isQuestIsGoToActivateTheKey && !currentQuest.isCompleted)
        {
            if (currentQuest is GoToActivateTheKey)
            {
                // Complete the quest and update state
                (currentQuest as GoToActivateTheKey).CompleteQuest();
                isQuestIsGoToActivateTheKey = false;
            }
        }
    }
    #endregion

    #region Quest State Checking
    /// <summary>
    /// Checks if the "Go To Activate The Key" quest has become active.
    /// Updates quest state and hides hologram when quest becomes active.
    /// </summary>
    public void checkIfTheQuestIsGoToActivateTheKey()
    {
        // Get current quest from player
        Player player = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();

        // Check if quest type matches and update state
        if (currentQuest is GoToActivateTheKey)
        {
            if (!isQuestIsGoToActivateTheKey)
            {
                isQuestIsGoToActivateTheKey = true;

                // Hide and destroy hologram when quest becomes active
                if (hologram != null)
                {
                    hologram.SetActive(false);
                    Destroy(hologram);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the "Activate The Key" quest has become active.
    /// Updates quest state and hides hologram when quest becomes active.
    /// </summary>
    public void checkIfTheQuestIsActivateTheKey()
    {
        // Get current quest from player
        Player player = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();

        // Check if quest type matches and update state
        if (currentQuest is ActivateTheKey)
        {
            if (!isQuestIsActivateTheKey)
            {
                isQuestIsActivateTheKey = true;
            }

            // Hide and destroy hologram when quest becomes active
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);
            }
        }
    }
    #endregion

    #region Quest Completion Monitoring
    /// <summary>
    /// Coroutine that waits for quest manager to be ready and then checks quest completion.
    /// Ensures quest system is fully initialized before monitoring quest states.
    /// </summary>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait until quest manager is ready to start quests
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check completion status of both quest types
        checkIfTheQuestIsGoToActivateTheKeyIsCompleted();
        checkIfTheQuestIsActivateTheKeyIsCompleted();
    }

    /// <summary>
    /// Checks if the "Go To Activate The Key" quest has been completed.
    /// Hides hologram when quest is completed.
    /// </summary>
    /// <returns>True if quest is completed, false otherwise.</returns>
    public bool checkIfTheQuestIsGoToActivateTheKeyIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToActivateTheKey)))
        {
            // Hide and destroy hologram when quest is completed
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the "Activate The Key" quest has been completed.
    /// Activates crystal, updates interaction state, and hides UI when quest is completed.
    /// </summary>
    /// <returns>True if quest is completed, false otherwise.</returns>
    public bool checkIfTheQuestIsActivateTheKeyIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(ActivateTheKey)))
        {
            // Hide and destroy hologram when quest is completed
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);

                // Activate crystal and update interaction state
                isQuestIsActivateTheKey = true;
                foundIT();
                objectInteraction.setIsFinshed(true);
                objectInteraction.hideCanvas();
            }
            return true;
        }
        return false;
    }
    #endregion
}
