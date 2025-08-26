using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Quest for finding the mysterious artifact in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "MysteriousArtifact",
    menuName = "Quests/Part2/MysteriousMan/MysteriousArtifact"
)]
public class MysteriousArtifact : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the MysteriousArtifact quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public MysteriousArtifact(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when the artifact is found
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the quest by updating dialogue and setting up the next quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Get the player instance for quest management
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();

        // Get dialogue conversion system for NPC interactions
        ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();

        // Load the dialogue data for when artifact is found
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "MysteriousManFoundArtifact"
        );

        // Set the new dialogue for the Mysterious Man NPC
        (
            (TalkativeNpc)
                GameObject.Find("MysteriousMan").GetComponent<StartNpc>().GetNpcsInstance()
        ).setDialogue(dialogueData);

        // Complete the base quest
        base.CompleteQuest();

        // Set up the next quest for the Mysterious Man to give to the player
        GameObject mysteriousMan = GameObject.Find("MysteriousMan");
        QuestGiver questGiver = (QuestGiver)
            mysteriousMan.GetComponent<StartNpc>().GetNpcsInstance();
        questGiver.setQuestToGive(player.getCurrentMainQuest(), mysteriousMan);
    }
    #endregion
}
