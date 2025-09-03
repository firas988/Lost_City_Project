using System.Linq;
using UnityEngine;

/// <summary>
/// Story quest for the final level of the dungeon.
/// Opens the dungeon exit when completed.
/// </summary>
[CreateAssetMenu(fileName = "DungeonLevelFinal", menuName = "Quests/DungeonLevelFinal")]
public class DungeonLevelFinal : StoryQuest
{
    #region Constructors

    /// <summary>
    /// Initializes a new DungeonLevelFinal quest.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public DungeonLevelFinal(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the final dungeon level quest and opens the dungeon exit.
    /// </summary>
    public override void CompleteQuest()
    {
        // Prevent multiple completions
        if (this.isCompleted)
            return;

        // Open the dungeon exit
        GameObject.FindAnyObjectByType<DungeonManager>().openDungeonExit();

        // Complete the base quest
        base.CompleteQuest();
    }

    #endregion
}
