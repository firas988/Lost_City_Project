using UnityEngine;

/// <summary>
/// Triggers dungeon progression when the player enters a room.
/// Coordinates with DungeonManager to start room encounters.
/// Manages room entry detection and progression triggers.
/// </summary>
public class RoomTrigger : MonoBehaviour
{
    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the dungeon manager for coordinating room progression.
    /// Used to trigger room start and manage dungeon state.
    /// </summary>
    private DungeonManager dungeonManager;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the room trigger by finding the dungeon manager.
    /// Sets up the reference needed for room progression coordination.
    /// </summary>
    void Start()
    {
        // Find and store reference to the dungeon manager
        dungeonManager = GameObject.Find("dungeon").GetComponent<DungeonManager>();
    }

    /// <summary>
    /// Handles player entry into the room trigger area.
    /// Triggers room progression and disables the trigger to prevent re-triggering.
    /// </summary>
    /// <param name="other">The collider that entered the trigger area.</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.gameObject.tag == "Player")
        {
            // Trigger room start in the dungeon manager
            dungeonManager.StartDungeon();

            // Disable the trigger to prevent multiple activations
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
    #endregion
}
