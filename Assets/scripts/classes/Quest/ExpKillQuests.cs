using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Represents a collection of KillQuest objects that can be randomly selected.
/// Inherits from the Quests class and provides a method to get a random KillQuest.
/// </summary>
[CreateAssetMenu(fileName = "NewQuestList", menuName = "Quests/ExpQuests/ExpKillQuests")]
public class ExpKillQuests : Quests
{
    #region Public Properties

    /// <summary>
    /// Gets a random KillQuest from the list of quests.
    /// </summary>
    /// <returns>A randomly selected KillQuest from the list.</returns>
    public new KillQuest RandomQuest => (KillQuest)quests[Random.Range(0, quests.Count)];

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds a KillQuest by its quest ID.
    /// </summary>
    /// <param name="questId">The ID of the quest to find.</param>
    /// <returns>The KillQuest with the specified ID, or null if not found.</returns>
    public KillQuest Find(int questId)
    {
        return (KillQuest)quests.Find(quest => quest.QuestId == questId);
    }

    #endregion
}
