using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ScriptableObject representing a collection of skills for a specific skill tree category.
/// Manages skill progression, level tracking, and bonus calculations for different skill types.
/// Provides centralized management for skill tree progression and UI coordination.
/// </summary>
[CreateAssetMenu(fileName = "New Skill List", menuName = "Skill List")]
[System.Serializable]
public class SkillList : ScriptableObject
{
    #region Serialized Fields
    [Header("Skill Collection")]
    /// <summary>
    /// List of skills available in this skill tree category.
    /// Contains all skill definitions including costs, bonuses, and progression data.
    /// </summary>
    [SerializeField]
    private List<Skill> skills;

    [Header("Skill Tree Configuration")]
    /// <summary>
    /// Maximum level that can be achieved in this skill tree.
    /// Controls the total number of skills that can be purchased.
    /// </summary>
    [SerializeField]
    private int maxLevel;

    /// <summary>
    /// Total bonus accumulated from all purchased skills in this category.
    /// Represents the cumulative effect of all acquired skills.
    /// </summary>
    [SerializeField]
    private float totalBonus;

    /// <summary>
    /// Current level achieved in this skill tree.
    /// Tracks progress through the skill tree progression.
    /// </summary>
    [SerializeField]
    private int currentLevel;

    [Header("Skill Type and Management")]
    /// <summary>
    /// The type of skills contained in this list (Strength, Speed, Health, Defense).
    /// Determines which player stats are affected by skill bonuses.
    /// </summary>
    [SerializeField]
    private SkillType skillType;

    /// <summary>
    /// Reference to the skill amount limit for managing skill point spending.
    /// Coordinates skill point allocation and validation.
    /// </summary>
    private SkillAmountLimit skillAmountLimit;

    /// <summary>
    /// List of UI buttons for the skill tree.
    /// Provides interactive elements for skill purchases and visual feedback.
    /// </summary>
    private List<Button> skillTreeButtons;
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Initializes the skill list with a UI component and maximum level.
    /// Sets up reactive subscriptions for level change events and restores saved progress.
    /// </summary>
    /// <param name="currentSavedLevel">The current level of the skill tree from saved data.</param>
    /// <param name="maxLevel">The maximum level that can be achieved in this skill tree.</param>
    /// <param name="skillTreeButtons">List of UI buttons for the skill tree.</param>
    /// <param name="skillAmountLimit">Reference to the skill amount limit.</param>
    public void Init(
        int currentSavedLevel,
        int maxLevel,
        List<Button> skillTreeButtons,
        SkillAmountLimit skillAmountLimit
    )
    {
        // Set up skill tree configuration
        this.maxLevel = maxLevel;
        this.skillAmountLimit = skillAmountLimit;
        this.skillTreeButtons = skillTreeButtons;

        // Initialize progress tracking
        this.currentLevel = 0;
        this.totalBonus = 0;

        // Restore saved skill progress by incrementing buttons
        for (int i = 0; i < currentSavedLevel; i++)
        {
            skillTreeButtons[i].GetComponentInParent<SkillTreeButton>().Increment(true);
        }
    }
    #endregion

    #region Upgrade Logic Methods
    /// <summary>
    /// Attempts to upgrade the skill tree to the next level.
    /// Applies the appropriate bonus based on the skill type and updates total bonus.
    /// </summary>
    /// <returns>True if the upgrade was successful, false if already at max level.</returns>
    public bool Upgrade()
    {
        // Update skill point spending and increment level
        skillAmountLimit.UpdateSpent(currentCost);
        currentLevel++;
        return true;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the cost of the next skill upgrade in this skill tree.
    /// </summary>
    public int currentCost
    {
        get { return this.skills[currentLevel].Cost; }
    }

    /// <summary>
    /// Gets the bonus value of the next skill upgrade in this skill tree.
    /// </summary>
    public float currentBonus
    {
        get { return this.skills[currentLevel].Bonus; }
    }
    #endregion

    #region Public API Methods
    /// <summary>
    /// Gets the current level achieved in this skill tree.
    /// </summary>
    /// <returns>The current level of the skill tree.</returns>
    public int getCurrentLevel()
    {
        return this.currentLevel;
    }

    /// <summary>
    /// Gets the maximum level that can be achieved in this skill tree.
    /// </summary>
    /// <returns>The maximum level of the skill tree.</returns>
    public int getMaxLevel()
    {
        return this.maxLevel;
    }

    /// <summary>
    /// Sets the current level of the skill tree.
    /// </summary>
    /// <param name="level">The new level to set for the skill tree.</param>
    public void setCurrentLevel(int level)
    {
        this.currentLevel = level;
    }

    /// <summary>
    /// Gets the list of UI buttons for the skill tree.
    /// </summary>
    /// <returns>List of UI buttons associated with this skill tree.</returns>
    public List<Button> getSkillTreeButtons()
    {
        return this.skillTreeButtons;
    }

    /// <summary>
    /// Checks if the skill tree has reached its maximum level.
    /// </summary>
    /// <returns>True if at maximum level, false otherwise.</returns>
    public bool isMaxLevel()
    {
        return this.currentLevel >= this.maxLevel;
    }

    /// <summary>
    /// Gets the type of skills contained in this list.
    /// </summary>
    /// <returns>The skill type for this skill tree.</returns>
    public SkillType getSkillType()
    {
        return this.skillType;
    }
    #endregion
}
