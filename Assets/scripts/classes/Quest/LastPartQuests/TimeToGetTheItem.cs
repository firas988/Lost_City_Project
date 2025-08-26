using UnityEngine;

/// <summary>
/// Quest for collecting a crucial item in the final part of the story
/// </summary>
[CreateAssetMenu(fileName = "TimeToGetTheItem", menuName = "Quests/LastPart/TimeToGetTheItem")]
public class TimeToGetTheItem : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the TimeToGetTheItem quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public TimeToGetTheItem(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when the crucial item is collected
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the TimeToGetTheItem quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
