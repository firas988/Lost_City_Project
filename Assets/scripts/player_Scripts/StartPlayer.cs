using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player initialization, loading, and cutscene state tracking.
/// Handles player data loading, spawn point positioning, and cutscene completion states.
/// Coordinates with scene management and player data persistence systems.
/// </summary>
public class StartPlayer : MonoBehaviour
{
    #region Player Instance
    /// <summary>
    /// The Player instance containing all player data, stats, and inventory.
    /// Created on Awake and managed throughout the game session.
    /// </summary>
    private Player player;
    #endregion

    #region Spawn Point Configuration
    /// <summary>
    /// Tag used to identify the player spawn point GameObject in the scene.
    /// Used for positioning the player when loading or respawning.
    /// </summary>
    private string spawnPointTag = "Respawn";
    #endregion

    #region Cutscene State Tracking
    /// <summary>
    /// Tracks whether the first part cutscene has been completed.
    /// Used to determine if cutscene should play on scene load.
    /// </summary>
    private bool isCutScenePart1Completed = false;

    /// <summary>
    /// Tracks whether the second part cutscene has been completed.
    /// Used to determine if cutscene should play on scene load.
    /// </summary>
    private bool isCutScenePart2Completed = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the player instance when the component is created.
    /// Creates a new Player object for the current game session.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(1)
    void Awake()
    {
        // Create new player instance for this session
        player = new Player();
    }
    #endregion

    #region Player Access
    /// <summary>
    /// Gets the Player instance for external access.
    /// </summary>
    /// <returns>The Player instance containing all player data.</returns>
    // COMPLEXITY ANALYSIS: getPlayer() - O(1)
    public Player getPlayer()
    {
        return player;
    }
    #endregion

    #region Player Loading System
    /// <summary>
    /// Loads player data and positions the player appropriately based on scene and data.
    /// Handles spawn point positioning, cutscene state, and scene-specific logic.
    /// </summary>
    /// <param name="playerData">Saved player data to load from, or null for new game.</param>
    // COMPLEXITY ANALYSIS: loadPlayer() - O(1)
    public void loadPlayer(PlayerData playerData)
    {
        // Handle case when no data exists or scene doesn't match saved data
        if (playerData == null || playerData.SceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            try
            {
                // Disable character controller during positioning
                gameObject.GetComponent<CharacterController>().enabled = false;

                // Position player at spawn point
                gameObject.transform.position = GameObject
                    .FindWithTag(spawnPointTag)
                    .transform.position;
                gameObject.transform.rotation = GameObject
                    .FindWithTag(spawnPointTag)
                    .transform.rotation;

                // Re-enable character controller
                gameObject.GetComponent<CharacterController>().enabled = true;
            }
            catch (System.Exception)
            {
                Debug.Log("No spawn point found");
            }
        }
        else
        {
            // Disable character controller during positioning
            gameObject.GetComponent<CharacterController>().enabled = false;

            // Special handling for scene 3 with position validation
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                // Check if saved position is within valid bounds
                if (
                    playerData.Position[0] <= 577f
                    && playerData.Position[0] >= 400f
                    && playerData.Position[2] <= 157f
                    && playerData.Position[2] >= 50f
                )
                {
                    // Position is valid, use spawn point instead
                    gameObject.transform.position = GameObject
                        .FindWithTag(spawnPointTag)
                        .transform.position;
                    gameObject.transform.rotation = GameObject
                        .FindWithTag(spawnPointTag)
                        .transform.rotation;

                    // Re-enable character controller and return
                    gameObject.GetComponent<CharacterController>().enabled = true;
                    return;
                }
            }

            // Use saved position and rotation data
            gameObject.transform.position = new Vector3(
                playerData.Position[0],
                playerData.Position[1],
                playerData.Position[2]
            );
            gameObject.transform.rotation = new Quaternion(
                playerData.Rotation[0],
                playerData.Rotation[1],
                playerData.Rotation[2],
                playerData.Rotation[3]
            );

            // Re-enable character controller
            gameObject.GetComponent<CharacterController>().enabled = true;
        }

        // Load cutscene completion states from saved data
        this.isCutScenePart1Completed =
            playerData != null ? playerData.IsCutScenePart1Completed : false;
        this.isCutScenePart2Completed =
            playerData != null ? playerData.IsCutScenePart2Completed : false;

        // Set cutscene state based on scene and completion status
        if (SceneManager.GetActiveScene().buildIndex == 1 && !isCutScenePart1Completed)
        {
            playerScript.setIsInCutscene(true);
        }

        if (SceneManager.GetActiveScene().buildIndex == 2 && !isCutScenePart2Completed)
        {
            playerScript.setIsInCutscene(true);
        }
    }
    #endregion

    #region Cutscene State Management
    /// <summary>
    /// Sets the completion state of the first part cutscene.
    /// </summary>
    /// <param name="isCutScenePart1Completed">True if cutscene is completed, false otherwise.</param>
    // COMPLEXITY ANALYSIS: setIsCutScenePart1Completed() - O(1)
    public void setIsCutScenePart1Completed(bool isCutScenePart1Completed)
    {
        this.isCutScenePart1Completed = isCutScenePart1Completed;
    }

    /// <summary>
    /// Sets the completion state of the second part cutscene.
    /// </summary>
    /// <param name="isCutScenePart2Completed">True if cutscene is completed, false otherwise.</param>
    // COMPLEXITY ANALYSIS: setIsCutScenePart2Completed() - O(1)
    public void setIsCutScenePart2Completed(bool isCutScenePart2Completed)
    {
        this.isCutScenePart2Completed = isCutScenePart2Completed;
    }

    /// <summary>
    /// Gets the completion state of the first part cutscene.
    /// </summary>
    /// <returns>True if cutscene is completed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getIsCutScenePart1Completed() - O(1)
    public bool getIsCutScenePart1Completed()
    {
        return isCutScenePart1Completed;
    }

    /// <summary>
    /// Gets the completion state of the second part cutscene.
    /// </summary>
    /// <returns>True if cutscene is completed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getIsCutScenePart2Completed() - O(1)
    public bool getIsCutScenePart2Completed()
    {
        return isCutScenePart2Completed;
    }
    #endregion
}
