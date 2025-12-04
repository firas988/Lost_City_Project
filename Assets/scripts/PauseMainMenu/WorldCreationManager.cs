using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages world creation, loading, and replacement functionality.
/// Handles world list display, directory management, and scene transitions.
/// Provides interface for creating new worlds and loading existing ones.
/// </summary>
public class WorldCreationManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Input Fields")]
    /// <summary>
    /// Input field for entering new world names.
    /// Player types the desired world name here.
    /// </summary>
    [SerializeField]
    private TMP_InputField worldNameInputField;

    [Header("UI Display Elements")]
    /// <summary>
    /// GameObject containing the list of existing worlds.
    /// Displays world buttons with names and modification dates.
    /// </summary>
    [SerializeField]
    private GameObject worldList;

    /// <summary>
    /// GameObject containing the confirmation panel for world replacement.
    /// Appears when trying to replace an existing world.
    /// </summary>
    [SerializeField]
    private GameObject confirmPanel;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the scene handler for managing scene transitions.
    /// Used to load appropriate scenes after world creation/loading.
    /// </summary>
    private SceneHandler sceneHandler;

    [Header("World Management State")]
    /// <summary>
    /// Reference to the world GameObject that will be replaced.
    /// Set when player attempts to create a world with an existing name.
    /// </summary>
    private GameObject worldToReplace;

    /// <summary>
    /// Reference to the world GameObject that will be loaded.
    /// Set when player clicks on an existing world to load.
    /// </summary>
    private GameObject worldToLoad;

    /// <summary>
    /// Flag indicating whether the player is attempting to replace a world.
    /// Controls the display of the confirmation panel.
    /// </summary>
    private bool isTryingToReplace;

    /// <summary>
    /// Flag indicating whether the world list is currently being loaded.
    /// Prevents multiple simultaneous loading operations.
    /// </summary>
    private bool isLoadingWorldList;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Updates the confirm panel visibility when trying to replace a world.
    /// Ensures the confirmation dialog is shown when needed.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        if (isTryingToReplace && !confirmPanel.activeSelf)
        {
            confirmPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Initializes the world creation manager and loads existing world data.
    /// Sets up the world list display and creates necessary directories.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(n) where n = number of world directories
    void Awake()
    {
        // Find and store the scene handler reference
        sceneHandler = GameObject.Find("SceneManager").GetComponent<SceneHandler>();

        // Initialize state flags
        isTryingToReplace = false;
        isLoadingWorldList = false;

        // Get all world buttons from the world list
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        confirmPanel.SetActive(false);

        // Ensure the game data directory exists
        Directory.CreateDirectory(Application.persistentDataPath + "/gameData/");

        // Get all existing world directories
        string[] directoryNames = new DirectoryInfo(Application.persistentDataPath + "/gameData/")
            .GetDirectories()
            .OrderByDescending(d => d.LastWriteTime)
            .Select(d => d.FullName)
            .ToArray();

        if (directoryNames.Length > 0)
        {
            // Populate world list with existing world data
            for (int i = 0; i < Mathf.Min(directoryNames.Length, worldNames.Length); i++)
            {
                // Find text components for world name and modification date
                TMP_Text worldNameText = worldNames[i]
                    .transform.Find("WorldName")
                    .GetComponent<TMP_Text>();
                TMP_Text worldModifiedText = worldNames[i]
                    .transform.Find("LastModified")
                    .GetComponent<TMP_Text>();

                // Set world name from directory name
                worldNameText.text = Path.GetFileName(directoryNames[i]);

                // Set modification date from directory info
                worldModifiedText.text =
                    "Last Modified: " + GetFolderLastModified(directoryNames[i]);
            }
        }
        else
        {
            // No worlds found - list will remain empty
        }
    }
    #endregion

    #region World Management Methods
    /// <summary>
    /// Gets the last modified date of a folder and its contents.
    /// Checks both the folder itself and all files within it for the latest modification.
    /// </summary>
    /// <param name="path">The path to the folder to check.</param>
    /// <returns>A formatted string representing the last modified date.</returns>
    // COMPLEXITY ANALYSIS: GetFolderLastModified() - O(f) where f = number of files in directory
    public string GetFolderLastModified(string path)
    {
        if (!Directory.Exists(path))
            return "Folder does not exist";

        // Get directory information
        var dirInfo = new DirectoryInfo(path);
        var lastWriteTime = dirInfo.LastWriteTime;

        // Check all files within the directory and subdirectories
        var fileTimes = dirInfo
            .GetFiles("*", SearchOption.AllDirectories)
            .Select(f => f.LastWriteTime);

        if (fileTimes.Any())
        {
            // Find the most recent file modification time
            var latestFileTime = fileTimes.Max();
            if (latestFileTime > lastWriteTime)
                lastWriteTime = latestFileTime;
        }

        // Return formatted date string
        return lastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    /// <summary>
    /// Navigates to the world list panel if a world name is provided.
    /// Validates input before allowing navigation to world selection.
    /// </summary>
    // COMPLEXITY ANALYSIS: goToWorldList() - O(1)
    public void goToWorldList()
    {
        if (!string.IsNullOrEmpty(worldNameInputField.text))
        {
            // Navigate to world list panel for world selection
            GetComponent<UIMenuManager>().worldListPanelPosition();
        }
        else
        {
            // World name is empty - no action taken
        }
    }

    /// <summary>
    /// Replaces an existing world with a new one.
    /// Deletes the old world directory and creates a new one with the same name.
    /// </summary>
    // COMPLEXITY ANALYSIS: replaceWorld() - O(1)
    public void replaceWorld()
    {
        // Build path to the world being replaced
        string pathToWorld =
            Application.persistentDataPath
            + "/gameData/"
            + worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        // Delete the old world directory completely
        DirectoryInfo worldToReplaceInfo = new DirectoryInfo(pathToWorld);
        worldToReplaceInfo.Delete(true);

        // Update the world name display to the new name
        worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text =
            worldNameInputField.text;

        // Create new world directory with the new name
        string worldPath = Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
        Directory.CreateDirectory(worldPath);

        // Load the newly created world
        loadWorld(worldPath);
    }

    /// <summary>
    /// Creates a new world or initiates world replacement if necessary.
    /// Handles both new world creation and existing world replacement scenarios.
    /// </summary>
    // COMPLEXITY ANALYSIS: createWorld() - O(1)
    public void createWorld()
    {
        // Store reference to the clicked world button
        worldToReplace = EventSystem.current.currentSelectedGameObject;

        // Get the current world name from the button
        string worldName = worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        if (
            Directory.Exists(
                Application.persistentDataPath + "/gameData/" + worldNameInputField.text
            )
        )
        {
            // World already exists with this name - no action taken
        }
        else if (Directory.Exists(Application.persistentDataPath + "/gameData/" + worldName))
        {
            // Existing world found - initiate replacement process
            isTryingToReplace = true;
        }
        else
        {
            // No existing world - create new one immediately
            worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text =
                worldNameInputField.text;

            // Create new world directory
            string worldPath =
                Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
            Directory.CreateDirectory(worldPath);

            // Load the newly created world
            loadWorld(worldPath);
        }
    }

    /// <summary>
    /// Loads a world based on the currently selected world button.
    /// Called when clicking on an existing world in the list.
    /// </summary>
    // COMPLEXITY ANALYSIS: loadWorld() - O(1)
    public void loadWorld()
    {
        // Store reference to the clicked world button
        worldToLoad = EventSystem.current.currentSelectedGameObject;

        // Build path to the selected world
        string worldPath =
            Application.persistentDataPath
            + "/gameData/"
            + worldToLoad.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        if (Directory.Exists(worldPath))
        {
            // World exists - load it
            loadWorld(worldPath);
        }
        else
        {
            // World does not exist - no action taken
        }
    }

    /// <summary>
    /// Loads a world from the specified path and transitions to the appropriate scene.
    /// Saves the world path and loads the correct scene based on saved player data.
    /// </summary>
    /// <param name="worldPath">The path to the world to load.</param>
    // COMPLEXITY ANALYSIS: loadWorld() - O(1)
    public void loadWorld(string worldPath)
    {
        // Save the world path for future reference
        PlayerPrefs.SetString("worldPath", worldPath);
        PlayerPrefs.Save();

        // Try to load existing player data
        PlayerData playerData = SaveSystem.LoadPlayer();

        if (playerData == null)
        {
            // No player data found - start from beginning (scene 1)
            sceneHandler.LoadScene(1);
        }
        else
        {
            // Player data found - load the scene they were last in
            sceneHandler.LoadScene(playerData.SceneIndex);
        }
    }
    #endregion

    #region UI Mode Management Methods
    /// <summary>
    /// Switches the world list to load mode, where clicking a world loads it.
    /// Changes button click behavior to load existing worlds.
    /// </summary>
    // COMPLEXITY ANALYSIS: switchToLoadMode() - O(b) where b = number of world buttons
    public void switchToLoadMode()
    {
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        foreach (Button worldName in worldNames)
        {
            // Remove existing click listeners
            worldName.onClick.RemoveAllListeners();

            // Add load world listener
            worldName.onClick.AddListener(() => loadWorld());
        }
    }

    /// <summary>
    /// Switches the world list to create mode, where clicking a world creates a new one.
    /// Changes button click behavior to create new worlds or replace existing ones.
    /// </summary>
    // COMPLEXITY ANALYSIS: switchToCreateMode() - O(b) where b = number of world buttons
    public void switchToCreateMode()
    {
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        foreach (Button worldName in worldNames)
        {
            // Remove existing click listeners
            worldName.onClick.RemoveAllListeners();

            // Add create world listener
            worldName.onClick.AddListener(() => createWorld());
        }
    }

    /// <summary>
    /// Hides the confirmation panel and resets the replacement flag.
    /// Called when player cancels world replacement.
    /// </summary>
    // COMPLEXITY ANALYSIS: hideConfirmPanel() - O(1)
    public void hideConfirmPanel()
    {
        confirmPanel.SetActive(false);
        isTryingToReplace = false;
    }
    #endregion
}
