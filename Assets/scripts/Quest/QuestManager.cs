using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject player;

    /// <summary>
    /// Reference to the player instance for quest management.
    /// </summary>
    private Player playerInstance;

    public Player PlayerInstance => playerInstance;

    /// <summary>
    /// Reference to the dialogue manager for quest integration.
    /// </summary>
    [SerializeField]
    private DialogueManager dialogueManager;

    /// <summary>
    /// List of story quests available in the game.
    /// </summary>
    [SerializeField]
    private StoryQuests storyQuestsList;

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

    public Queue<StoryQuest> StoryQuestListQueue => storyQuestListQueue;

    private List<StoryQuest> completedStoryQuest;

    public List<StoryQuest> CompletedStoryQuest => completedStoryQuest;

    /// <summary>
    /// List of all quests for efficient processing.
    /// </summary>
    [SerializeField]
    private ExpFindQuests allFindQuests;
    public ExpFindQuests AllFindQuests => allFindQuests;

    [SerializeField]
    private ExpKillQuests allKillQuests;
    public ExpKillQuests AllKillQuests => allKillQuests;

    /// <summary>
    /// List of active kill quests for efficient processing.
    /// </summary>
    private List<KillQuest> activeKillQuests;

    /// <summary>
    /// List of active find quests for efficient processing.
    /// </summary>
    private List<FindQuest> activeFindQuests;

    /// <summary>
    /// Reference to the notifications manager for displaying quest updates.
    /// </summary>
    private NotificationsManager notificationsManager;

    [SerializeField]
    private MinimapArrow minimapArrow;

    private string gameManagerTag = "GameManager";

    [SerializeField]
    private int storyQuestIndex;

    public int StoryQuestIndex => storyQuestIndex;

    private bool isReadyToStartQuest = false;

    public bool IsReadyToStartQuest => isReadyToStartQuest;

    [SerializeField]
    private float waitTimeQuest = 3f;
    public float WaitTimeQuest => waitTimeQuest;

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
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        notificationsManager = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<NotificationsManager>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        dialogueManager = GameObject.FindAnyObjectByType<DialogueManager>();
        initPlayer();
        subscribeToEvents();
    }

    #endregion

    #region Initialization Methods

    /// <summary>
    /// Subscribes to quest-related events for proper integration.
    /// </summary>
    private void subscribeToEvents()
    {
        StoryQuest.subscribeToQuestCompletion(nextStoryQuest);

        if (dialogueManager != null)
            dialogueManager.onDialogueExit += addQuest;
        KillEnemyHandler.Subscribe(addKill);
    }

    /// <summary>
    /// Initializes quest lists and sets up the story quest queue.
    /// </summary>
    public void initQuestLists(QuestData questData = null)
    {
        if (playerInstance == null)
        {
            initPlayer();
        }
        activeKillQuests = new List<KillQuest>();
        activeFindQuests = new List<FindQuest>();
        storyQuestListQueue = new Queue<StoryQuest>();
        completedStoryQuest = new List<StoryQuest>();
        storyQuestIndex = questData != null ? questData.StoryQuestIndex - 1 : -1;

        if (questData != null)
        {
            List<int> questIds = questData.ActiveQuestIds;

            foreach (int questId in questIds)
            {
                Quest quest = allFindQuests.Find(questId);
                if (quest == null)
                {
                    quest = allKillQuests.Find(questId);
                }
                if (quest != null)
                {
                    addQuest(quest);
                }
            }
        }
        else
        {
            foreach (Quest quest in playerInstance.ActiveSideQuests)
            {
                addQuest(quest);
            }
        }
        for (int i = 0; i < storyQuestIndex + 1; i++)
        {
            completedStoryQuest.Add(storyQuestsList.Quests[i]);
        }
        for (int i = storyQuestIndex + 1; i < storyQuestsList.Quests.Count; i++)
        {
            storyQuestListQueue.Enqueue(ScriptableObject.Instantiate(storyQuestsList.Quests[i]));
        }
        isReadyToStartQuest = true;
        nextStoryQuest();
    }

    /// <summary>
    /// Initializes the player instance reference for quest management.
    /// </summary>
    private void initPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        playerInstance = player.GetComponent<StartPlayer>().getPlayer();
        if (playerInstance == null)
        {
            Debug.LogError("Player instance is null");
        }
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Adds a new quest to the player's active quest list and categorizes it by type.
    /// </summary>
    /// <param name="quest">The quest to be added to the player's active quests.</param>
    public void addQuest(Quest quest)
    {
        Debug.Log("Add Quest: " + quest.GetQuestName());
        if (playerInstance == null)
            return;

        if (quest == null)
            return;

        if (playerInstance.addQuest(quest))
        {
            if (quest.ParentQuest == null)
                notificationsManager.queueTopLeftNotification("New Quest Added", "notification");
            if (quest is KillQuest)
                activeKillQuests.Add((KillQuest)quest);

            if (quest is FindQuest)
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
        Debug.Log("Add Find: " + objectFound.tag);
        FindQuest questToInc = activeFindQuests.Find(questToFind =>
            questToFind != null
            && string.Join(", ", questToFind.QuestTarget).Contains(objectFound.tag)
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

            if (questToInc.ParentQuest == null)
                notificationsManager.queueTopLeftNotification(
                    questToInc.GetQuestName() + " Completed! (+" + questToInc.Reward + " EXP)",
                    "notification"
                );

            if (questToInc.ParentQuest != null)
            {
                questToInc.ParentQuest.CompleteQuest();
            }

            onQuestFinish?.Invoke(questToInc.Reward);

            if (!(questToInc.ParentQuest is StoryQuest))
            {
                //find quest giver by gameobject ID
                StartCoroutine(refreshQuestGiver(questToInc.Giver));
            }
        }
    }

    /// <summary>
    /// Processes a killed object for kill quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectKilled">The GameObject that was killed, used to match against quest targets.</param>
    public void addKill(string objectKilled)
    {
        List<KillQuest> questToInc = activeKillQuests.FindAll(questToKill =>
            questToKill != null && string.Join(", ", questToKill.QuestTarget).Contains(objectKilled)
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
                totalReward += quest.Reward;
                playerInstance.removeQuest(quest);
                activeKillQuests.Remove(quest);
                if (quest.ParentQuest == null)
                    notificationsManager.queueTopLeftNotification(
                        quest.GetQuestName() + " Completed! (+" + quest.Reward + " EXP)",
                        "notification"
                    );
                if (quest.ParentQuest != null)
                {
                    quest.ParentQuest.CompleteQuest();
                }
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


    public void nextStoryQuest()
    {
        StartCoroutine(nextMainQuest());
    }

    /// <summary>
    /// Advances to the next main story quest in the queue.
    /// </summary>
    public IEnumerator nextMainQuest()
    {
        yield return new WaitForSeconds(waitTimeQuest);
        waitTimeQuest = 1.5f;
        while (playerScript.getIsInCutscene())
        {
            yield return null;
        }

        if (playerInstance != null && storyQuestListQueue.Count > 0)
        {
            playerInstance.setCurrentMainQuest(storyQuestListQueue.Dequeue());
            notificationsManager.queueTopLeftNotification(
                playerInstance.getCurrentMainQuest().GetQuestName() + " Started",
                "notification"
            );
            storyQuestIndex++;
            if (
                playerInstance.getCurrentMainQuest() != null
                && playerInstance.getCurrentMainQuest().GetChildQuests().Count > 0
            )
            {
                playerInstance
                    .getCurrentMainQuest()
                    .GetChildQuests()
                    .ForEach(quest =>
                    {
                        addQuest(quest);
                    });
            }
            if (minimapArrow != null)
                minimapArrow.SetTarget(playerInstance.getCurrentMainQuest().TargetPosition);
        }
    }

    /// <summary>
    /// Completes the current main quest by setting it to null.
    /// </summary>
    public void completeMainQuest()
    {
        playerInstance.setCurrentMainQuest(null);
    }

    public bool checkingCompletedStoryQuest(Type type)
    {
        if (completedStoryQuest == null)
            return false;
        foreach (StoryQuest quest in completedStoryQuest)
        {
            if (quest != null && type.IsInstanceOfType(quest))
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
