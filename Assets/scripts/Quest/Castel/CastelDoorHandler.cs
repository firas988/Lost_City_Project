using System;
using UnityEngine;

/// <summary>
/// Manages castle door animations and trigger events for quest progression.
/// Handles door opening animations and player interaction triggers.
/// Provides event subscription system for quest completion coordination.
/// </summary>
public class CastelDoorHandler : MonoBehaviour
{
    #region Private Fields
    [Header("Animation Components")]
    /// <summary>
    /// Animator component controlling the door's open/close animations.
    /// Manages the visual transitions for door states.
    /// </summary>
    private Animator animator;

    [Header("Event Management")]
    /// <summary>
    /// Action delegate for handling player trigger events.
    /// Subscribed to by quest systems for quest completion triggers.
    /// </summary>
    private Action onTriggerEnter;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the castle door handler by getting the animator component.
    /// Sets up the animation system for door control.
    /// </summary>
    void Start()
    {
        // Get the animator component for controlling door animations
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Update method - currently unused but kept for future functionality.
    /// </summary>
    void Update() { }
    #endregion

    #region Trigger Event Methods
    /// <summary>
    /// Handles player entry into the door trigger area.
    /// Invokes the subscribed trigger event when player enters.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the subscribed trigger event for quest coordination
            onTriggerEnter?.Invoke();
        }
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Subscribes an action to the door trigger event.
    /// Allows quest systems to respond to player interaction with the door.
    /// </summary>
    /// <param name="onTriggerEnter">The action to execute when player enters the trigger.</param>
    public void subscribeToOnTriggerEnter(Action onTriggerEnter)
    {
        this.onTriggerEnter += onTriggerEnter;
    }

    /// <summary>
    /// Opens the castle door by triggering the open animation.
    /// Activates the door opening sequence through the animator.
    /// </summary>
    public void openTheDoor()
    {
        // Trigger the door opening animation
        animator.SetTrigger("open");
    }
    #endregion
}
