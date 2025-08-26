using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages cutscene completion and state tracking across different game parts.
/// Handles timeline events, cutscene completion flags, and object visibility based on cutscene progress.
/// Coordinates with StartPlayer to track cutscene completion states for parts 1 and 2.
/// </summary>
public class CutSceneHandlerDone : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// PlayableDirector component that controls the cutscene timeline.
    /// Monitored for timeline completion events to update cutscene states.
    /// </summary>
    private PlayableDirector director;

    /// <summary>
    /// Reference to the StartPlayer component for managing cutscene completion states.
    /// Used to check and update cutscene completion flags for different game parts.
    /// </summary>
    private StartPlayer startPlayer;
    #endregion

    #region Serialized Fields
    /// <summary>
    /// List of GameObjects to hide when cutscene part 2 is already completed.
    /// These objects are deactivated to prevent duplicate cutscene elements.
    /// </summary>
    [SerializeField]
    private List<GameObject> objectsToHide;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the cutscene handler by setting up timeline monitoring and cutscene state management.
    /// Configures object visibility based on current cutscene completion status.
    /// </summary>
    void Start()
    {
        // Get the PlayableDirector component if not already assigned
        if (director == null)
            director = GetComponentInChildren<PlayableDirector>();

        // Subscribe to timeline completion events
        director.stopped += OnTimelineStopped;

        // Get reference to StartPlayer for cutscene state management
        startPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Handle cutscene visibility based on scene and completion status
        if (sceneIndex == 1)
        {
            // Hide cutscene if part 1 is already completed
            gameObject.SetActive(!startPlayer.getIsCutScenePart1Completed());
        }
        else if (sceneIndex == 2)
        {
            if (!startPlayer.getIsCutScenePart2Completed())
            {
                // Show cutscene if part 2 is not completed
                gameObject.SetActive(true);
            }
            else
            {
                // Hide all cutscene objects and the handler if part 2 is completed
                objectsToHide.ForEach(obj => obj.SetActive(false));
                gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Timeline Event Handling
    /// <summary>
    /// Event handler called when the cutscene timeline completes.
    /// Updates the appropriate cutscene completion flag based on the current scene.
    /// </summary>
    /// <param name="obj">The PlayableDirector that triggered the event.</param>
    void OnTimelineStopped(PlayableDirector obj)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Mark cutscene as completed based on current scene
        if (sceneIndex == 1)
        {
            startPlayer.setIsCutScenePart1Completed(true);
        }
        else if (sceneIndex == 2)
        {
            startPlayer.setIsCutScenePart2Completed(true);
        }
    }
    #endregion
}
