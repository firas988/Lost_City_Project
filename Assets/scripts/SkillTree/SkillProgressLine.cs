using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual progress bar for a skill in the skill tree.
/// Updates the fill amount of the progress bar image based on skill progress.
/// </summary>
public class SkillProgressLine : MonoBehaviour
{
    [SerializeField]
    private SkillList skillList; // Reference to the associated skill list (not used directly here)

    [SerializeField]
    private GameObject skillProgressBar; // The GameObject containing the progress bar Image

    private Image skillProgressBarImage; // The Image component representing the progress bar fill

    /// <summary>
    /// Initializes the progress bar image reference.
    /// </summary>
    void Start()
    {
        skillProgressBarImage = skillProgressBar.GetComponent<Image>();
    }

    /// <summary>
    /// (Unused) Unity Update method.
    /// </summary>
    void Update()
    {
        // No per-frame logic required
    }

    /// <summary>
    /// Sets the fill amount of the skill progress bar image.
    /// </summary>
    /// <param name="value">A value between 0 and 1 representing progress.</param>
    public void SetSkillProgressBar(float value)
    {
        skillProgressBarImage.fillAmount = value;
    }
}
