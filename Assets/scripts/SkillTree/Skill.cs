using UnityEngine;

/// <summary>
/// Defines the different types of skills available in the skill tree system.
/// </summary>
public enum SkillType { Strength, Accuracy, Speed, Health, Defense }

/// <summary>
/// ScriptableObject representing a skill that can be purchased and applied to the player.
/// Defines skill properties including type, bonus value, and cost.
/// </summary>
[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    /// <summary>
    /// The display name of the skill.
    /// </summary>
    [SerializeField]
    private string skillName;

    /// <summary>
    /// The type/category of the skill (Strength, Accuracy, Speed, Health, Defense).
    /// </summary>
    [SerializeField]
    private SkillType type;

    /// <summary>
    /// The numerical bonus value this skill provides when applied.
    /// </summary>
    [SerializeField]
    private float bonus;

    /// <summary>
    /// The cost in skill points required to purchase this skill.
    /// </summary>
    [SerializeField]
    private int cost;

    /// <summary>
    /// Gets the type/category of this skill.
    /// </summary>
    public SkillType SkillType { get { return this.type; } }

    /// <summary>
    /// Gets the bonus value this skill provides when applied.
    /// </summary>
    public float Bonus { get { return this.bonus; } }

    /// <summary>
    /// Gets the cost in skill points required to purchase this skill.
    /// </summary>
    public int Cost { get { return this.cost; } }
}
