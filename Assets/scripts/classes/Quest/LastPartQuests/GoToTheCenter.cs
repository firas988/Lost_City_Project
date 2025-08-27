using UnityEngine;

/// <summary>
/// Quest for guiding the player to the center location in the final part of the story
/// </summary>
[CreateAssetMenu(fileName = "GoToTheCenter", menuName = "Quests/LastPart/GoToTheCenter")]
public class GoToTheCenter : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the GoToTheCenter quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public GoToTheCenter(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the GoToTheCenter quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
