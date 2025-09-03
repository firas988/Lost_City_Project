using System.Linq;
using UnityEngine;

/// <summary>
/// Story quest that requires all child quests to be completed before marking as complete.
/// Manages quest completion logic for kill-based objectives.
/// </summary>
[CreateAssetMenu(fileName = "KillEnemies", menuName = "Quests/KillEnemies")]
public class KillEnemies : StoryQuest
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the KillEnemies class.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public KillEnemies(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the quest only if all child quests are completed.
    /// </summary>
    public override void CompleteQuest()
    {
        if (this.childQuests.All(quest => quest.isCompleted))
        {
            return;
        }

        base.CompleteQuest();
    }

    #endregion
}
