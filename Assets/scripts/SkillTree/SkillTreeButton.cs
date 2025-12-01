using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the logic and UI for a single skill button in the skill tree.
/// Manages skill purchase, color state, and button interaction.
/// Provides visual feedback and interaction handling for individual skill upgrades.
/// </summary>
public class SkillTreeButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [Header("UI Components")]
    /// <summary>
    /// The frame image of the button that changes color based on skill state.
    /// Provides visual distinction between purchased and unpurchased skills.
    /// </summary>
    [SerializeField]
    public Image frameImage;

    /// <summary>
    /// The icon image of the button that represents the skill type.
    /// Changes color to indicate skill availability and purchase status.
    /// </summary>
    [SerializeField]
    public Image iconImage;

    [Header("Skill References")]
    /// <summary>
    /// Reference to the skill list for this button.
    /// Contains skill data and manages skill progression for this specific skill.
    /// </summary>
    [SerializeField]
    public SkillList skillList;

    /// <summary>
    /// Reference to the skill tree manager for skill upgrade coordination.
    /// Used to trigger skill upgrades and manage skill point spending.
    /// </summary>
    [SerializeField]
    public SkillTreeManager skillTreeManager;

    /// <summary>
    /// Reference to the skill amount limit for checking available skill points.
    /// Validates that the player has enough skill points to purchase the skill.
    /// </summary>
    [SerializeField]
    public SkillAmountLimit skillLimit;

    [Header("Visual States")]
    /// <summary>
    /// Color for the frame when the skill is active/purchased.
    /// Indicates successful skill acquisition to the player.
    /// </summary>
    [SerializeField]
    public Color activeFrameColor;

    /// <summary>
    /// Color for the frame when the skill is disabled/unpurchased.
    /// Indicates skill availability for purchase.
    /// </summary>
    [SerializeField]
    public Color disabledFrameColor;

    /// <summary>
    /// Color for the icon when the skill is active/purchased.
    /// Provides visual confirmation of skill activation.
    /// </summary>
    [SerializeField]
    public Color activeIconColor;

    /// <summary>
    /// Color for the icon when the skill is disabled/unpurchased.
    /// Indicates skill is available for purchase.
    /// </summary>
    [SerializeField]
    public Color disabledIconColor;

    [Header("Progress Visualization")]
    /// <summary>
    /// Optional progress line to update when skill is purchased.
    /// Provides visual feedback showing skill tree progression.
    /// </summary>
    [SerializeField]
    public SkillProgressLine lineToUpdate;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the notifications manager for displaying skill-related messages.
    /// Shows feedback when skills are purchased or when errors occur.
    /// </summary>
    private NotificationsManager notificationsManager;

    /// <summary>
    /// Reference to the audio manager for playing skill-related sounds.
    /// Provides audio feedback for skill purchases and interactions.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Audio source component for playing skill-related sound effects.
    /// Handles local audio playback for this button.
    /// </summary>
    private AudioSource audioSource;

    [Header("Skill State")]
    /// <summary>
    /// Tracks if the skill has been purchased.
    /// Prevents duplicate purchases and controls button interactivity.
    /// </summary>
    private bool isSkillPurchased;

    /// <summary>
    /// Internal reference to skill amount limit.
    /// Cached reference for efficient skill point validation.
    /// </summary>
    private SkillAmountLimit _skillAmountLimit;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the button, sets up listeners and colors.
    /// Finds and stores references to required system components.
    /// </summary>
    void Awake()
    {
        // Find and store references to system managers
        notificationsManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<NotificationsManager>();

        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();

        skillTreeManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<SkillTreeManager>();

        skillLimit = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<SkillAmountLimit>();

        // Get local components and set initial state
        audioSource = GetComponent<AudioSource>();
        SetSkillAmountLimit(skillLimit);
        isSkillPurchased = false;
    }
    #endregion

    #region Skill Logic Methods
    /// <summary>
    /// Attempts to increment (purchase/upgrade) the skill if possible.
    /// Updates UI and listeners accordingly with proper validation.
    /// </summary>
    /// <param name="withoutNotification">Whether to suppress notification messages.</param>
    public void Increment(bool withoutNotification = false)
    {
        // Check if the skill is already purchased
        if (isSkillPurchased)
        {
            notificationsManager.queueTopLeftNotification("Skill already purchased", null);
            audioManager.playUI(audioSource, "Error");
            return;
        }

        // Check if previous skill in the tree is purchased
        if (
            skillList.getSkillTreeButtons().IndexOf(this.gameObject.GetComponent<Button>())
            > skillList.getCurrentLevel()
        )
        {
            notificationsManager.queueTopLeftNotification("Previous skill not purchased", null);
            audioManager.playUI(audioSource, "Error");
            return;
        }

        // Attempt to upgrade the skill through the skill tree manager
        isSkillPurchased = skillTreeManager.UpgradeSkill(skillList);

        // Update visual colors based on purchase status
        SetColors();

        // If the skill was successfully purchased, update the UI
        if (isSkillPurchased)
        {
            // Disable button interaction after purchase
            this.gameObject.GetComponent<Button>().interactable = false;

            // Show success notification and play sound (unless suppressed)
            if (!withoutNotification)
            {
                notificationsManager.queueTopLeftNotification("Skill Upgraded", "None");
                audioManager.playUI(audioSource, "skillupgraded");
            }

            // Update the skill progress bar if available
            if (lineToUpdate != null)
            {
                lineToUpdate.SetSkillProgressBar(1f);
            }
        }
    }

    /// <summary>
    /// Checks if the skill can be incremented (enough points and not already purchased).
    /// </summary>
    /// <returns>True if the skill can be purchased, false otherwise.</returns>
    private bool CanIncrement() => !isSkillPurchased;
    #endregion

    #region Utility Methods
    /// <summary>
    /// Allows setting the skill amount limit from outside.
    /// Provides external control over skill point validation.
    /// </summary>
    /// <param name="skillAmountLimit">The skill amount limit to set.</param>
    public void SetSkillAmountLimit(SkillAmountLimit skillAmountLimit) =>
        _skillAmountLimit = skillAmountLimit;

    /// <summary>
    /// Sets the button and icon colors based on whether the skill is purchased.
    /// Provides visual feedback about skill purchase status.
    /// </summary>
    private void SetColors()
    {
        if (isSkillPurchased)
        {
            // Set active colors for purchased skills
            frameImage.color = activeFrameColor;
            iconImage.color = activeIconColor;
        }
        else
        {
            // Set disabled colors for unpurchased skills
            frameImage.color = disabledFrameColor;
            iconImage.color = disabledIconColor;
        }
    }

    /// <summary>
    /// Sets the skill as purchased and updates the UI.
    /// Used for loading saved skill data and external skill state management.
    /// </summary>
    public void SetPurchasedToTrue()
    {
        // Mark skill as purchased and disable button
        isSkillPurchased = true;
        GetComponent<Button>().interactable = false;

        // Update the skill progress bar if available
        if (lineToUpdate != null)
        {
            lineToUpdate.SetSkillProgressBar(1f);
        }

        // Update visual colors to reflect purchased state
        SetColors();
    }
    #endregion


    #region Button Events 
    /// <summary>
    /// Shows the item tooltip when the pointer enters the slot area.
    /// Displays item information for non-empty slots with valid items.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    // COMPLEXITY ANALYSIS: OnPointerEnter() - O(1)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show tooltip only if slot contains an item
        
            Vector2 mousePos = Input.mousePosition;

        skillTreeManager.ShowSkillToolTip(skillList.Skills[skillList.getSkillTreeButtons().IndexOf(this.gameObject.GetComponent<Button>())],mousePos);
    }

    /// <summary>
    /// Hides the item tooltip when the pointer exits the slot area.
    /// Ensures tooltip is hidden when not hovering over the slot.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    // COMPLEXITY ANALYSIS: OnPointerExit() - O(1)
    public void OnPointerExit(PointerEventData eventData)
    {
        skillTreeManager.HideSkillToolTip();
    }

    #endregion

}
