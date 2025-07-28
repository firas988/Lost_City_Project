using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "StoryQuests", menuName = "Quests/StoryQuests")]
public abstract class StoryQuests : ScriptableObject
{
    [SerializeField]
    private List<StoryQuest> quests;


    public List<StoryQuest> GetQuests()
    {
        return quests;
    }
}
