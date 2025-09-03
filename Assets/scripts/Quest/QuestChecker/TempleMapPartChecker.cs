using System.Collections;
using UnityEngine;

/// <summary>
/// Checks and manages quest completion states for the TempleFindMapPart quest.
/// Controls dungeon entrance door access based on quest completion.
/// Manages the transition from temple exploration to dungeon access.
/// </summary>
public class TempleMapPartChecker : MonoBehaviour
{
    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the dungeon entrance door GameObject.
    /// Controlled based on quest completion status.
    /// </summary>
    private GameObject dungeonDoor;

    /// <summary>
    /// Reference to the quest manager for quest state coordination.
    /// Used to check quest completion status and trigger rewards.
    /// </summary>
    QuestManager questManager;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the temple map part checker and starts quest completion checking.
    /// Sets up component references and begins monitoring quest progress.
    /// </summary>
    void Start()
    {
        // Find and store references to required components
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Find the dungeon entrance door for access control
        dungeonDoor = GameObject.Find("dungeonEntrance");

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
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

        // Check if the TempleFindMapPart quest has been completed
        if (questManager.checkingCompletedStoryQuest(typeof(TempleFindMapPart)))
        {
            // Quest completed - open both dungeon doors to grant access
            dungeonDoor.GetComponent<DungeonDoorAnimateControl>().openBothDoors();
        }
    }
    #endregion
}
