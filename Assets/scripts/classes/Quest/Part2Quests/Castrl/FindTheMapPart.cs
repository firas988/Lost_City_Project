using UnityEngine;

/// <summary>
/// Quest for finding a map part within the castle in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "FindTheMapPart", menuName = "Quests/Part2/Castel/FindTheMapPart")]
public class FindTheMapPart : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the FindTheMapPart quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public FindTheMapPart(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when the map part is found/collected
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the FindTheMapPart quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
