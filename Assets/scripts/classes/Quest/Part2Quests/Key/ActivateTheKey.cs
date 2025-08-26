using UnityEngine;

/// <summary>
/// Quest for activating a key item in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "ActivateTheKey", menuName = "Quests/Part2/Key/ActivateTheKey")]
public class ActivateTheKey : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the ActivateTheKey quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public ActivateTheKey(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when the key is activated
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the ActivateTheKey quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
