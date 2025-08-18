using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryQuests", menuName = "Quests/StoryQuests")]
public class StoryQuests : ScriptableObject
{
    [SerializeField]
    private List<StoryQuest> quests;

    public StoryQuest this[int index] => quests[index];

    public int Count => quests.Count;

    public List<StoryQuest> Quests => quests;
}
