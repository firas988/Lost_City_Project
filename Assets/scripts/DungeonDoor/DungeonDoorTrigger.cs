using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers scene transition when the player enters the dungeon door area.
/// Checks if the player has the required quest before allowing dungeon access.
/// </summary>
public class DungeonDoorTrigger : MonoBehaviour
{
    [Header("Dungeon Access Control")]
    [Tooltip("If true, the dungeon will be accessible without any quest requirements.")]
    private bool canAccessDungeon = false;

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    QuestManager questManager;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    void Start()
    {
        // Find and store references to required components
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if the TempleFindMapPart quest has been completed and TimeToGetTheItem quest has not been completed
        if (questManager.checkingCompletedStoryQuest(typeof(TempleFindMapPart)))
        {
            if (questManager.checkingCompletedStoryQuest(typeof(TimeToGetTheItem)))
            {
                canAccessDungeon = false;
            }
            else
            {
                canAccessDungeon = true;
            }
        }
    }

    #region Unity Event Methods

    /// <summary>
    /// Handles player entry into the dungeon door trigger area.
    /// Loads the dungeon scene if the player has the required quest.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            // Get the player component to check quest status
            Player player = other.GetComponent<StartPlayer>().getPlayer();

            // Check if the player has the required dungeon level quest
            if (player.getCurrentMainQuest() is DungeonLevel1 || canAccessDungeon)
            {
                // Load the dungeon scene (scene index 5)
                GameObject
                    .FindWithTag("GameManager")
                    .GetComponentInChildren<SceneHandler>()
                    .LoadScene(5);
            }
        }
    }

    #endregion
}
