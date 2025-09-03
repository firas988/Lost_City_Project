using System.Linq;
using UnityEngine;

/// <summary>
/// Story quest for the first level of the dungeon.
/// Automatically progresses to the next room when completed.
/// </summary>
[CreateAssetMenu(fileName = "DungeonLevel1", menuName = "Quests/DungeonLevel1")]
public class DungeonLevel1 : StoryQuest
{
    #region Constructors

    /// <summary>
    /// Initializes a new DungeonLevel1 quest.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public DungeonLevel1(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the dungeon level quest and progresses to the next room.
    /// </summary>
    public override void CompleteQuest()
    {
        // Prevent multiple completions
        if (this.isCompleted)
            return;

        // Progress to the next room in the dungeon
        GameObject.FindAnyObjectByType<DungeonManager>().NextRoom();

        // Complete the base quest
        base.CompleteQuest();
    }

    #endregion
}
