using UnityEngine;

/// <summary>
/// Quest for guiding the player to the location where they can fight enemy waves in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "GoToFinshAllTheWave",
    menuName = "Quests/Part2/KillWave/GoToFinshAllTheWave"
)]
public class GoToFinshAllTheWave : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the GoToFinshAllTheWave quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public GoToFinshAllTheWave(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the GoToFinshAllTheWave quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
