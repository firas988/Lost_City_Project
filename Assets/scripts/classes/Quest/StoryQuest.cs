using System;
using UnityEngine;

public abstract class StoryQuest : Quest
{
    public static event Action onCompleted;
   

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
        onCompleted?.Invoke();
    }

}
