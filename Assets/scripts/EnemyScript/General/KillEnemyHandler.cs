using System;
using UnityEngine;

/// <summary>
/// Static event handler for enemy death notifications.
/// Provides a centralized system for other components to subscribe to enemy death events.
/// Uses the Observer pattern to decouple enemy death logic from other systems.
/// </summary>
public static class KillEnemyHandler
{
    #region Event Management
    /// <summary>Static event that is invoked when an enemy is killed.</summary>
    private static Action<string> onKilledEnemy;
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Notifies all subscribers that an enemy with the specified tag has been killed.
    /// </summary>
    /// <param name="enemyTag">The tag of the enemy that was killed.</param>
    public static void KilledEnemy(string enemyTag)
    {
        // Invoke the event if there are any subscribers
        onKilledEnemy?.Invoke(enemyTag);
    }

    /// <summary>
    /// Subscribes a callback function to the enemy death event.
    /// </summary>
    /// <param name="callback">The function to call when an enemy is killed.</param>
    public static void Subscribe(Action<string> callback)
    {
        onKilledEnemy += callback;
    }

    /// <summary>
    /// Unsubscribes a callback function from the enemy death event.
    /// </summary>
    /// <param name="callback">The function to remove from the event.</param>
    public static void Unsubscribe(Action<string> callback)
    {
        onKilledEnemy -= callback;
    }
    #endregion
}
