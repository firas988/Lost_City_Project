using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StoryQuest : Quest
{
    public static event Action onCompleted;

    [SerializeField]
    protected List<Quest> childQuests;

    public StoryQuest(Quest quest)
        : base(quest) { }

    public static void subscribeToQuestCompletion(Action action)
    {
        onCompleted = action;
    }

    public static void UnsubscribeFromQuestCompletion(Action action)
    {
        onCompleted -= action;
    }

    public virtual void CompleteQuest()
    {
        this.completed = true;


        onCompleted?.Invoke();
    }

    public List<Quest> GetChildQuests()
    {
        return childQuests;
    }

    public void SetChildQuests(List<Quest> childQuests)
    {
        this.childQuests = childQuests;
    }

    public void ProgressChildFindQuests(string objectFound, out int expReward)
    {
        expReward = 0;

        if (childQuests == null || childQuests.Count == 0)
        {
            return;
        }

        FindQuest storyQuest = (FindQuest)
            this.childQuests.Find(quest =>
                quest is FindQuest && string.Join(", ", quest.QuestTarget).Contains(objectFound)
            );
        if (storyQuest != null)
        {
            storyQuest.progress(out int questReward);
            if (storyQuest.isCompleted)
            {
                if (storyQuest.RewardType == RewardType.XP)
                {
                    expReward += questReward;
                }
            }
        }
    }

    public void ProgressChildKillQuests(string objectFound, out int expReward)
    {
        expReward = 0;

        if (childQuests == null || childQuests.Count == 0)
        {
            return;
        }

        List<KillQuest> questToInc = new List<KillQuest>();

        foreach (Quest quest in childQuests)
        {
            if (quest is KillQuest && string.Join(", ", quest.QuestTarget).Contains(objectFound))
            {
                questToInc.Add((KillQuest)quest);
            }
        }

        if (questToInc.Count > 0)
        {
            foreach (KillQuest quest in questToInc)
            {
                quest.progress(out int questReward);
                if (quest.isCompleted)
                {
                    if (quest.RewardType == RewardType.XP)
                    {
                        expReward += questReward;
                    }
                }
            }
        }
    }

    public override string GetProgress()
    {
        return "";
    }
}
