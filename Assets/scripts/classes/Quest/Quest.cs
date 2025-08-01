using System;
using UnityEngine;

/// <summary>
/// Abstract base class for all quest types in the game.
/// Defines common quest properties and functionality that all quest implementations must provide.
/// </summary>
[System.Serializable]
public abstract class Quest : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// The parent quest of this quest.
    /// </summary>
    [SerializeField]
    private StoryQuest parentQuest;

    /// <summary>
    /// The name of the quest displayed to the player.
    /// </summary>
    [SerializeField]
    private string questName;

    /// <summary>
    /// The description of the quest that explains what the player needs to accomplish.
    /// </summary>
    [SerializeField]
    private string description;

    /// <summary>
    /// The target or objective of the quest (e.g., enemy type to kill, item to find).
    /// </summary>
    [SerializeField]
    private string questTarget;

    /// <summary>
    /// The type of reward given upon quest completion (e.g., "XP", "Gold", "Item").
    /// </summary>
    [SerializeField]
    private string rewardType;

    /// <summary>
    /// The specific reward value or description given upon quest completion.
    /// </summary>
    [SerializeField]
    private string rewards;

    /// <summary>
    /// Flag indicating whether this quest has been completed by the player.
    /// </summary>
    [SerializeField]
    protected bool completed;

    #endregion

    #region Private Fields

    /// <summary>
    /// The unique identifier of the NPC that gave this quest.
    /// </summary>
    private GameObject giver;

    #endregion

    #region Constructors

    /// <summary>
    /// Copy constructor for quest.
    /// Copies all the properties of the quest to the new quest.
    /// </summary>
    /// <param name="quest">The quest to copy properties from.</param>
    public Quest(Quest quest)
    {
        this.questName = quest.questName;
        this.description = quest.description;
        this.questTarget = quest.questTarget;
        this.rewardType = quest.rewardType;
        this.rewards = quest.rewards;
        this.giver = quest.giver;
        this.completed = false;
    }

    #endregion

    #region Quest Giver Management

    /// <summary>
    /// Sets the ID of the NPC that gave this quest.
    /// </summary>
    /// <param name="giverId">The unique identifier of the quest giver NPC.</param>
    public void SetGiver(GameObject giver)
    {
        this.giver = giver;
    }

    /// <summary>
    /// Gets the ID of the NPC that gave this quest.
    /// </summary>
    public GameObject Giver
    {
        get { return giver; }
    }

    #endregion

    #region Quest Properties

    /// <summary>
    /// Gets the target or objective of this quest.
    /// </summary>
    public string QuestTarget
    {
        get { return this.questTarget; }
    }

    /// <summary>
    /// Gets whether this quest has been completed.
    /// </summary>
    public bool isCompleted
    {
        get { return this.completed; }
    }

    /// <summary>
    /// Gets the type of reward given upon quest completion.
    /// </summary>
    public string RewardType
    {
        get { return this.rewardType; }
    }

    /// <summary>
    /// Gets the specific reward value or description for this quest.
    /// </summary>
    public string Reward
    {
        get { return this.rewards; }
    }

    /// <summary>
    /// Gets the parent quest of this quest.
    /// </summary>
    public StoryQuest ParentQuest
    {
        get { return this.parentQuest; }
    }

    /// <summary>
    /// Gets the name of this quest.
    /// </summary>
    /// <returns>The quest name.</returns>
    public string GetQuestName()
    {
        return this.questName;
    }


    public void setParentQuest(StoryQuest parentQuest)
    {
        this.parentQuest = parentQuest;
    }

    
    #endregion

    #region Abstract Methods

    /// <summary>
    /// Abstract method that must be implemented by derived quest classes.
    /// Handles quest progress logic specific to each quest type.
    /// </summary>
    public abstract void progress();

    #endregion
}
