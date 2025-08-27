using UnityEngine;

/// <summary>
/// Quest for guiding the player to the bridge location in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "GoToBridge", menuName = "Quests/Part2/Bridge/GoToBridge")]
public class GoToBridge : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the GoToBridge quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public GoToBridge(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the GoToBridge quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
