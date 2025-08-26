using UnityEngine;

/// <summary>
/// Tracks the total distance traveled by the player during the current game session.
/// Calculates movement distance by measuring position changes between frames.
/// Ignores vertical movement to focus on horizontal travel distance.
/// </summary>
public class PlayerDistanceTracker : MonoBehaviour
{
    #region Distance Tracking
    /// <summary>
    /// The player's position from the previous frame for distance calculation.
    /// Stored as a 2D position (X, Z) to ignore vertical movement.
    /// </summary>
    private Vector3 lastPosition;

    /// <summary>
    /// Accumulated total distance traveled during the current session.
    /// Continuously updated as the player moves through the world.
    /// </summary>
    private float totalDistance;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the distance tracking system.
    /// Sets the starting position and resets the distance counter.
    /// </summary>
    void Start()
    {
        // Initialize starting position (ignore Y coordinate for 2D tracking)
        lastPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        // Reset distance counter for new session
        totalDistance = 0f;
    }

    /// <summary>
    /// Updates distance tracking each frame by measuring position changes.
    /// Calculates the distance moved since last frame and adds to total.
    /// </summary>
    void Update()
    {
        // Get current position (ignore Y coordinate for 2D tracking)
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        // Calculate distance moved since last frame
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);

        // Add to total distance and update last position
        totalDistance += distanceMoved;
        lastPosition = currentPosition;
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Gets the total distance traveled during the current game session.
    /// </summary>
    /// <returns>The accumulated distance in world units.</returns>
    public float getDistance()
    {
        return totalDistance;
    }
    #endregion
}
