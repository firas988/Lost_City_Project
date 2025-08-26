using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest for the Mysterious Man when the artifact has been found in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "MysteriousManQuestFoundArtifact",
    menuName = "Quests/Part2/MysteriousMan/MysteriousManQuestFoundArtifact"
)]
public class MysteriousManQuestFoundArtifact : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the MysteriousManQuestFoundArtifact quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public MysteriousManQuestFoundArtifact(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when talking to the Mysterious Man
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the quest by updating dialogue, setting up the next quest, and removing map obstacles
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

        // Load the dialogue data for where to go next
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "MysteriousManWhereToGo"
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

        // Remove the map piece that was blocking progress
        Destroy(GameObject.FindWithTag("MysteriousManMapPiece"));
    }
    #endregion
}
