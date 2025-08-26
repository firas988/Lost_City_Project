using UnityEngine;

/// <summary>
/// Handles cutscene-specific functionality for the Rebert Room sequence.
/// Manages scene transitions and cutscene state management during the Rebert Room cutscene.
/// Coordinates with SceneHandler for scene loading and playerScript for cutscene state.
/// </summary>
public class RebertRoomCutSceneHandler : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to the SceneHandler component for managing scene transitions.
    /// Used to load the next scene after the cutscene completes.
    /// </summary>
    private SceneHandler sceneHandler;
    #endregion

    #region Configuration
    /// <summary>
    /// Tag identifier for finding the GameManager GameObject in the scene.
    /// Used to locate the SceneHandler component within the GameManager hierarchy.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the cutscene handler by finding and storing references to required components.
    /// Locates the SceneHandler through the GameManager tag for scene management.
    /// </summary>
    void Start()
    {
        // Find the GameManager and get its SceneHandler component
        sceneHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<SceneHandler>();
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Loads scene index 2 after the cutscene completes.
    /// Triggers the scene transition through the SceneHandler component.
    /// </summary>
    public void loadScene()
    {
        sceneHandler.LoadScene(2);
    }

    /// <summary>
    /// Sets the player's cutscene state to control input and behavior.
    /// Updates the playerScript to reflect whether the player is currently in a cutscene.
    /// </summary>
    /// <param name="inCutScene">True if the player is in a cutscene, false otherwise.</param>
    public void SetIsInCutscene(bool inCutScene)
    {
        playerScript.setIsInCutscene(inCutScene);
    }
    #endregion
}
