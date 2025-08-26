using UnityEngine;

/// <summary>
/// Serializable data structure for storing player level and experience progression.
/// Captures current level, experience points, and experience required for next level.
/// Used by the save system to persist player progression across game sessions.
/// </summary>
[System.Serializable]
public class LevelData
{
    #region Level and Experience Data
    /// <summary>
    /// Current player level representing overall progression.
    /// Determines various gameplay mechanics and unlockables.
    /// </summary>
    [SerializeField]
    private int level;

    /// <summary>
    /// Current experience points accumulated by the player.
    /// Represents progress toward the next level.
    /// </summary>
    [SerializeField]
    private float currentXP;

    /// <summary>
    /// Experience points required to reach the next level.
    /// Used to calculate progress percentage and level-up timing.
    /// </summary>
    [SerializeField]
    private float xPtoNextLevel;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new LevelData instance by extracting data from a LevelManager.
    /// Captures current level, experience, and next level requirements.
    /// </summary>
    /// <param name="levelManager">The LevelManager component to extract data from.</param>
    public LevelData(LevelManager levelManager)
    {
        // Extract current level and experience data
        this.level = levelManager.getLevel();
        this.currentXP = levelManager.getCurrentXP();
        this.xPtoNextLevel = levelManager.getXPtoNextLevel();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the current player level.
    /// </summary>
    public int Level => level;

    /// <summary>
    /// Gets the current experience points accumulated.
    /// </summary>
    public float CurrentXP => currentXP;

    /// <summary>
    /// Gets the experience points required for the next level.
    /// </summary>
    public float XPtoNextLevel => xPtoNextLevel;
    #endregion
}
