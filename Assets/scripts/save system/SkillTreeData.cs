using UnityEngine;

/// <summary>
/// Serializable data structure for storing skill tree progression and state.
/// Captures skill points, spending, and individual skill level information.
/// Used by the save system to persist player skill development across game sessions.
/// </summary>
[System.Serializable]
public class SkillTreeData
{
    #region Skill Point Management
    /// <summary>
    /// Total number of skill points available to the player.
    /// Represents the maximum skill points that can be earned.
    /// </summary>
    [SerializeField]
    private int totalSkillPoints;

    /// <summary>
    /// Number of skill points that have been spent on skill upgrades.
    /// Used to calculate remaining available skill points.
    /// </summary>
    [SerializeField]
    private int spent;
    #endregion

    #region Skill Levels
    /// <summary>
    /// Current level of the strength skill.
    /// Determines player's damage output and combat effectiveness.
    /// </summary>
    [SerializeField]
    private int strengthLevel;

    /// <summary>
    /// Current level of the speed skill.
    /// Determines player's movement speed and agility.
    /// </summary>
    [SerializeField]
    private int speedLevel;

    /// <summary>
    /// Current level of the defense skill.
    /// Determines player's damage resistance and survivability.
    /// </summary>
    [SerializeField]
    private int defenseLevel;

    /// <summary>
    /// Current level of the health skill.
    /// Determines player's maximum health and regeneration.
    /// </summary>
    [SerializeField]
    private int healthLevel;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new SkillTreeData instance by extracting data from a SkillTreeManager.
    /// Captures current skill point status and individual skill levels.
    /// </summary>
    /// <param name="skillTreeManager">The SkillTreeManager component to extract data from.</param>
    public SkillTreeData(SkillTreeManager skillTreeManager)
    {
        // Extract skill point management data
        this.totalSkillPoints = skillTreeManager.getSkillAmountLimit().GetTotalSkillPoints();
        this.spent = skillTreeManager.getSkillAmountLimit().GetTotalSpent();

        // Extract individual skill level data
        this.strengthLevel = skillTreeManager.getStrengthLevel();
        this.speedLevel = skillTreeManager.getSpeedLevel();
        this.defenseLevel = skillTreeManager.getDefenseLevel();
        this.healthLevel = skillTreeManager.getHealthLevel();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the total number of skill points available to the player.
    /// </summary>
    public int TotalSkillPoints => totalSkillPoints;

    /// <summary>
    /// Gets the number of skill points that have been spent.
    /// </summary>
    public int Spent => spent;

    /// <summary>
    /// Gets the current level of the strength skill.
    /// </summary>
    public int StrengthLevel => strengthLevel;

    /// <summary>
    /// Gets the current level of the speed skill.
    /// </summary>
    public int SpeedLevel => speedLevel;

    /// <summary>
    /// Gets the current level of the defense skill.
    /// </summary>
    public int DefenseLevel => defenseLevel;

    /// <summary>
    /// Gets the current level of the health skill.
    /// </summary>
    public int HealthLevel => healthLevel;
    #endregion
}
