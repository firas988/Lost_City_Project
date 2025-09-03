using System;
using TMPro;
using UniRx;
using UnityEngine;

/// <summary>
/// Manages individual notification UI elements with show/hide animations.
/// Handles subtitle text display and animation state management.
/// Provides simple interface for displaying and hiding notification messages.
/// </summary>
public class Notification : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Text Components")]
    /// <summary>
    /// Text component for displaying the notification subtitle.
    /// Shows additional information below the main notification message.
    /// </summary>
    [SerializeField]
    private TMP_Text textSubtitle;
    #endregion

    #region Private Fields
    [Header("Animation Components")]
    /// <summary>
    /// Animator component controlling the notification's show/hide animations.
    /// Manages the visual transitions between visible and hidden states.
    /// </summary>
    private Animator animator;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the notification by getting the animator component.
    /// Sets up the animation system for show/hide functionality.
    /// </summary>
    void Awake()
    {
        // Get the animator component for controlling notification animations
        animator = this.GetComponent<Animator>();
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Sets the subtitle text for the notification.
    /// Updates the display text without triggering animations.
    /// </summary>
    /// <param name="subtitle">The text to display as subtitle in the notification.</param>
    public void SetSubtitle(string subtitle) => textSubtitle.text = subtitle;

    /// <summary>
    /// Shows the notification by setting the animator to visible state.
    /// Triggers the show animation to make the notification appear.
    /// </summary>
    public void Show()
    {
        // Set animator parameter to trigger show animation
        animator.SetBool("isVisible", true);
    }

    /// <summary>
    /// Hides the notification by setting the animator to hidden state.
    /// Triggers the hide animation to make the notification disappear.
    /// </summary>
    public void Hide()
    {
        // Set animator parameter to trigger hide animation
        animator.SetBool("isVisible", false);
    }
    #endregion
}
