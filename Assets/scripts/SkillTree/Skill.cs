using UnityEngine;

/// <summary>
/// Defines the different types of skills available in the skill tree system.
/// Categorizes skills by their primary effect on player attributes.
/// </summary>
public enum SkillType
{
    /// <summary>
    /// Skills that increase player damage output and physical strength.
    /// </summary>
    Strength,

    /// <summary>
    /// Skills that increase player movement speed and agility.
    /// </summary>
    Speed,

    /// <summary>
    /// Skills that increase player maximum health and survivability.
    /// </summary>
    Health,

    /// <summary>
    /// Skills that increase player damage resistance and protection.
    /// </summary>
    Defense,
}

/// <summary>
/// ScriptableObject representing a skill that can be purchased and applied to the player.
/// Defines skill properties including type, bonus value, and cost.
/// Provides the foundation for skill tree progression and player character development.
/// </summary>
[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    #region Serialized Fields
    [Header("Skill Information")]
    /// <summary>
    /// The display name of the skill.
    /// Shown to the player in the skill tree interface.
    /// </summary>
    [SerializeField]
    private string skillName;

    [Header("Skill Properties")]
    /// <summary>
    /// The type/category of the skill (Strength, Speed, Health, Defense).
    /// Determines which player attribute is affected by this skill.
    /// </summary>
    [SerializeField]
    private SkillType type;

    /// <summary>
    /// The numerical bonus value this skill provides when applied.
    /// Represents the magnitude of the skill's effect on player attributes.
    /// </summary>
    [SerializeField]
    private float bonus;

    /// <summary>
    /// The cost in skill points required to purchase this skill.
    /// Controls skill tree progression and resource management.
    /// </summary>
    [SerializeField]
    private int cost;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the type/category of this skill.
    /// </summary>
    /// <returns>The skill type that determines the attribute affected.</returns>
    public SkillType SkillType
    {
        get { return this.type; }
    }

    /// <summary>
    /// Gets the bonus value this skill provides when applied.
    /// </summary>
    /// <returns>The numerical bonus value for the affected attribute.</returns>
    public float Bonus
    {
        get { return this.bonus; }
    }

    /// <summary>
    /// Gets the cost in skill points required to purchase this skill.
    /// </summary>
    /// <returns>The skill point cost for purchasing this skill.</returns>
    public int Cost
    {
        get { return this.cost; }
    }
    #endregion
}
