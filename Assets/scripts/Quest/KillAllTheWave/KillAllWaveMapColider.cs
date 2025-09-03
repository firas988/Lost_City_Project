using System;
using UnityEngine;

/// <summary>
/// Handles player collision detection for wave-based quest completion.
/// Provides subscription-based event system for quest completion triggers.
/// Manages player interaction with wave completion zones.
/// </summary>
public class KillAllWaveMapColider : MonoBehaviour
{
    #region Private Fields
    [Header("Event Management")]
    /// <summary>
    /// Action delegate for handling player entry events.
    /// Subscribed to by quest systems for wave completion triggers.
    /// </summary>
    private Action onEnter;
    #endregion

    #region Unity Trigger Methods
    /// <summary>
    /// Handles player entry into the wave completion trigger area.
    /// Invokes the subscribed onEnter event when player enters.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the subscribed entry event for quest coordination
            onEnter?.Invoke();
        }
    }
    #endregion

    #region Event Management Methods
    /// <summary>
    /// Unsubscribes an action from the onEnter event.
    /// Removes the specified action from the event invocation list.
    /// </summary>
    /// <param name="onEnter">The action to unsubscribe from the event.</param>
    public void unsubscribeToOnEnter(Action onEnter)
    {
        this.onEnter -= onEnter;
    }

    /// <summary>
    /// Subscribes an action to the onEnter event.
    /// Adds the specified action to the event invocation list.
    /// </summary>
    /// <param name="onEnter">The action to subscribe to the event.</param>
    public void subscribeToOnEnter(Action onEnter)
    {
        this.onEnter += onEnter;
    }
    #endregion
}
