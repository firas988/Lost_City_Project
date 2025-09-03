using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks and manages quest completion states for the Mysterious Man quest chain.
/// Updates dialogue and quest availability based on quest progression.
/// Manages the progression through MysteriousManQuest, MysteriousArtifact, MysteriousManQuestFoundArtifact, and MysteriousManQuestWhereToGo quests.
/// </summary>
public class MysteriousManQuestChecker : MonoBehaviour
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
    /// Manages the progression through the Mysterious Man quest chain based on completion status.
    /// </summary>
    /// <returns>Coroutine for managing quest completion checking.</returns>
    private IEnumerator checkIfTheQuestIsCompleted()
    {
        // Wait for quest manager to be ready before proceeding
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        // Check if the first quest in the chain is completed
        if (checkIfTheQuestIsMysteriousManQuestCompleted())
        {
            // Get current player and dialogue converter references
            Player player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
            ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
            QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();

            // Check if the artifact quest is completed
            if (checkIfTheQuestIsMysteriousArtifactCompleted())
            {
                // Check if the found artifact quest is completed
                if (checkIfTheQuestIsMysteriousManQuestFoundArtifactCompleted())
                {
                    // Set up dialogue for the final quest direction
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "MysteriousManWhereToGo"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );

                    // Remove the map piece and check final quest completion
                    Destroy(GameObject.FindWithTag("MysteriousManMapPiece"));

                    if (checkIfTheQuestIsMysteriousManQuestWhereToGoCompleted())
                    {
                        // All quests completed - set quest giver to give no quest
                        questGiver.setQuestToGive(null, this.gameObject);
                    }
                    else
                    {
                        // Set quest giver to give the final quest
                        questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
                    }
                }
                else
                {
                    // Set up dialogue for the found artifact quest
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "MysteriousManFoundArtifact"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );

                    // Set quest giver to give the found artifact quest
                    questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
                }
            }
            else
            {
                // Set up dialogue for when artifact is not found
                Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                    "MysteriousManDidntFindArtifact"
                );
                ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                    dialogueData
                );

                // Set quest giver to give the artifact quest
                questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
            }
        }
    }
    #endregion

    #region Quest State Checking Methods
    /// <summary>
    /// Checks if the MysteriousManQuest has been completed.
    /// Verifies completion status of the first quest in the Mysterious Man quest chain.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsMysteriousManQuestCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuest)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the MysteriousArtifact quest has been completed.
    /// Verifies completion status of the artifact collection quest.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsMysteriousArtifactCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousArtifact)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the MysteriousManQuestFoundArtifact quest has been completed.
    /// Verifies completion status of the found artifact quest.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsMysteriousManQuestFoundArtifactCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuestFoundArtifact)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the MysteriousManQuestWhereToGo quest has been completed.
    /// Verifies completion status of the final direction quest.
    /// </summary>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    private bool checkIfTheQuestIsMysteriousManQuestWhereToGoCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuestWhereToGo)))
        {
            return true;
        }
        return false;
    }
    #endregion
}
