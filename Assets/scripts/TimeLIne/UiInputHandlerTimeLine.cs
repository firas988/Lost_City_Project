using System.Collections;
using UnityEngine;

/// <summary>
/// Manages UI input and visibility control during timeline/cutscene sequences.
/// Provides methods to enable/disable player input and UI elements for seamless cutscene integration.
/// Coordinates between InputListener and UIManager to control player interaction capabilities.
/// </summary>
public class UiInputHandlerTimeLine : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to the InputListener component for controlling player input capabilities.
    /// Used to enable/disable menu opening functionality during cutscenes.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to the UIManager component for controlling UI element visibility.
    /// Used to show/hide player UI elements and manage menu states during cutscenes.
    /// </summary>
    private UIManager uiManager;
    #endregion

    #region Configuration
    /// <summary>
    /// Tag identifier for finding the GameManager GameObject in the scene.
    /// Used to locate the InputListener and UIManager components within the GameManager hierarchy.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the input handler by finding and storing references to required components.
    /// Locates InputListener and UIManager through the GameManager tag for input and UI management.
    /// </summary>
    void Awake()
    {
        // Find and store reference to InputListener for input control
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();

        // Find and store reference to UIManager for UI control
        uiManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }
    #endregion

    #region Input Control Methods
    /// <summary>
    /// Disables player input by preventing menu opening during cutscenes.
    /// Ensures player cannot interrupt cutscene sequences with menu interactions.
    /// </summary>
    public void disableInput()
    {
        inputListener.setCanOpenMenu(false);
    }

    /// <summary>
    /// Enables player input by allowing menu opening after cutscenes.
    /// Restores normal player interaction capabilities when cutscenes complete.
    /// </summary>
    public void enableInput()
    {
        inputListener.setCanOpenMenu(true);
    }
    #endregion

    #region UI Control Methods
    /// <summary>
    /// Hides all UI menus and elements during cutscenes.
    /// Provides clean visual experience without UI distractions during cinematic sequences.
    /// </summary>
    public void disableUi()
    {
        uiManager.hideAllMenus();
    }

    /// <summary>
    /// Shows the player UI elements after cutscenes complete.
    /// Restores normal UI visibility for gameplay continuation.
    /// </summary>
    public void enableUi()
    {
        uiManager.showPlayerUI();
    }
    #endregion
}
