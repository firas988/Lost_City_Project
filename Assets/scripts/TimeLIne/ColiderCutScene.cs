using System;
using UnityEngine;

/// <summary>
/// Provides a trigger-based cutscene activation system using Unity's trigger system.
/// Allows other components to subscribe to player trigger events for cutscene initiation.
/// Implements a simple event system for cutscene coordination without tight coupling.
/// </summary>
public class ColiderCutScene : MonoBehaviour
{
    #region Event Management
    /// <summary>
    /// Action delegate that gets invoked when the player enters the trigger area.
    /// Subscribed components can use this to trigger cutscene sequences or other events.
    /// </summary>
    private Action onTriggerEnter;
    #endregion

    #region Public Interface
    /// <summary>
    /// Allows other components to subscribe to the trigger enter event.
    /// Provides a way for cutscene managers to register callback methods for trigger activation.
    /// </summary>
    /// <param name="onTriggerEnter">The action to execute when the player enters the trigger.</param>
    public void subscribeToOnTriggerEnter(Action onTriggerEnter)
    {
        this.onTriggerEnter += onTriggerEnter;
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Unity trigger event that detects when objects enter the trigger area.
    /// Checks if the entering object is the player and invokes subscribed actions accordingly.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger events for player objects
        if (other.CompareTag("Player"))
        {
            // Invoke all subscribed actions when player enters trigger
            onTriggerEnter?.Invoke();
        }
    }
    #endregion
}
