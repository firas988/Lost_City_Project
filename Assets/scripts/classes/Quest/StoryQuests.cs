using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that contains a collection of story quests.
/// Provides indexed access and count information for story quest management.
/// </summary>
[CreateAssetMenu(fileName = "StoryQuests", menuName = "Quests/StoryQuests")]
public class StoryQuests : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// List of all story quests in this collection.
    /// </summary>
    [SerializeField]
    private List<StoryQuest> quests;

    #endregion

    #region Public Properties

    /// <summary>
    /// Indexer to access story quests by index.
    /// </summary>
    /// <param name="index">The index of the quest to retrieve.</param>
    /// <returns>The story quest at the specified index.</returns>
    public StoryQuest this[int index] => quests[index];

    /// <summary>
    /// Gets the total number of story quests in this collection.
    /// </summary>
    public int Count => quests.Count;

    /// <summary>
    /// Gets the list of all story quests in this collection.
    /// </summary>
    public List<StoryQuest> Quests => quests;

    #endregion
}
