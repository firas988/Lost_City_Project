using UnityEngine;

/// <summary>
/// Quest for defeating the final boss in the last part of the story
/// </summary>
[CreateAssetMenu(fileName = "KillTheFinalBoss", menuName = "Quests/LastPart/KillTheFinalBoss")]
public class KillTheFinalBoss : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the KillTheFinalBoss quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public KillTheFinalBoss(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the KillTheFinalBoss quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
