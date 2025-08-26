using System;
using UnityEngine;

/// <summary>
/// Handles weapon collision detection and relays the hit event to subscribed listeners.
/// Manages weapon collider activation/deactivation and provides an event system for hit detection.
/// Integrates with the attack system to coordinate collision detection during attack animations.
/// </summary>
public class WeaponHitRelay : MonoBehaviour
{
    #region Event System
    /// <summary>
    /// Event triggered when the weapon hits a collider.
    /// Subscribed to by the PlayerAttackController for damage processing.
    /// </summary>
    private Action<Collider> onHit;
    #endregion

    #region Component References
    /// <summary>
    /// Reference to the weapon's collider component for collision detection.
    /// Used to enable/disable hit detection during attack sequences.
    /// </summary>
    private Collider weaponCollider;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the collider reference and logs a warning if not found.
    /// Sets up the weapon collision detection system.
    /// </summary>
    private void Awake()
    {
        // Get the collider component for this weapon
        weaponCollider = GetComponent<Collider>();

        // Warn if no collider is found (required for hit detection)
        if (weaponCollider == null)
            Debug.LogWarning("No Collider found on weapon!");
    }
    #endregion

    #region Collision Detection
    /// <summary>
    /// Called automatically by Unity when the weapon collider enters another collider.
    /// Invokes the hit event if there are listeners subscribed.
    /// </summary>
    /// <param name="other">The collider that was hit by the weapon.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Invoke the hit callback if any listeners are subscribed
        onHit?.Invoke(other);
    }
    #endregion

    #region Event Subscription Management
    /// <summary>
    /// Subscribes a callback method to the hit event.
    /// Allows other components to receive notifications when the weapon hits something.
    /// </summary>
    /// <param name="callback">The method to call when a hit occurs.</param>
    public void Subscribe(Action<Collider> callback)
    {
        // Add the callback to the hit event
        onHit += callback;
    }

    /// <summary>
    /// Unsubscribes a callback method from the hit event.
    /// Removes the callback from receiving hit notifications.
    /// </summary>
    /// <param name="callback">The method to remove from the invocation list.</param>
    public void Unsubscribe(Action<Collider> callback)
    {
        // Remove the callback from the hit event
        onHit -= callback;
    }
    #endregion

    #region Collider Control
    /// <summary>
    /// Enables the weapon's collider to start detecting collisions.
    /// Called during attack animations to activate hit detection.
    /// </summary>
    public void EnableCollider()
    {
        // Enable collision detection if collider exists
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    /// <summary>
    /// Disables the weapon's collider to stop detecting collisions.
    /// Called after attack animations to deactivate hit detection.
    /// </summary>
    public void DisableCollider()
    {
        // Disable collision detection if collider exists
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
    #endregion
}
