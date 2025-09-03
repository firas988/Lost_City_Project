using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual progress bar for a skill in the skill tree.
/// Updates the fill amount of the progress bar image based on skill progress.
/// Provides visual feedback showing skill tree progression and completion status.
/// </summary>
public class SkillProgressLine : MonoBehaviour
{
    #region Serialized Fields
    [Header("Skill References")]
    /// <summary>
    /// Reference to the associated skill list (not used directly here).
    /// Maintains connection to the skill system for potential future functionality.
    /// </summary>
    [SerializeField]
    private SkillList skillList;

    [Header("UI Components")]
    /// <summary>
    /// The GameObject containing the progress bar Image.
    /// Houses the visual progress bar element that gets updated.
    /// </summary>
    [SerializeField]
    private GameObject skillProgressBar;
    #endregion

    #region Private Fields
    [Header("Progress Bar Components")]
    /// <summary>
    /// The Image component representing the progress bar fill.
    /// Controls the visual fill amount of the progress bar.
    /// </summary>
    private Image skillProgressBarImage;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the progress bar image reference.
    /// Gets the Image component from the progress bar GameObject for efficient access.
    /// </summary>
    void Awake()
    {
        // Get the Image component for controlling the progress bar fill
        skillProgressBarImage = skillProgressBar.GetComponent<Image>();
    }
    #endregion

    #region Progress Bar Management Methods
    /// <summary>
    /// Sets the fill amount of the skill progress bar image.
    /// Updates the visual progress to show skill completion status.
    /// </summary>
    /// <param name="value">A value between 0 and 1 representing progress (0 = empty, 1 = full).</param>
    public void SetSkillProgressBar(float value)
    {
        // Ensure the progress bar image reference is valid
        if (skillProgressBarImage != null)
        {
            // Set the fill amount to show progress
            skillProgressBarImage.fillAmount = value;
        }
        else
        {
            // Re-acquire the reference if it was lost and set the fill amount
            skillProgressBarImage = skillProgressBar.GetComponent<Image>();
            skillProgressBarImage.fillAmount = value;
        }
    }
    #endregion
}
