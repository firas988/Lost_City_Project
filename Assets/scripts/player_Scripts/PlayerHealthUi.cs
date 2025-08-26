using UnityEngine;

/// <summary>
/// Manages the player health bar UI display and updates.
/// Continuously monitors player health and updates the progress bar visualization.
/// Provides real-time health status feedback to the player.
/// </summary>
public class PlayerHealthUi : MonoBehaviour
{
    #region UI Components
    /// <summary>
    /// Reference to the ProgressBar component for health visualization.
    /// Displays current health as a percentage of maximum health.
    /// </summary>
    private ProgressBar progressBar;
    #endregion

    #region Player Reference
    /// <summary>
    /// Reference to the Player instance for health status monitoring.
    /// Used to get current and maximum health values for UI updates.
    /// </summary>
    private Player player;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the health UI by getting required component references.
    /// Sets up the progress bar and player instance connections.
    /// </summary>
    void Start()
    {
        // Get the ProgressBar component for health visualization
        progressBar = GetComponent<ProgressBar>();

        // Find the player instance for health monitoring
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
    }

    /// <summary>
    /// Updates the health bar display each frame with current player health.
    /// Ensures player reference exists and updates progress bar accordingly.
    /// </summary>
    void Update()
    {
        // Ensure player reference exists (fallback if lost)
        if (player == null)
        {
            player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
        }
        else
        {
            // Update progress bar with current health percentage
            progressBar.SetProgress(player.getHealth() / player.getMaxHealth());
        }
    }
    #endregion
}
