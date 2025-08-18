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
        onCompleted += action;
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
}
