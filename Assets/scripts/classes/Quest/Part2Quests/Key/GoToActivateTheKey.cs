using UnityEngine;

/// <summary>
/// Quest for guiding the player to the location where they can activate the key in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "GoToActivateTheKey", menuName = "Quests/Part2/Key/GoToActivateTheKey")]
public class GoToActivateTheKey : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the GoToActivateTheKey quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public GoToActivateTheKey(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the GoToActivateTheKey quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
