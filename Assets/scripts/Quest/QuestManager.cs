using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages quest-related functionality including story quests, active quests, and quest progress tracking.
/// Handles different types of quests (KillQuest, FindQuest) and provides quest completion events.
/// </summary>
public class QuestManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the player GameObject for quest management.
    /// </summary>
    [SerializeField]
    private GameObject player;

    private AudioSource audioSource;
    /// <summary>
    /// Reference to the dialogue manager for quest integration.
    /// </summary>
    [SerializeField]
    private DialogueManager dialogueManager;

    [SerializeField]
    private AudioManager audioManager;
    /// <summary>
    /// Queue containing story quests to be processed in order.
    /// </summary>
    private Queue<Quest> storyQuests;

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
    /// Event triggered when a quest is completed, providing the reward amount.
    /// </summary>
    public event Action<float> onQuestFinish;

    /// <summary>
    /// Initializes quest management system, sets up quest collections, and subscribes to dialogue events.
    /// </summary>
    private NotificationsManager notificationsManager;

    private void Start()
    {
        notificationsManager = GameObject.Find("GameManger").GetComponent<NotificationsManager>();
        Debug.Log(player.GetComponent<StartPlayer>().getPlayer());
        dialogueManager.onDialogueExit += addQuest;
        initQuestLists();
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    private void initQuestLists()
    {
        storyQuests = new Queue<Quest>();
        activeQuests = player.GetComponent<StartPlayer>().getPlayer().ActiveQuest;
        activeKillQuests = new List<KillQuest>().FindAll(quest =>
            quest != null && quest.GetType() == typeof(KillQuest)
        );
        activeFindQuests = new List<FindQuest>().FindAll(quest =>
            quest != null && quest.GetType() == typeof(FindQuest)
        );
    }

    /// <summary>
    /// Adds a new quest to the player's active quest list and categorizes it by type.
    /// </summary>
    /// <param name="quest">The quest to be added to the player's active quests.</param>
    public void addQuest(Quest quest)
    {
        if (player.GetComponent<StartPlayer>().getPlayer() == null)
            return;
        if (dialogueManager == null)
            return;
        if (quest == null)
            return;

      if (player.GetComponent<StartPlayer>().getPlayer().addQuest(quest))
      {
        notificationsManager.queueTopLeftNotification("New Quest Added");
       StartCoroutine(audioManager.queueUI(audioSource ,"notification"));
        if (quest.GetType() == typeof(KillQuest))
            activeKillQuests.Add((KillQuest)quest);

        if (quest.GetType() == typeof(FindQuest))
            activeFindQuests.Add((FindQuest)quest);

      }

    }

    /// <summary>
    /// Processes a found object for find quests, updates quest progress, and handles quest completion.
    /// </summary>
    /// <param name="objectFound">The GameObject that was found, used to match against quest targets.</param>
    public void addFind(GameObject objectFound)
    {
        FindQuest questToInc = activeFindQuests.Find(questToFind =>questToFind != null && questToFind.QuestTarget == objectFound.tag);

        if (questToInc == null){
            Debug.Log("Object is not related to any quest");
            return;
        }

        questToInc.progress();
        Destroy(objectFound);
        if (questToInc.isCompleted){
            activeFindQuests.Remove(questToInc);
            Debug.Log("Quest Completed: " + questToInc.GetQuestName());
           StartCoroutine(audioManager.queueUI(audioSource ,"notification"));
            notificationsManager.queueTopLeftNotification(questToInc.GetQuestName() + " Completed! (+" + questToInc.Reward + " EXP)");            
            onQuestFinish?.Invoke(float.Parse(questToInc.Reward));


        }

    }

    public void addKill(GameObject objectKilled)
    {
        List<KillQuest> questToInc = activeKillQuests.FindAll(questToKill =>questToKill != null && questToKill.QuestTarget == objectKilled.tag);

        if (questToInc.Count == 0){
            Debug.Log("Object is not related to any quest");
            return;
        }

        float totalReward = 0;

        foreach (KillQuest quest in questToInc)
        {
            quest.progress();
            if (quest.isCompleted){
                totalReward += float.Parse(quest.Reward);
                activeKillQuests.Remove(quest);
                notificationsManager.queueTopLeftNotification(quest.GetQuestName() + " Completed! (+" + quest.Reward + " EXP)");
                StartCoroutine(audioManager.queueUI(audioSource ,"notification")    );
            }
        }

        onQuestFinish?.Invoke(totalReward);
    }

    public void nextMainQuest(){
        player.GetComponent<StartPlayer>().getPlayer().setCurrentMainQuest(storyQuests.Dequeue());
    }

    public void completeMainQuest(){
        player.GetComponent<StartPlayer>().getPlayer().setCurrentMainQuest(null);
    }

}
