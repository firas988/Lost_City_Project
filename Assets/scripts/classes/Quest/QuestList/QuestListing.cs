using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestListing : MonoBehaviour
{
    [SerializeField]
    private int questId;

    private TMP_Text questName;
    private TMP_Text questDescription;
    private TMP_Text questProgress;

    private Quest questToAdd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        questName = transform.Find("Details").Find("Name").GetComponent<TMP_Text>();
        questDescription = transform.Find("Details").Find("Description").GetComponent<TMP_Text>();

        Transform progress = transform.Find("Progress");
        if (progress != null)
        {
            questProgress = progress.GetComponentInChildren<TMP_Text>();
            questProgress.text = "0/" + questToAdd?.QuestTarget.Count;
        }

        if (questToAdd != null)
        {
            questName.text = questToAdd.GetQuestName();
            questDescription.text = questToAdd.GetDescription();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    public void SetQuestToAdd(Quest quest)
    {
        questToAdd = quest;
    }

    public void SetQuestId(int id)
    {
        gameObject.name = id.ToString();
    }

    public void SetName(string name)
    {
        questName.text = name;
    }

    public void SetDescription(string description)
    {
        questDescription.text = description;
    }

    public void SetProgress(string progress)
    {
        questProgress.text = progress;
    }
}
