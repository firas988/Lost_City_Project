using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collection of KillQuest objects that can be randomly selected.
/// Inherits from the Quests class and provides a method to get a random KillQuest.
/// </summary>
[CreateAssetMenu(fileName = "NewQuestList" , menuName = "Quests/ExpQuests/ExpKillQuests")]
public class ExpKillQuests : Quests
{
    /// <summary>
    /// Gets a random KillQuest from the list of quests.
    /// </summary>
    /// <returns>A randomly selected KillQuest from the list.</returns>
   public new KillQuest RandomQuest =>  (KillQuest)quests[Random.Range(0,quests.Count)];
}
