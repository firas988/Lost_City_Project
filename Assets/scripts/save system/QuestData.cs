using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Serializable data structure for storing quest progression and state information.
/// Captures main story quest index and active side quest identifiers.
/// Used by the save system to persist quest progress across game sessions.
/// </summary>
[System.Serializable]
public class QuestData
{
    #region Quest Progress Data
    /// <summary>
    /// Index of the current main story quest in the quest progression.
    /// Represents the player's position in the main storyline.
    /// </summary>
    [SerializeField]
    private int storyQuestIndex;

    /// <summary>
    /// List of active side quest identifiers currently being pursued.
    /// Used to restore side quest states when loading the game.
    /// </summary>
    [SerializeField]
    private List<int> activeQuestIds;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new QuestData instance by extracting data from a QuestManager.
    /// Captures current story quest progress and active side quests.
    /// </summary>
    /// <param name="questManager">The QuestManager component to extract data from.</param>
    public QuestData(QuestManager questManager)
    {
        // Extract main story quest progress
        storyQuestIndex = questManager.StoryQuestIndex;

        // Extract active side quest identifiers using LINQ
        activeQuestIds = questManager
            .PlayerInstance.ActiveSideQuests.Select(quest => quest.QuestId)
            .ToList();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the current story quest index.
    /// </summary>
    public int StoryQuestIndex => storyQuestIndex;

    /// <summary>
    /// Gets the list of active side quest identifiers.
    /// </summary>
    public List<int> ActiveQuestIds => activeQuestIds;
    #endregion
}
