using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestListDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject questPrefab;
    private Dictionary<int, QuestListing> quests;

    void Awake()
    {
        quests = new Dictionary<int, QuestListing>();
    }

    public void addQuest(int questId, Quest quest)
    {
        GameObject questListing = Instantiate(questPrefab, this.gameObject.transform, false);
        quests.Add(questId, questListing.GetComponent<QuestListing>());

        questListing.GetComponent<QuestListing>().SetName(quest.GetQuestName());
        questListing.GetComponent<QuestListing>().SetDescription(quest.GetDescription());
        questListing.GetComponent<QuestListing>().SetProgress(quest.GetProgress());
        questListing.GetComponent<QuestListing>().SetQuestId(questId);

        LayoutRebuilder.ForceRebuildLayoutImmediate(questListing.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void removeQuest(int questId)
    {
        Destroy(quests[questId].gameObject);
        quests.Remove(questId);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void updateQuestProgress(int questId, string progress)
    {
        quests[questId].SetProgress(progress);
        LayoutRebuilder.ForceRebuildLayoutImmediate(quests[questId].GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
