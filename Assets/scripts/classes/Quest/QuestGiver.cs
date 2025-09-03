using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC class specialized for giving quests to players.
/// Manages dialogue interactions and quest distribution functionality.
/// </summary>
public class QuestGiver : TalkativeNpc
{
    #region Private Fields

    /// <summary>
    /// The starting dialogue key for initiating conversation with this quest giver.
    /// </summary>
    private string start;

    /// <summary>
    /// The quest that this NPC will give to the player upon dialogue completion.
    /// </summary>
    private Quest questToGive;

    /// <summary>
    /// The story quest that this NPC will give to the player upon dialogue completion.
    /// </summary>
    private StoryQuest storyQuestToGive;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the QuestGiver class.
    /// </summary>
    /// <param name="id">The unique identifier for this quest giver.</param>
    /// <param name="name">The name of this quest giver.</param>
    /// <param name="type">The type of NPC.</param>
    /// <param name="walkRadius">The radius within which the NPC can walk.</param>
    /// <param name="areaMask">The area mask for navigation.</param>
    /// <param name="waitTimeRange">The range of time the NPC waits between actions.</param>
    /// <param name="navMeshAreaName">The name of the navigation mesh area.</param>
    /// <param name="speed">The movement speed of the NPC.</param>
    /// <param name="maxSpeed">The maximum speed of the NPC.</param>
    /// <param name="start">The starting dialogue key.</param>
    /// <param name="dialogues">Dictionary of available dialogues.</param>
    /// <param name="questToGive">The quest to be given to the player.</param>
    public QuestGiver(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float speed,
        float maxSpeed,
        string start,
        Dictionary<string, Dialogue> dialogues,
        Quest questToGive,
        GameObject giver
    )
        : base(
            id,
            name,
            type,
            walkRadius,
            areaMask,
            waitTimeRange,
            navMeshAreaName,
            speed,
            maxSpeed,
            dialogues,
            start
        )
    {
        if (questToGive != null)
        {
            this.questToGive = questToGive;
            this.questToGive.SetGiver(giver);
        }
    }

    /// <summary>
    /// Initializes a new instance of the QuestGiver class with a story quest.
    /// </summary>
    /// <param name="id">The unique identifier for this quest giver.</param>
    /// <param name="name">The name of this quest giver.</param>
    /// <param name="type">The type of NPC.</param>
    /// <param name="walkRadius">The radius within which the NPC can walk.</param>
    /// <param name="areaMask">The area mask for navigation.</param>
    /// <param name="waitTimeRange">The range of time the NPC waits between actions.</param>
    /// <param name="navMeshAreaName">The name of the navigation mesh area.</param>
    /// <param name="speed">The movement speed of the NPC.</param>
    /// <param name="maxSpeed">The maximum speed of the NPC.</param>
    /// <param name="start">The starting dialogue key.</param>
    /// <param name="dialogues">Dictionary of available dialogues.</param>
    /// <param name="storyQuestToGive">The story quest to be given to the player.</param>
    /// <param name="giver">The GameObject reference of the quest giver.</param>
    public QuestGiver(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float speed,
        float maxSpeed,
        string start,
        Dictionary<string, Dialogue> dialogues,
        StoryQuest storyQuestToGive,
        GameObject giver
    )
        : base(
            id,
            name,
            type,
            walkRadius,
            areaMask,
            waitTimeRange,
            navMeshAreaName,
            speed,
            maxSpeed,
            dialogues,
            start
        )
    {
        if (storyQuestToGive != null)
        {
            this.storyQuestToGive = storyQuestToGive;
            this.storyQuestToGive.SetGiver(giver);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the dictionary of dialogues associated with this quest giver.
    /// </summary>
    /// <returns>Dictionary containing dialogue keys and their corresponding Dialogue objects.</returns>
    public Dictionary<string, Dialogue> getDialogues()
    {
        return this.dialogues;
    }

    /// <summary>
    /// Gives a quest to the player identified by the provided tag.
    /// </summary>
    /// <param name="playerTag">The tag identifying the player to receive the quest.</param>
    public void giveQUest(string playerTag)
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        player.GetComponent<StartPlayer>().getPlayer().addQuest(questToGive);
    }

    /// <summary>
    /// Responds to a dialogue interaction by providing the dialogue text and options.
    /// </summary>
    /// <param name="dialogue">The key identifying the dialogue to respond to.</param>
    /// <param name="options">The array of response options for the dialogue.</param>
    /// <returns>The text content of the dialogue response.</returns>
    public string respodToDialogue(string dialogue, out string[] options)
    {
        options = this.dialogues[dialogue].GetOptions();

        return this.dialogues[dialogue].GetText();
    }

    /// <summary>
    /// Sets the quest to be given by this quest giver.
    /// </summary>
    /// <param name="quest">The quest to be given.</param>
    /// <param name="giver">The GameObject reference of the quest giver.</param>
    public void setQuestToGive(Quest quest, GameObject giver)
    {
        if (quest == null)
        {
            this.questToGive = null;
            return;
        }

        if (quest.GetType() == typeof(FindQuest))
        {
            this.questToGive = new FindQuest(quest as FindQuest);
            this.questToGive.SetGiver(giver);
        }
        else if (quest.GetType() == typeof(KillQuest))
        {
            this.questToGive = new KillQuest(quest as KillQuest);
            this.questToGive.SetGiver(giver);
        }
        else if (quest is StoryQuest)
        {
            this.questToGive = quest;
            this.questToGive.SetGiver(giver);
        }
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the quest that this quest giver is currently giving.
    /// </summary>
    /// <returns>The quest that this quest giver is currently giving.</returns>
    public Quest GetQuestToGive()
    {
        return this.questToGive;
    }

    /// <summary>
    /// Gets the story quest that this quest giver is currently giving.
    /// </summary>
    /// <returns>The story quest that this quest giver is currently giving.</returns>
    public StoryQuest GetStoryQuestToGive()
    {
        return this.storyQuestToGive;
    }

    #endregion
}
