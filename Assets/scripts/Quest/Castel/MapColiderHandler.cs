using System;
using UnityEngine;

/// <summary>
/// Handles player collision detection for map part collection quests.
/// Provides subscription-based event system for quest completion triggers.
/// Manages player interaction with map part objects in castle areas.
/// </summary>
public class MapColiderHandler : MonoBehaviour
{
    #region Private Fields
    [Header("Event Management")]
    /// <summary>
    /// Action delegate for handling player trigger events.
    /// Subscribed to by quest systems for map part collection triggers.
    /// </summary>
    private Action onTriggerEnter;
    #endregion

    #region Unity Trigger Methods
    /// <summary>
    /// Handles player entry into the map part trigger area.
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
    /// Subscribes an action to the map part trigger event.
    /// Allows quest systems to respond to player interaction with map parts.
    /// </summary>
    /// <param name="onTriggerEnter">The action to execute when player enters the trigger.</param>
    public void subscribeToOnTriggerEnter(Action onTriggerEnter)
    {
        this.onTriggerEnter += onTriggerEnter;
    }
    #endregion
}
