using UnityEngine;

/// <summary>
/// Component that allows animations to continue playing even when the game is paused.
/// Updates the animator using unscaled delta time when Time.timeScale is 0.
/// </summary>
public class IgnoreFreeze : MonoBehaviour
{
    #region Unity Lifecycle Methods

    /// <summary>
    /// Unity Start method - currently unused.
    /// </summary>
    void Start() { }

    /// <summary>
    /// Updates the animator using unscaled delta time when the game is paused.
    /// This ensures animations continue playing even when Time.timeScale is 0.
    /// </summary>
    void Update()
    {
        // Check if the game is paused
        if (Time.timeScale == 0)
        {
            // Update the animator using unscaled delta time to continue animation
            GetComponent<Animator>().Update(Time.unscaledDeltaTime);
        }
    }

    #endregion
}
