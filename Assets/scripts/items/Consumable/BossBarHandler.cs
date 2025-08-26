using UnityEngine;

/// <summary>
/// Manages the boss health bar UI display and visibility.
/// Controls the progress bar component and provides methods to show/hide the boss bar.
/// Used during boss encounters to display current boss health status.
/// </summary>
public class BossBarHandler : MonoBehaviour
{
    #region UI Components
    /// <summary>
    /// Reference to the progress bar component that displays the boss health.
    /// Used to update the visual representation of boss health.
    /// </summary>
    private ProgressBar progressBar;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the progress bar reference by finding it in the children components.
    /// </summary>
    void Start()
    {
        // Get the ProgressBar component from child objects
        progressBar = GetComponentInChildren<ProgressBar>();
    }
    #endregion

    #region Health Bar Management
    /// <summary>
    /// Updates the boss health bar to reflect the current health value.
    /// </summary>
    /// <param name="health">The current health value to display (typically as a percentage).</param>
    public void TakeDamage(float health)
    {
        // Update the progress bar with the new health value
        progressBar.SetProgress(health);
    }

    /// <summary>
    /// Hides the boss health bar by deactivating the GameObject.
    /// Called when the boss is defeated or the encounter ends.
    /// </summary>
    public void hideBar()
    {
        // Deactivate the boss bar GameObject
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the boss health bar by activating the GameObject.
    /// Called when a boss encounter begins.
    /// </summary>
    public void showBar()
    {
        // Activate the boss bar GameObject
        this.gameObject.SetActive(true);
    }
    #endregion
}
