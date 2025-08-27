using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest for the Mysterious Man when the artifact has not been found in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "MysteriousManQuest",
    menuName = "Quests/Part2/MysteriousMan/MysteriousManQuest"
)]
public class MysteriousManQuest : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the MysteriousManQuest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public MysteriousManQuest(Quest quest)
        : base(quest) { }
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

        // Load the dialogue data for when artifact is not found
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "MysteriousManDidntFindArtifact"
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
