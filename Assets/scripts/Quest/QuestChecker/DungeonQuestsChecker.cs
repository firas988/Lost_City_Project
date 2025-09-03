using System.Collections;
using UnityEngine;

/// <summary>
/// Checks dungeon quest completion status and manages dungeon progression accordingly.
/// Handles scene transitions and cutscene visibility based on quest completion.
/// </summary>
public class DungeonQuestsChecker : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Cutscene GameObject to control when entering the dungeon.
    /// </summary>
    [SerializeField]
    private GameObject enterCutScene;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the quest manager for checking quest completion status.
    /// </summary>
    private QuestManager questManager;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the quest checker and starts the quest completion checking coroutine.
    /// </summary>
    void Awake()
    {
        // Find the quest manager component
        questManager = GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponentInChildren<QuestManager>();

        // Start checking quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    #endregion

    #region Quest Completion Checking

    /// <summary>
    /// Coroutine that waits for the quest manager to be ready and checks quest completion status.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for the quest manager to be ready
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if the final dungeon level quest is completed
        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevelFinal)))
        {
            // Load the next scene (scene index 4)
            GameObject
                .FindGameObjectWithTag("GameManager")
                .GetComponentInChildren<SceneHandler>()
                .LoadScene(4);
        }

        // Check if the first dungeon level quest is completed
        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevel1)))
        {
            // Hide the enter cutscene and progress to the next room
            enterCutScene.SetActive(false);
            GetComponent<DungeonManager>().NextRoom();
        }

        // Check if the second dungeon level quest is completed
        if (questManager.checkingCompletedStoryQuest(typeof(DungeonLevel2)))
        {
            // Hide the enter cutscene and progress to the next room
            enterCutScene.SetActive(false);
            GetComponent<DungeonManager>().NextRoom();
        }
    }

    #endregion
}
