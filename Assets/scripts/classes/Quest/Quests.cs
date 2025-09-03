using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Base ScriptableObject class representing a collection of quests.
/// Provides functionality for managing and randomly selecting quests from the collection.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewQuestList", menuName = "Quests/ExpQuests/QuestList")]
public class Quests : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// List of quests stored in this collection.
    /// </summary>
    [SerializeField]
    protected List<Quest> quests;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the list of all quests in this collection.
    /// </summary>
    /// <returns>List of all quests in this collection.</returns>
    public List<Quest> allquests => quests;

    /// <summary>
    /// Gets a random quest from the list of quests.
    /// </summary>
    /// <returns>A randomly selected quest from the list.</returns>
    public Quest RandomQuest => quests[Random.Range(0, quests.Count)];

    #endregion
}
