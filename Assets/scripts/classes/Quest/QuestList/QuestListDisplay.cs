using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of multiple quests in a scrollable list.
/// Handles quest addition, removal, and progress updates with automatic layout management.
/// </summary>
public class QuestListDisplay : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Prefab for individual quest listing items.
    /// </summary>
    [SerializeField]
    private GameObject questPrefab;

    #endregion

    #region Private Fields

    /// <summary>
    /// Dictionary mapping quest IDs to their QuestListing components.
    /// </summary>
    private Dictionary<int, QuestListing> quests;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the quest list display by creating the quests dictionary.
    /// </summary>
    void Awake()
    {
        quests = new Dictionary<int, QuestListing>();
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Adds a new quest to the display list.
    /// </summary>
    /// <param name="questId">Unique identifier for the quest.</param>
    /// <param name="quest">The quest data to display.</param>
    public void addQuest(int questId, Quest quest)
    {
        // Instantiate the quest listing prefab
        GameObject questListing = Instantiate(questPrefab, this.gameObject.transform, false);

        // Add to the quests dictionary
        quests.Add(questId, questListing.GetComponent<QuestListing>());
        quests[questId].SetQuestToAdd(quest);

        // Rebuild layout to accommodate the new quest
        LayoutRebuilder.ForceRebuildLayoutImmediate(questListing.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    /// <summary>
    /// Removes a quest from the display list.
    /// </summary>
    /// <param name="questId">Unique identifier for the quest to remove.</param>
    public void removeQuest(int questId)
    {
        // Destroy the quest listing GameObject
        Destroy(quests[questId].gameObject);

        // Remove from the quests dictionary
        quests.Remove(questId);

        // Rebuild layout after removal
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    /// <summary>
    /// Updates the progress display for a specific quest.
    /// </summary>
    /// <param name="questId">Unique identifier for the quest to update.</param>
    /// <param name="progress">The new progress string to display.</param>
    public void updateQuestProgress(int questId, string progress)
    {
        // Update the quest progress
        quests[questId].SetProgress(progress);

        // Rebuild layout to accommodate progress changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(quests[questId].GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    #endregion
}
