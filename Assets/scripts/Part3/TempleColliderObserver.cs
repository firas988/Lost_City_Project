using UnityEngine;

/// <summary>
/// Observes player collision with temple areas and manages collider behavior based on quest state.
/// Controls access to temple areas based on the TempleKillAllGaurds quest completion.
/// Manages collider state transitions between trigger and solid based on player quest progress.
/// </summary>
public class TempleColliderObserver : MonoBehaviour
{
    #region Private Fields
    [Header("Quest State Tracking")]
    /// <summary>
    /// Flag indicating whether the player has already passed through this temple area.
    /// Prevents repeated processing of the same temple access point.
    /// </summary>
    private bool playerPassed = false;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Updates the collider trigger state each frame.
    /// Ensures the collider remains in trigger mode until player meets quest requirements.
    /// </summary>
    void Update()
    {
        // Keep collider as trigger until player has passed through
        if (!playerPassed)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    /// <summary>
    /// Handles player entry into the temple trigger area.
    /// Evaluates quest completion status and adjusts collider behavior accordingly.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    void OnTriggerEnter(Collider other)
    {
        // Skip processing if player has already passed through
        if (playerPassed)
        {
            return;
        }

        // Check if the entering object is the player with the required quest
        if (
            !(
                other.gameObject.tag == "Player"
                && other.gameObject.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                    is TempleKillAllGaurds
            )
        )
        {
            // Player doesn't have the required quest - make collider solid to block access
            GetComponent<BoxCollider>().isTrigger = false;
        }
        else
        {
            // Player has the required quest - disable collider and mark as passed
            GetComponent<BoxCollider>().enabled = false;
            playerPassed = true;
        }
    }
    #endregion
}
