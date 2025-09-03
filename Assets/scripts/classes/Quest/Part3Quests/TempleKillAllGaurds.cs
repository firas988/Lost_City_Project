using UnityEngine;

/// <summary>
/// Story quest that requires killing all temple guards.
/// Inherits basic quest completion behavior from the base class.
/// </summary>
[CreateAssetMenu(fileName = "TempleKillAllGaurds", menuName = "Quests/TempleKillAllGaurds")]
public class TempleKillAllGaurds : StoryQuest
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the TempleKillAllGaurds class.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public TempleKillAllGaurds(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the quest using the base class implementation.
    /// </summary>
    public override void CompleteQuest()
    {
        base.CompleteQuest();
    }

    #endregion
}
