using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for story-based quests that contain child quests.
/// Manages quest completion events and child quest progression.
/// </summary>
[System.Serializable]
public abstract class StoryQuest : Quest
{
    #region Events

    /// <summary>
    /// Static event that is triggered when any story quest is completed.
    /// </summary>
    public static event Action onCompleted;

    #endregion

    #region Serialized Fields

    [SerializeField]
    protected List<Quest> childQuests;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the StoryQuest class.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public StoryQuest(Quest quest)
        : base(quest) { }

    #endregion

    #region Event Management

    /// <summary>
    /// Subscribes an action to the quest completion event.
    /// </summary>
    /// <param name="action">The action to subscribe to the event.</param>
    public static void subscribeToQuestCompletion(Action action)
    {
        onCompleted = action;
    }

    /// <summary>
    /// Unsubscribes an action from the quest completion event.
    /// </summary>
    /// <param name="action">The action to unsubscribe from the event.</param>
    public static void UnsubscribeFromQuestCompletion(Action action)
    {
        onCompleted -= action;
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Completes the story quest and triggers the completion event.
    /// </summary>
    public virtual void CompleteQuest()
    {
        this.completed = true;

        onCompleted?.Invoke();
    }

    /// <summary>
    /// Gets the list of child quests associated with this story quest.
    /// </summary>
    /// <returns>The list of child quests.</returns>
    public List<Quest> GetChildQuests()
    {
        return childQuests;
    }

    /// <summary>
    /// Sets the list of child quests for this story quest.
    /// </summary>
    /// <param name="childQuests">The list of child quests to set.</param>
    public void SetChildQuests(List<Quest> childQuests)
    {
        this.childQuests = childQuests;
    }

    #endregion

    #region Quest Progress

    /// <summary>
    /// Overrides the base progress method to handle story quest specific logic.
    /// </summary>
    /// <param name="expReward">Output parameter for experience reward.</param>
    public override void progress(out int expReward)
    {
        expReward = 0;
        return;
    }

    /// <summary>
    /// Progresses child find quests based on the found object.
    /// </summary>
    /// <param name="objectFound">The tag of the object that was found.</param>
    /// <param name="expReward">Output parameter for accumulated experience reward.</param>
    public void ProgressChildFindQuests(GameObject objectFound, out int expReward)
    {
        expReward = 0;

        if (childQuests == null || childQuests.Count == 0)
        {
            return;
        }

        FindQuest storyQuest = (FindQuest)
            this.childQuests.Find(quest =>
                quest is FindQuest && string.Join(", ", quest.QuestTarget).Contains(objectFound.tag)
            );
        if (storyQuest != null)
        {
            storyQuest.progress(out int questReward);
            objectFound.SetActive(false);
            if (storyQuest.isCompleted)
            {
                if (storyQuest.RewardType == RewardType.XP)
                {
                    expReward += questReward;
                }
            }
        }
    }

    /// <summary>
    /// Progresses child kill quests based on the killed object.
    /// </summary>
    /// <param name="objectFound">The tag of the object that was killed.</param>
    /// <param name="expReward">Output parameter for accumulated experience reward.</param>
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

    /// <summary>
    /// Gets the progress string for the story quest.
    /// </summary>
    /// <returns>An empty string as story quests don't have traditional progress.</returns>
    public override string GetProgress()
    {
        return "";
    }

    #endregion
}
