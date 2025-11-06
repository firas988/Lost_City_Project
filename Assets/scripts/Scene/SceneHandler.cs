using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages scene loading and transitions with loading screen UI and progress bar.
/// Handles both in-game scene changes and menu-to-game transitions with proper save operations.
/// Provides smooth loading experience with visual progress feedback.
/// </summary>
public class SceneHandler : MonoBehaviour
{
    #region UI Components
    /// <summary>
    /// Loading screen GameObject that displays during scene transitions.
    /// Provides visual feedback that the game is loading.
    /// </summary>
    [SerializeField]
    private GameObject loadingScreen;

    /// <summary>
    /// Load/Create scene UI panel for menu transitions.
    /// Manages the interface when switching between menu and game scenes.
    /// </summary>
    [SerializeField]
    private GameObject loadCreateScene;

    /// <summary>
    /// Progress bar UI element that shows loading progress.
    /// Provides visual feedback on scene loading completion.
    /// </summary>
    [SerializeField]
    private Slider loadingBar;
    #endregion

    #region Manager References
    /// <summary>
    /// Reference to the UI manager for controlling game interface elements.
    /// Used to hide menus and toggle loading screen during scene transitions.
    /// </summary>
    [SerializeField]
    private UIManager uiManager;

    /// <summary>
    /// Reference to the UI menu manager for controlling menu panel states.
    /// Manages menu panel visibility and loading screen toggling.
    /// </summary>
    [SerializeField]
    private UIMenuManager uiMenuManager;

    /// <summary>
    /// Reference to the save handler for persisting game data.
    /// Ensures game progress is saved before scene transitions.
    /// </summary>
    [SerializeField]
    private SaveHandler saveHandler;
    #endregion

    #region Configuration
    /// <summary>
    /// Flag indicating whether the scene handler is operating in-game mode.
    /// Determines whether to save game data and manage in-game UI elements.
    /// </summary>
    [SerializeField]
    private bool inGame = true;
    #endregion

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It sets the time scale to 1 if it is 0.
    /// </summary>
    private void Awake()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    #region Public Interface

    /// <summary>
    /// Initiates scene loading for the specified scene index.
    /// Starts the asynchronous loading coroutine with proper UI management.
    /// </summary>
    /// <param name="index">The build index of the scene to load.</param>
    // COMPLEXITY ANALYSIS: LoadScene() - O(1)
    public void LoadScene(int index)
    {
        StartCoroutine(LoadAsynchronously(index));
    }
    #endregion

    #region Scene Loading Coroutines
    /// <summary>
    /// Asynchronously loads a scene with loading screen and progress bar.
    /// Manages UI transitions, save operations, and provides smooth loading experience.
    /// </summary>
    /// <param name="index">The build index of the scene to load.</param>
    /// <returns>Coroutine for asynchronous execution.</returns>
    // COMPLEXITY ANALYSIS: LoadAsynchronously() - O(1)
    IEnumerator LoadAsynchronously(int index)
    {
        // Initialize progress tracking variables
        float displayedProgress = 0f;
        loadingBar.value = 0f;
        float fakeProgressSpeed = 0.5f;

        // Handle in-game scene transitions
        if (inGame)
        {
            // Save game data before transitioning (except when returning to main menu)
            if (index != 0)
            {
                saveHandler.SaveGame();
            }

            // Hide all game UI and show loading screen
            uiManager.hideAllMenus();
            uiManager.toggleLoadingScreen();
            uiMenuManager.DisablePanels();
            uiMenuManager.toggleLoadingScreen();

            // Brief delay for UI transitions
            yield return new WaitForSecondsRealtime(1f);
        }
        else
        {
            // Handle menu-to-game transitions
            loadCreateScene.SetActive(false);
            loadingScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Start asynchronous scene loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        // Monitor loading progress and update UI
        while (!operation.isDone)
        {
            // Smoothly animate progress bar towards 90%
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                0.9f,
                fakeProgressSpeed * Time.unscaledDeltaTime
            );
            loadingBar.value = displayedProgress;

            // When actual loading is nearly complete, animate to 100%
            if (operation.progress >= 0.9f)
            {
                // Complete the progress bar animation
                while (displayedProgress < 1f)
                {
                    displayedProgress = Mathf.MoveTowards(
                        displayedProgress,
                        1f,
                        fakeProgressSpeed * Time.unscaledDeltaTime
                    );
                    loadingBar.value = displayedProgress;
                    yield return null;
                }

                // Brief delay to show completion, then activate scene
                yield return new WaitForSecondsRealtime(1.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    #endregion
}
