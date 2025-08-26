using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

/// <summary>
/// Handles player water interaction and respawn mechanics when entering water areas.
/// Automatically respawns the player at a designated respawn point when they fall into water.
/// Manages CharacterController state during respawn to prevent physics conflicts.
/// </summary>
public class EnteredTheWater : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to the player GameObject for position and component management.
    /// Used to access player transform and CharacterController for respawn operations.
    /// </summary>
    private GameObject player;
    #endregion

    #region Serialized Fields
    /// <summary>
    /// Designated respawn point GameObject where the player will be teleported.
    /// Should be assigned in the inspector to define the safe respawn location.
    /// </summary>
    [SerializeField]
    private GameObject respawn;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the water handler by finding the player reference and validating setup.
    /// Locates the player GameObject and checks that required components are properly assigned.
    /// </summary>
    void Awake()
    {
        // Find the player GameObject in the scene
        player = GameObject.FindGameObjectWithTag("Player");

        // Validate that required components are assigned
        if (player == null)
        {
            // Player object not found - this is a critical setup error
        }
        if (respawn == null)
        {
            // Respawn point not assigned - this will prevent respawn functionality
        }
    }
    #endregion

    #region Water Interaction
    /// <summary>
    /// Detects when the player enters the water trigger area and initiates respawn sequence.
    /// Temporarily disables CharacterController, teleports player to respawn point, then re-enables controller.
    /// </summary>
    /// <param name="other">The collider that entered the water trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered water and all required components are available
        if (other.CompareTag("Player") && player != null && respawn != null)
        {
            // Temporarily disable CharacterController to prevent physics conflicts during teleport
            player.GetComponent<CharacterController>().enabled = false;

            // Teleport player to respawn point with matching position and rotation
            player.transform.position = respawn.transform.position;
            player.transform.rotation = respawn.transform.rotation;

            // Re-enable CharacterController to restore normal player movement
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
    #endregion
}
