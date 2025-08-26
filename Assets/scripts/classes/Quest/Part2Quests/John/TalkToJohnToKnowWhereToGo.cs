using UnityEngine;

/// <summary>
/// Quest for talking to John to learn where to go next in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "TalkToJohnToKnowWhereToGo",
    menuName = "Quests/Part2/John/TalkToJohnToKnowWhereToGo"
)]
public class TalkToJohnToKnowWhereToGo : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the TalkToJohnToKnowWhereToGo quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public TalkToJohnToKnowWhereToGo(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when talking to John for directions
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the TalkToJohnToKnowWhereToGo quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
