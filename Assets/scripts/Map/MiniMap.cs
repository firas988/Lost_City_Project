using UnityEngine;

/// <summary>
/// Manages the minimap camera positioning and rotation to follow the player.
/// Positions the minimap camera above the player at a fixed height and rotates it to match the main camera's yaw.
/// Provides an overhead view of the player's location for navigation purposes.
/// </summary>
public class MiniMap : MonoBehaviour
{
    #region GameObject References
    /// <summary>
    /// Reference to the player GameObject for tracking position.
    /// Used to position the minimap camera above the player.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Tag used to identify the main camera GameObject in the scene.
    /// </summary>
    private string mainCameraTag = "MainCamera";

    /// <summary>
    /// Reference to the main camera GameObject for rotation synchronization.
    /// Used to match the minimap camera's yaw rotation with the main camera.
    /// </summary>
    private GameObject mainCamera;
    #endregion

    #region Positioning Configuration
    /// <summary>
    /// Fixed height above the player for the minimap camera.
    /// Provides an overhead view while maintaining consistent altitude.
    /// </summary>
    private float yPosition = 200f;

    /// <summary>
    /// Temporary vector for calculating new minimap camera position.
    /// Used to update position each frame without creating new Vector3 objects.
    /// </summary>
    private Vector3 newPosition;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by finding the player and main camera references.
    /// Sets up the required GameObject references for position and rotation tracking.
    /// </summary>
    void Start()
    {
        // Find player GameObject for position tracking
        player = GameObject.FindGameObjectWithTag(playerTag);

        // Find main camera GameObject for rotation synchronization
        mainCamera = GameObject.FindGameObjectWithTag(mainCameraTag);
    }

    /// <summary>
    /// Updates the minimap camera position and rotation each frame.
    /// Follows the player's position and synchronizes rotation with the main camera.
    /// </summary>
    void Update()
    {
        // Update minimap camera position to follow player
        newPosition = player.transform.position;
        newPosition.y = yPosition; // Set fixed height above player
        transform.position = newPosition;

        // Synchronize minimap camera rotation with main camera
        // Keep 90-degree tilt for overhead view, match main camera's yaw rotation
        transform.rotation = Quaternion.Euler(90f, mainCamera.transform.eulerAngles.y, 0f);
    }
    #endregion
}
