

using System;
using UnityEngine;

/// <summary>
/// Abstract base class for all quest types in the game.
/// Defines common quest properties and functionality that all quest implementations must provide.
/// </summary>
[System.Serializable]
public abstract class Quest : ScriptableObject
{
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
    /// The unique identifier of the NPC that gave this quest.
    /// </summary>
    private int giverId;
    
    /// <summary>
    /// Flag indicating whether this quest has been completed by the player.
    /// </summary>
    [SerializeField]
    protected bool completed;



/// <summary>
/// Copy constructor for quest
/// Copy all the properties of the quest to the new quest
/// </summary>
/// <param name="quest"></param>
    public Quest(Quest quest)
    {
        this.questName = quest.questName;
        this.description = quest.description;
        this.questTarget = quest.questTarget;
        this.rewardType = quest.rewardType;
        this.rewards = quest.rewards;
        this.giverId = quest.giverId;
        this.completed = false;
    }

    



    /// <summary>
    /// Sets the ID of the NPC that gave this quest.
    /// </summary>
    /// <param name="giverId">The unique identifier of the quest giver NPC.</param>
    public void SetGiverID(int giverId)
    {
        this.giverId = giverId;
    }
    
    /// <summary>
    /// Gets the ID of the NPC that gave this quest.
    /// </summary>
    public int GiverId
    {
        get
        {
            return giverId;
        }
    }

    /// <summary>
    /// Gets the target or objective of this quest.
    /// </summary>
    public string QuestTarget
    {
        get
        {
            return this.questTarget;
        }
    }
    
    /// <summary>
    /// Abstract method that must be implemented by derived quest classes.
    /// Handles quest progress logic specific to each quest type.
    /// </summary>
    public abstract void progress();

    /// <summary>
    /// Gets whether this quest has been completed.
    /// </summary>
    public bool isCompleted
    {
        get
        {
            return this.completed;
        }
    }
    
    /// <summary>
    /// Gets the type of reward given upon quest completion.
    /// </summary>
    public string RewardType
    {
        get
        {
            return this.rewardType;
        }
    }

    /// <summary>
    /// Gets the specific reward value or description for this quest.
    /// </summary>
    public string Reward
    {
        get {
            return this.rewards;
        }
    }

    public string GetQuestName()
    {
        return this.questName;
    }



}