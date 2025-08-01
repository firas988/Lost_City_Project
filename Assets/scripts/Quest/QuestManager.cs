using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages quest-related functionality including story quests, active quests, and quest progress tracking.
/// Handles different types of quests (KillQuest, FindQuest) and provides quest completion events.
/// </summary>
public class QuestManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the player GameObject for quest management.
    /// </summary>
    [SerializeField]
    private GameObject player;

    /// <summary>
    /// Reference to the dialogue manager for quest integration.
    /// </summary>
    [SerializeField]
    private DialogueManager dialogueManager;

    /// <summary>
    /// Reference to the audio manager for playing quest-related sounds.
    /// </summary>
    [SerializeField]
    private AudioManager audioManager;

    /// <summary>
    /// List of story quests available in the game.
    /// </summary>
    [SerializeField]
    private List<StoryQuest> storyQuestsList;

    #endregion

    #region Private Fields

    /// <summary>
    /// Audio source for playing audio clips.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Queue containing story quests to be processed in order.
    /// </summary>
    private Queue<StoryQuest> storyQuestListQueue;

    /// <summary>
    /// List of currently active quests for the player.
    /// </summary>
    private List<Quest> activeQuests;

    /// <summary>
    /// List of active kill quests for efficient processing.
    /// </summary>
    private List<KillQuest> activeKillQuests;

    /// <summary>
    /// List of active find quests for efficient processing.
    /// </summary>
    private List<FindQuest> activeFindQuests;

    /// <summary>
    /// Reference to the player instance for quest management.
    /// </summary>
    private Player playerInstance;

    /// <summary>
    /// Reference to the notifications manager for displaying quest updates.
    /// </summary>
    private NotificationsManager notificationsManager;

    #endregion

    #region Events

    /// <summary>
    /// Event triggered when a quest is completed, providing the reward amount.
    /// </summary>
    public event Action<float> onQuestFinish;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes quest management system, sets up quest collections, and subscribes to dialogue events.
    /// </summary>
    private void Start()
    {
        notificationsManager = GameObject.Find("GameManger").GetComponent<NotificationsManager>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        initPlayer();
        subscribeToEvents();
        initQuestLists();
    }

    #endregion

    #region Initialization Methods

    /// <summary>
    /// Subscribes to quest-related events for proper integration.
    /// </summary>
    private void subscribeToEvents()
    {
        StoryQuest.subscribeToQuestCompletion(nextMainQuest);
        dialogueManager.onDialogueExit += addQuest;
    }

    /// <summary>
    /// Initializes quest lists and sets up the story quest queue.
    /// </summary>
    private void initQuestLists()
    {
        activeQuests = playerInstance.ActiveQuest;
        activeKillQuests = new List<KillQuest>().FindAll(quest =>
            quest != null && quest.GetType() == typeof(KillQuest)
        );
        activeFindQuests = new List<FindQuest>().FindAll(quest =>
            quest != null && quest.GetType() == typeof(FindQuest)
        );
        storyQuestListQueue = new Queue<StoryQuest>(storyQuestsList);

        foreach (StoryQuest quest in storyQuestsList)
        {
            storyQuestListQueue.Enqueue(quest);
        }

        if (storyQuestListQueue != null && storyQuestListQueue.Count > 0)
            playerInstance.setCurrentMainQuest(storyQuestListQueue.Dequeue());
    }

    /// <summary>
    /// Initializes the player instance reference for quest management.
    /// </summary>
    private void initPlayer()
    {
        playerInstance = player.GetComponent<StartPlayer>().getPlayer();
        if (playerInstance == null)
            Debug.LogError("Player instance is null");
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Adds a new quest to the player's active quest list and categorizes it by type.
    /// </summary>
    /// <param name="quest">The quest to be added to the player's active quests.</param>
    public void addQuest(Quest quest)
    {
        if (playerInstance == null)
            return;
        if (dialogueManager == null)
            return;
        if (quest == null)
            return;

        if (playerInstance.addQuest(quest))
        {
            notificationsManager.queueTopLeftNotification("New Quest Added", "notification");
            if (quest.GetType() == typeof(KillQuest))
                activeKillQuests.Add((KillQuest)quest);

            if (quest.GetType() == typeof(FindQuest))
                activeFindQuests.Add((FindQuest)quest);
        }
    }

    #endregion

    #region Quest Progress Tracking

    /// <summary>
    /// Processes a found object for find quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectFound">The GameObject that was found, used to match against quest targets.</param>
    public void addFind(GameObject objectFound)
    {
        FindQuest questToInc = activeFindQuests.Find(questToFind =>
            questToFind != null && questToFind.QuestTarget == objectFound.tag
        );

        if (questToInc == null)
        {
            return;
        }

        questToInc.progress();
        Destroy(objectFound);
        if (questToInc.isCompleted)
        {
            activeFindQuests.Remove(questToInc);
            playerInstance.removeQuest(questToInc);
            notificationsManager.queueTopLeftNotification(
                questToInc.GetQuestName() + " Completed! (+" + questToInc.Reward + " EXP)",
                "notification"
            );
            onQuestFinish?.Invoke(float.Parse(questToInc.Reward));
            //find quest giver by gameobject ID
            StartCoroutine(refreshQuestGiver(questToInc.Giver));
        }
    }

    /// <summary>
    /// Processes a killed object for kill quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectKilled">The GameObject that was killed, used to match against quest targets.</param>
    public void addKill(GameObject objectKilled)
    {
        List<KillQuest> questToInc = activeKillQuests.FindAll(questToKill =>
            questToKill != null && questToKill.QuestTarget == objectKilled.tag
        );

        if (questToInc.Count == 0)
        {
            return;
        }

        float totalReward = 0;

        foreach (KillQuest quest in questToInc)
        {
            quest.progress();

            if (quest.isCompleted)
            {
                totalReward += float.Parse(quest.Reward);
                playerInstance.removeQuest(quest);
                activeKillQuests.Remove(quest);
                notificationsManager.queueTopLeftNotification(
                    quest.GetQuestName() + " Completed! (+" + quest.Reward + " EXP)",
                    "notification"
                );
            }
        }

        onQuestFinish?.Invoke(totalReward);
    }

    private IEnumerator refreshQuestGiver(GameObject giver)
    {
        yield return new WaitForSeconds(1.5f);
        if (giver != null)
        {
            giver.GetComponent<StartNpc>().refreshQuestGiver();
        }
        else
        {
            Debug.LogError("Quest giver not found");
        }
    }
    #endregion

    #region Story Quest Management

    /// <summary>
    /// Advances to the next main story quest in the queue.
    /// </summary>
    public void nextMainQuest()
    {
        if (playerInstance != null && storyQuestListQueue.Count > 0)
        {
            playerInstance.setCurrentMainQuest(storyQuestListQueue.Dequeue());
        }
    }

    /// <summary>
    /// Completes the current main quest by setting it to null.
    /// </summary>
    public void completeMainQuest()
    {
        playerInstance.setCurrentMainQuest(null);
    }

    #endregion
}
