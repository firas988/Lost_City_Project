using UnityEngine;

/// <summary>
/// Quest for eliminating all enemies within the castle in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "KillAllTheEnemyInTheCastel",
    menuName = "Quests/Part2/Castel/KillAllTheEnemyInTheCastel"
)]
public class KillAllTheEnemyInTheCastel : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the KillAllTheEnemyInTheCastel quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public KillAllTheEnemyInTheCastel(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is handled externally when all enemies are defeated
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the KillAllTheEnemyInTheCastel quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
