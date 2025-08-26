using UnityEngine;

/// <summary>
/// Quest type that requires the player to kill a specific number of enemies.
/// Tracks kill count progress and marks quest as complete when target is reached.
/// </summary>
[CreateAssetMenu(fileName = "newQuest", menuName = "Quests/ExpQuest/KillQuest")]
public class KillQuest : Quest
{
    public KillQuest(KillQuest quest)
        : base(quest)
    {
        this.targetKills = quest.targetKills;
        this.kills = quest.kills;
    }

    /// <summary>
    /// Current number of enemies killed by the player for this quest.
    /// </summary>
    [SerializeField]
    private int kills;

    /// <summary>
    /// Target number of enemies that must be killed to complete this quest.
    /// </summary>
    [SerializeField]
    private int targetKills;

    /// <summary>
    /// Increments the kill count for this quest.
    /// Called when the player successfully kills an enemy of the target type.
    /// </summary>
    override public void progress()
    {
        kills = Mathf.Min(kills + 1, targetKills);
        Debug.Log(this.kills);

        if (kills == targetKills)
        {
            completed = true;

            if (ParentQuest != null)
            {
                ParentQuest.CompleteQuest();
            }
            
        }
    }

    override public string GetProgress()
    {
        return kills + "/" + targetKills;
    }
}
