using UnityEngine;

/// <summary>
/// Quest for guiding the player to the castle location in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "GoToCastel", menuName = "Quests/Part2/Castel/GoToCastel")]
public class GoToCastel : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the GoToCastel quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public GoToCastel(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the GoToCastel quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
