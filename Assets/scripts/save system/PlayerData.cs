using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Serializable data structure for storing player state and progress information.
/// Captures player position, rotation, current scene, and cutscene completion states.
/// Used by the save system to persist player data across game sessions.
/// </summary>
[System.Serializable]
public class PlayerData
{
    #region Transform Data
    /// <summary>
    /// List storing the player's X, Y, Z position coordinates.
    /// Serialized for save system persistence and scene restoration.
    /// </summary>
    [SerializeField]
    private List<float> position;

    /// <summary>
    /// List storing the player's X, Y, Z, W rotation quaternion values.
    /// Serialized for save system persistence and orientation restoration.
    /// </summary>
    [SerializeField]
    private List<float> rotation;
    #endregion

    #region Scene and Progress Data
    /// <summary>
    /// Index of the scene where the player was last located.
    /// Used to determine which scene to load when restoring player data.
    /// </summary>
    [SerializeField]
    private int sceneIndex;

    /// <summary>
    /// Tracks whether the first part cutscene has been completed.
    /// Used to determine if cutscene should play on scene load.
    /// </summary>
    [SerializeField]
    private bool isCutScenePart1Completed;

    /// <summary>
    /// Tracks whether the second part cutscene has been completed.
    /// Used to determine if cutscene should play on scene load.
    /// </summary>
    [SerializeField]
    private bool isCutScenePart2Completed;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new PlayerData instance by extracting data from a StartPlayer component.
    /// Captures current transform, scene, and cutscene completion states.
    /// </summary>
    /// <param name="startPlayer">The StartPlayer component to extract data from.</param>
    public PlayerData(StartPlayer startPlayer)
    {
        // Initialize position and rotation lists
        this.position = new List<float>();
        this.rotation = new List<float>();

        // Extract X, Y, Z position coordinates
        this.position.Add(startPlayer.gameObject.transform.position.x);
        this.position.Add(startPlayer.gameObject.transform.position.y);
        this.position.Add(startPlayer.gameObject.transform.position.z);

        // Extract X, Y, Z, W quaternion rotation values
        this.rotation.Add(startPlayer.gameObject.transform.rotation.x);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.y);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.z);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.w);

        // Store current scene index for scene restoration
        this.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Capture cutscene completion states from StartPlayer
        this.isCutScenePart1Completed = startPlayer.getIsCutScenePart1Completed();
        this.isCutScenePart2Completed = startPlayer.getIsCutScenePart2Completed();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the player's position as a list of X, Y, Z coordinates.
    /// </summary>
    public List<float> Position => position;

    /// <summary>
    /// Gets the player's rotation as a list of X, Y, Z, W quaternion values.
    /// </summary>
    public List<float> Rotation => rotation;

    /// <summary>
    /// Gets the scene index where the player was last located.
    /// </summary>
    public int SceneIndex => sceneIndex;

    /// <summary>
    /// Gets whether the first part cutscene has been completed.
    /// </summary>
    public bool IsCutScenePart1Completed => isCutScenePart1Completed;

    /// <summary>
    /// Gets whether the second part cutscene has been completed.
    /// </summary>
    public bool IsCutScenePart2Completed => isCutScenePart2Completed;
    #endregion
}
