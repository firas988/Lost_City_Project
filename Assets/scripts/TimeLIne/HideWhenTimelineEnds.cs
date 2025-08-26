using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Manages object visibility and timeline control based on UI menu state and timeline completion.
/// Automatically hides/shows objects when menus are opened/closed and deactivates itself when timeline ends.
/// Provides seamless integration between UI system and cutscene timeline management.
/// </summary>
public class HideWhenTimelineEnds : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// PlayableDirector component that controls the cutscene timeline.
    /// Monitored for timeline completion events and can be paused/resumed based on menu state.
    /// </summary>
    private PlayableDirector director;

    /// <summary>
    /// Reference to the UIManager for detecting menu open/close states.
    /// Used to control timeline playback and object visibility based on menu interactions.
    /// </summary>
    private UIManager uiManager;
    #endregion

    #region Serialized Fields
    /// <summary>
    /// List of GameObjects to hide/show based on menu state and timeline progress.
    /// These objects are controlled to provide appropriate visual feedback during cutscenes.
    /// </summary>
    [SerializeField]
    private List<GameObject> objectsToHide;
    #endregion

    #region Configuration
    /// <summary>
    /// Tag identifier for finding the GameManager GameObject in the scene.
    /// Used to locate the UIManager component within the GameManager hierarchy.
    /// </summary>
    private string GameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the timeline handler by setting up timeline monitoring and UI manager reference.
    /// Configures event subscriptions for timeline completion and UI state changes.
    /// </summary>
    void Start()
    {
        // Get the PlayableDirector component if not already assigned
        if (director == null)
            director = GetComponentInChildren<PlayableDirector>();

        // Subscribe to timeline completion events
        director.stopped += OnTimelineStopped;

        // Find and store reference to UIManager for menu state monitoring
        uiManager = GameObject
            .FindGameObjectWithTag(GameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }

    /// <summary>
    /// Continuously monitors UI menu state and controls timeline playback accordingly.
    /// Pauses timeline and hides objects when menu is open, resumes and shows objects when menu is closed.
    /// </summary>
    private void Update()
    {
        // Ensure UIManager reference is valid, re-acquire if needed
        if (uiManager == null)
        {
            uiManager = GameObject
                .FindGameObjectWithTag(GameManagerTag)
                .transform.parent.GetComponentInChildren<UIManager>();
            return;
        }

        // Control timeline and object visibility based on menu state
        if (uiManager.isMenuOpen())
        {
            // Pause timeline and hide objects when menu is open
            director.Pause();
            objectsToHide.ForEach(obj => obj.SetActive(false));
        }
        else
        {
            // Resume timeline and show objects when menu is closed
            director.Resume();
            objectsToHide.ForEach(obj => obj.SetActive(true));
        }
    }
    #endregion

    #region Timeline Event Handling
    /// <summary>
    /// Event handler called when the cutscene timeline completes.
    /// Deactivates this GameObject to clean up after the cutscene finishes.
    /// </summary>
    /// <param name="obj">The PlayableDirector that triggered the event.</param>
    void OnTimelineStopped(PlayableDirector obj)
    {
        gameObject.SetActive(false);
    }
    #endregion
}
