using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    [SerializeField]
    private int storyQuestIndex;

    [SerializeField]
    private List<int> activeQuestIds;

    public QuestData(QuestManager questManager)
    {
        storyQuestIndex = questManager.StoryQuestIndex;
        activeQuestIds = questManager
            .PlayerInstance.ActiveSideQuests.Select(quest => quest.QuestId)
            .ToList();
    }

    public int StoryQuestIndex => storyQuestIndex;
    public List<int> ActiveQuestIds => activeQuestIds;
}
