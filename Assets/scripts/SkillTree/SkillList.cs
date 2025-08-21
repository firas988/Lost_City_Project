using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ScriptableObject representing a collection of skills for a specific skill tree category.
/// Manages skill progression, level tracking, and bonus calculations for different skill types.
/// </summary>
[CreateAssetMenu(fileName = "New Skill List", menuName = "Skill List")]
[System.Serializable]
public class SkillList : ScriptableObject
{
    #region Inspector Fields

    /// <summary>
    /// List of skills available in this skill tree category.
    /// </summary>
    [SerializeField]
    private List<Skill> skills;

    /// <summary>
    /// Maximum level that can be achieved in this skill tree.
    /// </summary>
    [SerializeField]
    private int maxLevel;

    /// <summary>
    /// Total bonus accumulated from all purchased skills in this category.
    /// </summary>
    [SerializeField]
    private float totalBonus;

    /// <summary>
    /// Current level achieved in this skill tree.
    /// </summary>
    [SerializeField]
    private int currentLevel;

    private SkillAmountLimit skillAmountLimit;

    private List<Button> skillTreeButtons;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the skill list with a UI component and maximum level.
    /// Sets up reactive subscriptions for level change events.
    /// </summary>
    /// <param name="currentLevel">The current level of the skill tree.</param>
    /// <param name="maxLevel">The maximum level that can be achieved in this skill tree.</param>
    /// <param name="skillTreeButtons">List of UI buttons for the skill tree.</param>
    /// <param name="skillAmountLimit">Reference to the skill amount limit.</param>
    public void Init(
        int currentLevel,
        int maxLevel,
        List<Button> skillTreeButtons,
        SkillAmountLimit skillAmountLimit
    )
    {
        this.maxLevel = maxLevel;
        this.skillAmountLimit = skillAmountLimit;
        this.skillTreeButtons = skillTreeButtons;
        this.currentLevel = currentLevel;
        this.totalBonus = 0;
        Debug.Log("Init SkillList: " + GetType() + " " + currentLevel);

        for (int i = 0; i < currentLevel; i++)
        {
            skillTreeButtons[i].GetComponentInParent<SkillTreeButton>().Increment(true);
            Debug.Log("Incrementing skill " + i);
        }
    }

    #endregion

    #region Upgrade Logic

    /// <summary>
    /// Attempts to upgrade the skill tree to the next level.
    /// Applies the appropriate bonus based on the skill type and updates total bonus.
    /// </summary>
    /// <returns>True if the upgrade was successful, false if already at max level.</returns>
    public bool Upgrade()
    {
        if (currentLevel < maxLevel && skillAmountLimit.CanSpend(currentCost))
        {
            skillAmountLimit.UpdateSpent(currentCost);
            currentLevel++;

            return true;
        }
        return false;
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

    #region Public API

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
    public void setCurrentLevel(int level)
    {
        this.currentLevel = level;
    }

    /// <summary>
    /// Gets the list of UI buttons for the skill tree.
    /// </summary>
    public List<Button> getSkillTreeButtons()
    {
        return this.skillTreeButtons;
    }

    public bool isMaxLevel()
    {
        return this.currentLevel >= this.maxLevel;
    }

    #endregion
}
