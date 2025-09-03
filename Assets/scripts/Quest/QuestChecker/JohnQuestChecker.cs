using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks and manages quest completion states for the John quest chain.
/// Updates dialogue and quest availability based on quest progression.
/// Manages the progression through TalkToJohnToGetWeapon, TalkToJohnToKnowWhereToGo, and TimeToGetTheItem quests.
/// </summary>
public class JohnQuestChecker : MonoBehaviour
{
    #region Private Fields
    [Header("System References")]
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
    /// Initializes the quest checker and starts quest completion checking.
    /// Sets up component references and begins monitoring quest progress.
    /// </summary>
    private void Awake()
    {
        // Find and store reference to the quest manager
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        // Start monitoring quest completion status
        StartCoroutine(checkIfTheQuestIsCompleted());
    }
    #endregion

    #region Quest Completion Checking Methods
    /// <summary>
    /// Coroutine that waits for the quest manager to be ready before checking quest completion.
    /// Manages the progression through the John quest chain based on completion status.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if the first quest in the chain is completed
        if (checkIfTheQuestIsTalkToJohnToGetWeaponCompleted())
        {
            // Get current player and dialogue converter references
            Player player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
            ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();

            // Check if the second quest is completed
            if (checkIfTheQuestIsTalkToJohnToKnowWhereToGoCompleted())
            {
                // Check if the final quest is completed
                if (checkIfTheQuestIsTimeToGetTheItemCompleted())
                {
                    // All quests completed - destroy this checker
                    Destroy(this.gameObject);
                    yield break;
                }
                else
                {
                    // Set up dialogue for the final quest
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "TalkToJohnToKnowWhereToGo"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );

                    // Set quest giver to give no quest (final quest is active)
                    QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();
                    questGiver.setQuestToGive(null, this.gameObject);
                }
            }
            else
            {
                // Set up dialogue for the second quest
                Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                    "TalkToJohnToKnowWhereToGo"
                );
                ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                    dialogueData
                );

                // Set quest giver to give the second quest
                QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();
                questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
            }
        }
    }
    #endregion

    #region Quest State Checking Methods
    /// <summary>
    /// Checks if the TalkToJohnToGetWeapon quest has been completed.
    /// Verifies completion status of the first quest in the John quest chain.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsTalkToJohnToGetWeaponCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TalkToJohnToGetWeapon)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the TalkToJohnToKnowWhereToGo quest has been completed.
    /// Verifies completion status of the second quest in the John quest chain.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsTalkToJohnToKnowWhereToGoCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TalkToJohnToKnowWhereToGo)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the TimeToGetTheItem quest has been completed.
    /// Verifies completion status of the final quest in the John quest chain.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsTimeToGetTheItemCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TimeToGetTheItem)))
        {
            return true;
        }
        return false;
    }
    #endregion
}
