using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest for talking to John to receive a weapon reward in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "TalkToJohnToGetWeapon",
    menuName = "Quests/Part2/John/TalkToJohnToGetWeapon"
)]
public class TalkToJohnToGetWeapon : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the TalkToJohnToGetWeapon quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public TalkToJohnToGetWeapon(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when talking to John
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the quest by giving rewards and setting up the next quest
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

        // Load the dialogue data for the next quest conversation
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "TalkToJohnToKnowWhereToGo"
        );

        // Set the new dialogue for John NPC
        (
            (TalkativeNpc)GameObject.FindWithTag("John").GetComponent<StartNpc>().GetNpcsInstance()
        ).setDialogue(dialogueData);

        // Get reward manager and give the quest reward
        RewardManager rewardManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<RewardManager>();
        rewardManager.GiveReward(base.Reward);

        // Complete the base quest
        base.CompleteQuest();

        // Set up the next quest for John to give to the player
        GameObject john = GameObject.FindWithTag("John");
        QuestGiver questGiver = (QuestGiver)john.GetComponent<StartNpc>().GetNpcsInstance();
        questGiver.setQuestToGive(player.getCurrentMainQuest(), john);
    }
    #endregion
}
