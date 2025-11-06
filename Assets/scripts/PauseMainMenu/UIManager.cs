using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI elements including menus, screens, and quest displays.
/// Handles menu toggling, screen transitions, and quest UI updates.
/// Coordinates between different UI systems and manages overall UI state.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Menu References")]
    /// <summary>
    /// GameObject containing the full map menu interface.
    /// Displays the complete world map when activated.
    /// </summary>
    [SerializeField]
    private GameObject fullMapMenu;

    /// <summary>
    /// GameObject containing the skill tree menu interface.
    /// Displays player skills and progression when activated.
    /// </summary>
    [SerializeField]
    private GameObject skillTreeMenu;

    /// <summary>
    /// GameObject containing the inventory menu interface.
    /// Displays player items and equipment when activated.
    /// </summary>
    [SerializeField]
    private GameObject inventoryMenu;

    /// <summary>
    /// GameObject containing the black screen overlay.
    /// Used for fade transitions between scenes and menus.
    /// </summary>
    [SerializeField]
    private GameObject blackScreen;

    /// <summary>
    /// GameObject containing the pause menu interface.
    /// Displays game options and pause functionality when activated.
    /// </summary>
    [SerializeField]
    private GameObject pauseMenu;

    [Header("Quest UI References")]
    /// <summary>
    /// GameObject containing the side quest panel.
    /// Displays active side quests and their progress.
    /// </summary>
    [SerializeField]
    private GameObject SideQuestPanel;

    /// <summary>
    /// GameObject containing the story quest panel.
    /// Displays the current main story quest information.
    /// </summary>
    [SerializeField]
    private GameObject storyQuestPanel;

    /// <summary>
    /// GameObject containing the main player UI elements.
    /// Includes health bars, minimap, and other HUD elements.
    /// </summary>
    [SerializeField]
    private GameObject playerUI;

    /// <summary>
    /// GameObject containing the boss health bar.
    /// Displays boss health during boss encounters.
    /// </summary>
    [SerializeField]
    private GameObject bossHealthBar;
    #endregion

    #region Private Fields
    [Header("Component References")]
    /// <summary>
    /// Reference to the main camera in the scene.
    /// Used for camera control during menu states.
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// Reference to the player controller script.
    /// Used to control player movement and camera rotation.
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// Reference to the main player script.
    /// Used to check player state and cutscene status.
    /// </summary>
    private playerScript playerScript;

    /// <summary>
    /// Reference to the input listener script.
    /// Used to detect menu input and key presses.
    /// </summary>
    private InputListener inputListener;

    [Header("System Tags")]
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string GameManagerTag = "GameManager";

    /// <summary>
    /// Tag used to find the Player GameObject in the scene.
    /// </summary>
    private string PlayerTag = "Player";

    [Header("Menu State Management")]
    /// <summary>
    /// Cooldown flag for pause menu opening to prevent rapid toggling.
    /// </summary>
    private bool cooldownPauseOpen = false;

    /// <summary>
    /// Cooldown flag for inventory menu opening to prevent rapid toggling.
    /// </summary>
    private bool cooldownInventoryOpen = false;

    /// <summary>
    /// Cooldown flag for skill tree menu opening to prevent rapid toggling.
    /// </summary>
    private bool cooldownSkillTreeOpen = false;

    /// <summary>
    /// Cooldown flag for full map menu opening to prevent rapid toggling.
    /// </summary>
    private bool cooldownFullMapOpen = false;

    /// <summary>
    /// Flag indicating whether any menu is currently open.
    /// Used to prevent multiple menus from opening simultaneously.
    /// </summary>
    private bool menuIsOpen = false;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the UI manager and sets up initial UI state.
    /// Sets up component references and configures initial UI visibility.
    /// </summary>
    private void Awake()
    {
        // Find and store the input listener component
        inputListener = GameObject
            .FindGameObjectWithTag(GameManagerTag)
            .GetComponentInChildren<InputListener>();

        // Initialize black screen to be invisible
        blackScreen.SetActive(false);
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);

        // Initialize all menus to be hidden
        skillTreeMenu.GetComponent<Canvas>().enabled = false;
        inventoryMenu.SetActive(false);
        fullMapMenu.SetActive(false);
        bossHealthBar.SetActive(false);

        // Find and store component references
        mainCamera = GameObject.FindGameObjectWithTag(PlayerTag).GetComponentInChildren<Camera>();
        playerScript = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<playerScript>();
        playerController = GameObject
            .FindGameObjectWithTag(PlayerTag)
            .GetComponent<PlayerController>();

        // Ensure player UI is visible by default
        playerUI?.SetActive(true);
    }

    /// <summary>
    /// Handles input for menu toggling each frame.
    /// Checks for menu input and triggers appropriate menu actions.
    /// </summary>
    private void Update()
    {
        // Check for inventory menu input
        if (inputListener.isPressingInventory() && !menuIsOpen)
        {
            toggleInventory();
        }

        // Check for skill tree menu input
        if (inputListener.isPressingSkillTree() && !menuIsOpen)
        {
            toggleSkillTreeMenu();
        }

        // Check for full map menu input
        if (inputListener.isPressingFullMap() && !menuIsOpen)
        {
            toggleFullMapMenu();
        }

        // Check for pause menu input (always allowed)
        if (inputListener.isPressingPause())
        {
            togglePauseMenu();
        }
    }
    #endregion

    #region Screen Transition Methods
    /// <summary>
    /// Fades in the black screen overlay over a specified duration.
    /// Creates a smooth transition effect for scene changes.
    /// </summary>
    /// <param name="fadeInAmount">Duration of the fade in effect in seconds.</param>
    /// <returns>Coroutine for managing the fade animation.</returns>
    public IEnumerator FadeInBlackScreen(float fadeInAmount)
    {
        float alpha = 0;
        // Set initial transparent state
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
        blackScreen.SetActive(true);

        // Gradually increase alpha to create fade in effect
        while (alpha < fadeInAmount)
        {
            alpha += Time.deltaTime / fadeInAmount;
            blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    /// <summary>
    /// Fades out the black screen overlay over a specified duration.
    /// Creates a smooth transition effect for revealing content.
    /// </summary>
    /// <param name="fadeOutAmount">Duration of the fade out effect in seconds.</param>
    /// <returns>Coroutine for managing the fade animation.</returns>
    public IEnumerator FadeOutBlackScreen(float fadeOutAmount)
    {
        float alpha = 1;
        // Gradually decrease alpha to create fade out effect
        while (alpha > fadeOutAmount)
        {
            alpha -= Time.deltaTime / fadeOutAmount;
            blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        // Hide the black screen when fade is complete
        blackScreen.SetActive(false);
        yield return null;
    }
    #endregion

    #region Menu Toggle Methods
    /// <summary>
    /// Toggles the inventory menu on/off with proper state management.
    /// Handles cursor state, camera rotation, and character preview.
    /// </summary>
    public void toggleInventory()
    {
        if (!cooldownInventoryOpen)
        {
            // Find character preview controller for inventory functionality
            CharacterPrevController characterPrevController = GameObject
                .FindGameObjectWithTag(PlayerTag)
                .GetComponentInChildren<CharacterPrevController>();

            // Close other menus first
            fullMapMenu.SetActive(false);
            skillTreeMenu.GetComponent<Canvas>().enabled = false;

            if (inventoryMenu.activeSelf)
            {
                // Closing inventory - restore game state
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                characterPrevController.hideCharacterPreview();
                playerController.startCameraRotation();
            }
            else
            {
                // Opening inventory - set menu state
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterPrevController.showCharacterPreview();
                playerController.stopCameraRotation();
            }

            // Toggle menu visibility and start cooldown
            StartCoroutine(activateCooldownInventoryOpen(0.2f));
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        }
    }

    /// <summary>
    /// Toggles the skill tree menu on/off with proper state management.
    /// Handles cursor state, camera rotation, and menu visibility.
    /// </summary>
    public void toggleSkillTreeMenu()
    {
        if (!cooldownSkillTreeOpen)
        {
            // Close other menus first
            fullMapMenu.SetActive(false);
            inventoryMenu.SetActive(false);

            if (skillTreeMenu.GetComponent<Canvas>().enabled)
            {
                // Closing skill tree - restore game state
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.startCameraRotation();
            }
            else
            {
                // Opening skill tree - set menu state
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.stopCameraRotation();
            }

            // Toggle menu visibility and start cooldown
            StartCoroutine(activateCooldownSkillTreeOpen(0.2f));
            skillTreeMenu.GetComponent<Canvas>().enabled = !skillTreeMenu
                .GetComponent<Canvas>()
                .enabled;
        }
    }

    /// <summary>
    /// Toggles the full map menu on/off with proper state management.
    /// Handles cursor state, camera rotation, and menu visibility.
    /// </summary>
    public void toggleFullMapMenu()
    {
        if (!cooldownFullMapOpen)
        {
            // Close other menus first
            inventoryMenu.SetActive(false);
            skillTreeMenu.GetComponent<Canvas>().enabled = false;

            if (fullMapMenu.activeSelf)
            {
                // Closing map - restore game state
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.startCameraRotation();
            }
            else
            {
                // Opening map - set menu state
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.stopCameraRotation();
            }

            // Toggle menu visibility and start cooldown
            StartCoroutine(activateCooldownFullMapOpen(0.2f));
            fullMapMenu.SetActive(!fullMapMenu.activeSelf);
        }
    }

    /// <summary>
    /// Toggles the pause menu on/off with proper state management.
    /// Handles time scale, cursor state, and camera control.
    /// </summary>
    public void togglePauseMenu()
    {
        if (!cooldownPauseOpen)
        {
            if (pauseMenu.activeSelf)
            {
                // Resuming game - restore normal state
                menuIsOpen = false;
                mainCamera.enabled = true;
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f; // Resume normal time
                showPlayerUI();
                playerController.startCameraRotation();
            }
            else
            {
                // Pausing game - set pause state
                menuIsOpen = true;
                mainCamera.enabled = false;
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f; // Pause time
                playerUI.SetActive(false);
                playerController.stopCameraRotation();
            }
            // Note: Pause cooldown is commented out for immediate response
            // StartCoroutine(activateCooldownPauseOpen(1.5f));
        }
    }

    /// <summary>
    /// Toggles the loading screen on for scene transitions.
    /// Sets up UI state for loading between scenes.
    /// </summary>
    public void toggleLoadingScreen()
    {
        // Disable camera and show pause menu for loading state
        mainCamera.enabled = false;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerUI.SetActive(false);
        playerController.stopCameraRotation();
    }
    #endregion

    #region Cooldown Management Methods
    /// <summary>
    /// Activates cooldown for inventory menu opening to prevent rapid toggling.
    /// </summary>
    /// <param name="cooldownTime">Duration of the cooldown in seconds.</param>
    /// <returns>Coroutine for managing the cooldown timer.</returns>
    public IEnumerator activateCooldownInventoryOpen(float cooldownTime)
    {
        cooldownInventoryOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownInventoryOpen = false;
    }

    /// <summary>
    /// Activates cooldown for skill tree menu opening to prevent rapid toggling.
    /// </summary>
    /// <param name="cooldownTime">Duration of the cooldown in seconds.</param>
    /// <returns>Coroutine for managing the cooldown timer.</returns>
    public IEnumerator activateCooldownSkillTreeOpen(float cooldownTime)
    {
        cooldownSkillTreeOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownSkillTreeOpen = false;
    }

    /// <summary>
    /// Activates cooldown for pause menu opening to prevent rapid toggling.
    /// </summary>
    /// <param name="cooldownTime">Duration of the cooldown in seconds.</param>
    /// <returns>Coroutine for managing the cooldown timer.</returns>
    public IEnumerator activateCooldownPauseOpen(float cooldownTime)
    {
        cooldownPauseOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownPauseOpen = false;
    }

    /// <summary>
    /// Activates cooldown for full map menu opening to prevent rapid toggling.
    /// </summary>
    /// <param name="cooldownTime">Duration of the cooldown in seconds.</param>
    /// <returns>Coroutine for managing the cooldown timer.</returns>
    public IEnumerator activateCooldownFullMapOpen(float cooldownTime)
    {
        cooldownFullMapOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownFullMapOpen = false;
    }
    #endregion

    #region UI State Management Methods
    /// <summary>
    /// Hides all menus and UI elements.
    /// Used for scene transitions or resetting UI state.
    /// </summary>
    public void hideAllMenus()
    {
        // Hide all menu elements
        fullMapMenu.SetActive(false);
        skillTreeMenu.GetComponent<Canvas>().enabled = false;
        inventoryMenu.SetActive(false);
        blackScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    /// <summary>
    /// Shows the player UI if not in a cutscene.
    /// Checks cutscene state before displaying UI elements.
    /// </summary>
    public void showPlayerUI()
    {
        if (!playerScript.getIsInCutscene())
        {
            playerUI.SetActive(true);
        }
    }

    /// <summary>
    /// Starts the fade in black screen effect.
    /// Wrapper method for starting fade coroutines from external calls.
    /// </summary>
    /// <param name="fadeInAmount">Duration of the fade in effect in seconds.</param>
    public void startFadeInBlackScreen(float fadeInAmount)
    {
        StartCoroutine(FadeInBlackScreen(fadeInAmount));
    }

    /// <summary>
    /// Starts the fade out black screen effect.
    /// Wrapper method for starting fade coroutines from external calls.
    /// </summary>
    /// <param name="fadeOutAmount">Duration of the fade out effect in seconds.</param>
    public void startFadeOutBlackScreen(float fadeOutAmount)
    {
        StartCoroutine(FadeOutBlackScreen(fadeOutAmount));
    }

    /// <summary>
    /// Shows the boss health bar for boss encounters.
    /// Activates the boss health display UI element.
    /// </summary>
    public void showBossHealthBar()
    {
        bossHealthBar.SetActive(true);
    }

    /// <summary>
    /// Hides the boss health bar when boss encounter ends.
    /// Deactivates the boss health display UI element.
    /// </summary>
    public void hideBossHealthBar()
    {
        bossHealthBar.SetActive(false);
    }

    /// <summary>
    /// Gets whether any menu is currently open.
    /// Used to prevent multiple menus from opening simultaneously.
    /// </summary>
    /// <returns>True if a menu is open, false otherwise.</returns>
    public bool isMenuOpen()
    {
        return menuIsOpen;
    }
    #endregion

    #region Quest UI Management Methods
    /// <summary>
    /// Adds a quest to the side quest panel.
    /// Delegates to the QuestListDisplay component for actual addition.
    /// </summary>
    /// <param name="questId">The ID of the quest to add.</param>
    /// <param name="questToAdd">The quest object to add to the display.</param>
    public void addQuest(int questId, Quest questToAdd)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().addQuest(questId, questToAdd);
    }

    /// <summary>
    /// Removes a quest from the side quest panel.
    /// Delegates to the QuestListDisplay component for actual removal.
    /// </summary>
    /// <param name="questId">The ID of the quest to remove.</param>
    public void removeQuest(int questId)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().removeQuest(questId);
    }

    /// <summary>
    /// Updates the progress of a quest in the side quest panel.
    /// Delegates to the QuestListDisplay component for actual update.
    /// </summary>
    /// <param name="questId">The ID of the quest to update.</param>
    /// <param name="progress">The new progress string to display.</param>
    public void updateQuestProgress(int questId, string progress)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().updateQuestProgress(questId, progress);
    }

    /// <summary>
    /// Updates the story quest panel with new quest information.
    /// Sets the quest name and description in the story quest display.
    /// </summary>
    /// <param name="questToAdd">The quest to display in the story quest panel.</param>
    public void updateStoryQuestPanel(Quest questToAdd)
    {
        if (storyQuestPanel.GetComponent<QuestListing>() != null)
        {
            // Update quest name and description
            storyQuestPanel.GetComponent<QuestListing>().SetName(questToAdd.GetQuestName());
            storyQuestPanel
                .GetComponent<QuestListing>()
                .SetDescription(questToAdd.GetDescription());

            // Force layout rebuild to ensure proper positioning
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                storyQuestPanel.GetComponent<RectTransform>()
            );
        }
    }

    /// <summary>
    /// Gets a quest GameObject by its ID.
    /// Searches the side quest panel for a specific quest.
    /// </summary>
    /// <param name="questId">The ID of the quest to find.</param>
    /// <returns>The GameObject representing the quest, or null if not found.</returns>
    public GameObject getQuestById(int questId)
    {
        return SideQuestPanel.transform.Find(questId.ToString()).gameObject;
    }
    #endregion
}
