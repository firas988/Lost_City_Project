using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the TempleKillAllGaurds quest, tracking guard zone completion and force field deactivation.
/// Coordinates with enemy spawners and quest completion logic.
/// Handles quest progression, enemy zone monitoring, and reward distribution.
/// </summary>
public class TempleKillAllGaurdsQuest : MonoBehaviour
{
    #region Serialized Fields
    [Header("Quest Zone Configuration")]
    /// <summary>
    /// List of guard zone GameObjects that contain enemy spawners.
    /// Each zone must be cleared of enemies to complete the quest.
    /// </summary>
    [SerializeField]
    private List<GameObject> gaurdZones;

    [Header("Quest Reward Elements")]
    /// <summary>
    /// List of force field hologram GameObjects that block temple access.
    /// These are deactivated when the quest is completed.
    /// </summary>
    [SerializeField]
    private List<GameObject> forceFields;
    #endregion

    #region Private Fields
    [Header("Quest State Management")]
    /// <summary>
    /// Flag indicating whether the quest has been completed.
    /// Prevents repeated quest completion processing.
    /// </summary>
    private bool isCompleted;

    [Header("System References")]
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to the player character for quest progress tracking.
    /// Used to check current main quest and trigger completion.
    /// </summary>
    private Player player;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the temple quest handler and starts quest completion checking.
    /// Sets up component references and begins monitoring quest progress.
    /// </summary>
    void Start()
    {
        // Find and store references to required components
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Initialize quest state and start monitoring
        isCompleted = false;
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    /// <summary>
    /// Updates the temple quest handler logic each frame.
    /// Monitors quest completion conditions and triggers rewards when met.
    /// </summary>
    void Update()
    {
        // Skip processing if quest is already completed
        if (isCompleted)
        {
            return;
        }

        // Check if player has the quest and all guard zones are cleared
        if (
            (
                player.getCurrentMainQuest() is TempleKillAllGaurds
                && gaurdZones.All(gaurdZone =>
                    gaurdZone.GetComponent<Enemyspawner>().getAllEnemiesDead()
                )
            )
        )
        {
            // Quest completed - deactivate force fields and mark quest complete
            deactivateHolograms();
            player.getCurrentMainQuest().CompleteQuest();
            isCompleted = true;
        }
    }
    #endregion

    #region Quest Management Methods
    /// <summary>
    /// Deactivates all force field holograms when the quest is completed.
    /// Removes barriers blocking temple access as a reward for quest completion.
    /// </summary>
    public void deactivateHolograms()
    {
        // Disable all force field holograms to grant temple access
        foreach (GameObject forceField in forceFields)
        {
            forceField.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine that waits for the quest manager to be ready before checking quest completion.
    /// Handles both initial quest state and ongoing progress monitoring.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if quest was already completed previously
        if (questManager.checkingCompletedStoryQuest(typeof(TempleKillAllGaurds)))
        {
            // Quest already completed - enable multiple respawns and deactivate force fields
            foreach (GameObject gaurdZone in gaurdZones)
            {
                gaurdZone.GetComponent<Enemyspawner>().setCanMultipleRespawn(true);
            }
            deactivateHolograms();
            isCompleted = true;
        }
        else if (
            (
                player.getCurrentMainQuest() is TempleKillAllGaurds
                && gaurdZones.All(gaurdZone =>
                    gaurdZone.GetComponent<Enemyspawner>().getAllEnemiesDead()
                )
            )
        )
        {
            // Quest completed during this session - deactivate force fields and mark complete
            deactivateHolograms();
            player.getCurrentMainQuest().CompleteQuest();
            isCompleted = true;
        }
    }
    #endregion
}
