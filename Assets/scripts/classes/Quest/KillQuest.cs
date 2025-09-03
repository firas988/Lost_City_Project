using UnityEngine;

/// <summary>
/// Quest type that requires the player to kill a specific number of enemies.
/// Tracks kill count progress and marks quest as complete when target is reached.
/// </summary>
[CreateAssetMenu(fileName = "newQuest", menuName = "Quests/ExpQuest/KillQuest")]
public class KillQuest : Quest
{
    #region Constructors

    /// <summary>
    /// Copy constructor for KillQuest.
    /// </summary>
    /// <param name="quest">The KillQuest to copy properties from.</param>
    public KillQuest(KillQuest quest)
        : base(quest)
    {
        this.targetKills = quest.targetKills;
        this.kills = quest.kills;
    }

    #endregion

    #region Serialized Fields

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

    #endregion

    #region Quest Progress

    /// <summary>
    /// Gets the progress string showing kills vs target kills.
    /// </summary>
    /// <returns>A string in the format "kills/targetKills".</returns>
    public override string GetProgress()
    {
        return $"{kills}/{targetKills}";
    }

    #endregion
}
