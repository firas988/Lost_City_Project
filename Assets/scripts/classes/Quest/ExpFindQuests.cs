using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collection of FindQuest objects that can be randomly selected.
/// Inherits from the Quests class and provides a method to get a random FindQuest.
/// </summary>
[CreateAssetMenu(fileName = "NewQuestList", menuName = "Quests/ExpQuests/ExpFindQuests")]
public class ExpFindQuests : Quests
{
    /// <summary>
    /// Gets a random FindQuest from the list of quests.
    /// </summary>
    /// <returns>A randomly selected FindQuest from the list.</returns>
    public new FindQuest RandomQuest => (FindQuest)quests[Random.Range(0, quests.Count)];
}
