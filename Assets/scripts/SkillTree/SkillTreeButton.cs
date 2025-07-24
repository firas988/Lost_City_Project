using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the logic and UI for a single skill button in the skill tree.
/// Manages skill purchase, color state, and button interaction.
/// </summary>
public class SkillTreeButton : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    public Image frameImage; // The frame image of the button

    [SerializeField]
    public Image iconImage; // The icon image of the button

    [SerializeField]
    public SkillList skillList; // Reference to the skill list for this button

    [SerializeField]
    public SkillTreeManager skillTreeManager; // Reference to the skill tree manager

    [SerializeField]
    public SkillAmountLimit skillLimit; // Reference to the skill amount limit

    [SerializeField]
    public Color activeFrameColor; // Color for the frame when active

    [SerializeField]
    public Color disabledFrameColor; // Color for the frame when disabled

    [SerializeField]
    public Color activeIconColor; // Color for the icon when active

    [SerializeField]
    public Color disabledIconColor; // Color for the icon when disabled

    [SerializeField]
    public SkillProgressLine lineToUpdate; // Optional: Progress line to update when skill is purchased
    #endregion

    #region Private Fields

    private bool isSkillPurchased; // Tracks if the skill has been purchased
    private SkillAmountLimit _skillAmountLimit; // Internal reference to skill amount limit
    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the button, sets up listeners and colors.
    /// </summary>
    void Start()
    {
        SetSkillAmountLimit(skillLimit);
        GetComponent<Button>().onClick.AddListener(Increment);
        isSkillPurchased = false;
        SetColors();
    }

    #endregion

    #region Skill Logic

    /// <summary>
    /// Attempts to increment (purchase/upgrade) the skill if possible.
    /// Updates UI and listeners accordingly.
    /// </summary>
    public void Increment()
    {
        if (!CanIncrement())
            return;
        Debug.Log("Incrementing skill " + skillList.getCurrentLevel());

        GetComponent<Button>().enabled = false;
        skillList
            .getSkillTreeButtons()[skillList.getCurrentLevel()]
            .GetComponent<Button>()
            .enabled = true;

        if (skillList.getCurrentLevel() < skillList.getMaxLevel())
        {
            skillList.getSkillTreeButtons()[skillList.getCurrentLevel()].enabled = true;
        }

        isSkillPurchased = skillTreeManager.UpgradeSkill(skillList);
        Debug.Log("Skill upgraded " + isSkillPurchased);
        SetColors();

        if (lineToUpdate != null)
            lineToUpdate.SetSkillProgressBar(1f);
    }

    /// <summary>
    /// Checks if the skill can be incremented (enough points and not already purchased).
    /// </summary>
    private bool CanIncrement() =>
        _skillAmountLimit.CanSpend(skillList.currentCost) && !isSkillPurchased;

    #endregion

    #region Utility Methods

    /// <summary>
    /// Allows setting the skill amount limit from outside.
    /// </summary>
    public void SetSkillAmountLimit(SkillAmountLimit skillAmountLimit) =>
        _skillAmountLimit = skillAmountLimit;

    /// <summary>
    /// Sets the button and icon colors based on whether the skill is purchased.
    /// </summary>
    private void SetColors()
    {
        if (isSkillPurchased)
        {
            frameImage.color = activeFrameColor;
            iconImage.color = activeIconColor;
        }
        else
        {
            frameImage.color = disabledFrameColor;
            iconImage.color = disabledIconColor;
        }
    }

    #endregion
}
