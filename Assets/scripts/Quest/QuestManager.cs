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

    [SerializeField]
    private UIManager uiManager;

    private RewardManager rewardManager;

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

    private GameObject questSpawns;
    private ObjectSpawnsManager objectSpawnManager;

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

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes quest management system, sets up quest collections, and subscribes to dialogue events.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(n) where n = number of quest spawn objects
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        notificationsManager = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<NotificationsManager>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        dialogueManager = GameObject.FindAnyObjectByType<DialogueManager>();
        uiManager = GameObject.FindAnyObjectByType<UIManager>();
        rewardManager = GameObject.FindAnyObjectByType<RewardManager>();

        questSpawns = GameObject.FindWithTag("ObjectSpawns");
        if (questSpawns != null)
            objectSpawnManager = questSpawns.GetComponent<ObjectSpawnsManager>();

        if (rewardManager == null)
        {
            // RewardManager not found
        }

        if (questSpawns != null)
        {
            foreach (Transform child in questSpawns.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        initPlayer();
        subscribeToEvents();
    }

    #endregion

    #region Initialization Methods

    /// <summary>
    /// Subscribes to quest-related events for proper integration.
    /// </summary>
    // COMPLEXITY ANALYSIS: subscribeToEvents() - O(1)
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
    // COMPLEXITY ANALYSIS: initQuestLists() - O(q + s) where q = number of quests, s = number of story quests
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
            List<Quest> childQuests = storyQuestsList.Quests[i].GetChildQuests();
            List<Quest> childQuestsToAdd = new List<Quest>();
            foreach (Quest childQuest in childQuests)
            {
                childQuestsToAdd.Add(ScriptableObject.Instantiate(childQuest));
            }
            storyQuestsList.Quests[i].SetChildQuests(null);

            StoryQuest quest = ScriptableObject.Instantiate(storyQuestsList.Quests[i]);

            storyQuestsList.Quests[i].SetChildQuests(childQuests);

            quest.SetChildQuests(childQuestsToAdd);
            foreach (Quest childQuest in childQuestsToAdd)
            {
                childQuest.setParentQuest(quest);
            }

            storyQuestListQueue.Enqueue(quest);
        }
        isReadyToStartQuest = true;
        nextStoryQuest();
    }

    /// <summary>
    /// Initializes the player instance reference for quest management.
    /// </summary>
    // COMPLEXITY ANALYSIS: initPlayer() - O(1)
    private void initPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        playerInstance = player.GetComponent<StartPlayer>().getPlayer();
        if (playerInstance == null)
        {
            return;
        }
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Adds a new quest to the player's active quest list and categorizes it by type.
    /// </summary>
    /// <param name="quest">The quest to be added to the player's active quests.</param>
    // COMPLEXITY ANALYSIS: addQuest() - O(s) where s = number of quest spawn objects
    public void addQuest(Quest quest)
    {
        if (playerInstance == null)
            return;

        if (quest == null)
            return;

        if (playerInstance.addQuest(quest))
        {
            if (quest.ParentQuest == null)
                notificationsManager.queueTopLeftNotification("New Quest Added", "notification");

            tryAddKillQuest(quest);

            tryAddFindQuest(quest);
        }
    }

    public void tryAddKillQuest(Quest quest)
    {
        if (quest is KillQuest)
        {
            activeKillQuests.Add((KillQuest)quest);
            uiManager.addQuest(quest.Giver.GetInstanceID(), quest);
        }
    }

    public void tryAddFindQuest(Quest quest)
    {
        if (quest is FindQuest)
        {
            activeFindQuests.Add((FindQuest)quest);
            uiManager.addQuest(quest.Giver.GetInstanceID(), quest);

            if (objectSpawnManager != null)
                objectSpawnManager.SpawnAccordingToQuest(quest);

            if (quest.Giver != null)
            {
                uiManager.addQuest(quest.Giver.GetInstanceID(), quest);
            }
            else
            {
                GameObject giver = new GameObject("Quest Giver");
                quest.SetGiver(giver);
                uiManager.addQuest(quest.Giver.GetInstanceID(), quest);
            }
        }
    }
    #endregion

    #region Quest Progress Tracking

    /// <summary>
    /// Processes a found object for find quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectFound">The GameObject that was found, used to match against quest targets.</param>
    // COMPLEXITY ANALYSIS: addFind() - O(q) where q = number of active find quests
    public void addFind(GameObject objectFound)
    {
        FindQuest questToInc = activeFindQuests.Find(questToFind =>
            questToFind != null
            && string.Join(", ", questToFind.QuestTarget).Contains(objectFound.tag)
        );

        int expReward = 0;

        if (questToInc != null)
        {
            questToInc.progress(out int questReward);
            expReward += questReward;
            uiManager.updateQuestProgress(
                questToInc.Giver.GetInstanceID(),
                questToInc.GetProgress()
            );

            objectSpawnManager.DeSpawnOjbect(objectFound);
            if (questToInc.isCompleted)
            {
                removeQuest(questToInc);

                notificationsManager.queueTopLeftNotification(
                    questToInc.GetQuestName() + " Completed!",
                    "notification"
                );
                if (questToInc.RewardType == RewardType.Item)
                {
                    rewardManager.GiveReward(questToInc.Reward);
                }
                StartCoroutine(refreshQuestGiver(questToInc.Giver));
            }
        }

        if (playerInstance.getCurrentMainQuest() != null)
        {
            playerInstance
                .getCurrentMainQuest()
                .ProgressChildFindQuests(objectFound, out int questReward);
            expReward += questReward;
        }
        if (expReward > 0)
            rewardManager.GiveExpReward(expReward);
    }

    /// <summary>
    /// Processes a killed object for kill quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectKilled">The GameObject that was killed, used to match against quest targets.</param>
    // COMPLEXITY ANALYSIS: addKill() - O(q) where q = number of active kill quests
    public void addKill(string objectKilled)
    {
        // Process kill quest for: " + objectKilled
        List<KillQuest> questToInc = activeKillQuests.FindAll(questToKill =>
            questToKill != null && string.Join(", ", questToKill.QuestTarget).Contains(objectKilled)
        );
        int expReward = 0;

        if (questToInc.Count > 0)
        {
            foreach (KillQuest quest in questToInc)
            {
                quest.progress(out int questReward);
                expReward += questReward;
                uiManager.updateQuestProgress(quest.Giver.GetInstanceID(), quest.GetProgress());

                if (quest.isCompleted)
                {
                    removeQuest(quest);

                    notificationsManager.queueTopLeftNotification(
                        quest.GetQuestName() + " Completed!",
                        "notification"
                    );
                    if (quest.RewardType == RewardType.Item)
                    {
                        rewardManager.GiveReward(quest.Reward);
                    }

                    StartCoroutine(refreshQuestGiver(quest.Giver));
                }
            }
        }

        if (playerInstance.getCurrentMainQuest() != null)
        {
            playerInstance
                .getCurrentMainQuest()
                .ProgressChildKillQuests(objectKilled, out int questReward);

            expReward += questReward;
        }
        if (expReward > 0)
        {
            rewardManager.GiveExpReward(expReward);
        }
    }

    public void removeQuest(Quest quest)
    {
        if (quest is KillQuest)
        {
            playerInstance.removeQuest(quest);
            activeKillQuests.Remove((KillQuest)quest);
            uiManager.removeQuest(quest.Giver.GetInstanceID());
        }
        if (quest is FindQuest)
        {
            playerInstance.removeQuest(quest);
            activeFindQuests.Remove((FindQuest)quest);
            uiManager.removeQuest(quest.Giver.GetInstanceID());
        }
    }

    /// <summary>
    /// Refreshes the quest giver after a quest is completed.
    /// </summary>
    /// <param name="giver">The quest giver GameObject to refresh.</param>
    // COMPLEXITY ANALYSIS: refreshQuestGiver() - O(1)
    private IEnumerator refreshQuestGiver(GameObject giver)
    {
        yield return new WaitForSeconds(1.5f);
        if (giver != null)
        {
            giver.GetComponent<StartNpc>().refreshQuestGiver();
        }
    }

    #endregion

    #region Story Quest Management

    /// <summary>
    /// Initiates the next story quest in the sequence.
    /// </summary>
    // COMPLEXITY ANALYSIS: nextStoryQuest() - O(1)
    public void nextStoryQuest()
    {
        StartCoroutine(nextMainQuest());
    }

    /// <summary>
    /// Advances to the next main story quest in the queue.
    /// </summary>
    // COMPLEXITY ANALYSIS: nextMainQuest() - O(1)
    public IEnumerator nextMainQuest()
    {
        storyQuestIndex++;

        if (
            playerInstance.getCurrentMainQuest() != null
            && playerInstance.getCurrentMainQuest().RewardType == RewardType.Item
        )
        {
            rewardManager.GiveReward(playerInstance.getCurrentMainQuest().Reward);
        }
        else if (
            playerInstance.getCurrentMainQuest() != null
            && playerInstance.getCurrentMainQuest().RewardType == RewardType.XP
        )
        {
            rewardManager.GiveExpReward(playerInstance.getCurrentMainQuest().Reward);
        }

        if (storyQuestListQueue.Count > 0)
        {
            uiManager.updateStoryQuestPanel(storyQuestListQueue.Peek());
        }

        if (playerInstance != null && storyQuestListQueue.Count > 0)
        {
            playerInstance.setCurrentMainQuest(storyQuestListQueue.Dequeue());

            while (playerScript.getIsInCutscene())
            {
                yield return null;
            }
            yield return new WaitForSeconds(waitTimeQuest);
            waitTimeQuest = 1.5f;

            notificationsManager.queueTopLeftNotification(
                playerInstance.getCurrentMainQuest().GetQuestName() + " Started",
                "notification"
            );

            if (minimapArrow != null)
                minimapArrow.SetTarget(playerInstance.getCurrentMainQuest().TargetPosition);
        }
        yield return null;
    }

    /// <summary>
    /// Completes the current main quest by setting it to null.
    /// </summary>
    // COMPLEXITY ANALYSIS: completeMainQuest() - O(1)
    public void completeMainQuest()
    {
        playerInstance.setCurrentMainQuest(null);
    }

    /// <summary>
    /// Checks if a story quest of the specified type has been completed.
    /// </summary>
    /// <param name="type">The type of story quest to check.</param>
    /// <returns>True if a story quest of the specified type has been completed.</returns>
    // COMPLEXITY ANALYSIS: checkingCompletedStoryQuest() - O(s) where s = number of completed story quests
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
