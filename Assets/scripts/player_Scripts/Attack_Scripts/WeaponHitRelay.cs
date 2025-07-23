using System;
using UnityEngine;

/// <summary>
/// Handles weapon collision detection and relays the hit event to subscribed listeners.
/// </summary>
public class WeaponHitRelay : MonoBehaviour
{
    /// ===== INSTANCE VARIABLES =====
    /// <summary>
    /// Event triggered when the weapon hits a collider.
    /// </summary>
    private Action<Collider> onHit;

    /// <summary>
    /// Reference to the weapon's collider component.
    /// </summary>
    private Collider weaponCollider;

    /// ===== METHODS =====
    /// <summary>
    /// Initializes the collider reference and logs a warning if not found.
    /// </summary>
    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider == null)
            Debug.LogWarning("No Collider found on weapon!");
    }

    /// <summary>
    /// Called automatically by Unity when the weapon collider enters another collider.
    /// Invokes the hit event if there are listeners.
    /// </summary>
    /// <param name="other">The collider that was hit.</param>
    private void OnTriggerEnter(Collider other)
    {
        onHit?.Invoke(other); // Invoke the hit callback if subscribed
    }

    /// <summary>
    /// Subscribes a callback method to the hit event.
    /// </summary>
    /// <param name="callback">The method to call when a hit occurs.</param>
    public void Subscribe(Action<Collider> callback)
    {
        onHit += callback;
    }

    /// <summary>
    /// Unsubscribes a callback method from the hit event.
    /// </summary>
    /// <param name="callback">The method to remove from the invocation list.</param>
    public void Unsubscribe(Action<Collider> callback)
    {
        onHit -= callback;
    }

    /// <summary>
    /// Enables the weapon's collider to start detecting collisions.
    /// </summary>
    public void EnableCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    /// <summary>
    /// Disables the weapon's collider to stop detecting collisions.
    /// </summary>
    public void DisableCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
}
