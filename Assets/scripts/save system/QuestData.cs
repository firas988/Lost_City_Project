using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    [SerializeField]
    private int storyQuestIndex;
    public int StoryQuestIndex => storyQuestIndex;

    private List<int> activeQuestIds;
    public List<int> ActiveQuestIds => activeQuestIds;

    public QuestData(QuestManager questManager)
    {
        storyQuestIndex = questManager.StoryQuestIndex;
        activeQuestIds = questManager
            .PlayerInstance.ActiveSideQuests.Select(quest => quest.QuestId)
            .ToList();
        Debug.Log("QuestData: " + activeQuestIds.Count);
    }
}
