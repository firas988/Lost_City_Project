using UnityEngine;

/// <summary>
/// Serializable data structure for storing comprehensive player statistics and progress.
/// Captures movement, gameplay time, combat achievements, and enemy kill counts.
/// Used by the save system to persist player statistics across game sessions.
/// </summary>
[System.Serializable]
public class StatisticsData
{
    #region Core Gameplay Statistics
    /// <summary>
    /// Total distance traveled by the player across all game sessions.
    /// Represents cumulative exploration and movement progress.
    /// </summary>
    [SerializeField]
    private float totalDistance;

    /// <summary>
    /// Total time played across all game sessions.
    /// Tracks actual gameplay time for progress monitoring.
    /// </summary>
    [SerializeField]
    private float totalTimePlayed;

    /// <summary>
    /// Total number of jumps performed by the player.
    /// Represents player mobility and exploration achievements.
    /// </summary>
    [SerializeField]
    private int totalJumps;

    /// <summary>
    /// Total number of player deaths across all game sessions.
    /// Tracks difficulty and learning curve progression.
    /// </summary>
    [SerializeField]
    private int totalDeaths;
    #endregion

    #region Combat Achievement Statistics
    /// <summary>
    /// Total number of enemies killed across all types.
    /// General combat achievement counter.
    /// </summary>
    [SerializeField]
    private int totalEnemiesKilled;

    /// <summary>
    /// Total number of Bear enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalBearsKilled;

    /// <summary>
    /// Total number of Wolf enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalWolfesKilled;

    /// <summary>
    /// Total number of Troll enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalTrollsKilled;

    /// <summary>
    /// Total number of HobGoblin enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalHobGoblinsKilled;

    /// <summary>
    /// Total number of Goblin enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalGoblinsKilled;

    /// <summary>
    /// Total number of MonsterMutant enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalMonsterMutantsKilled;

    /// <summary>
    /// Total number of Executioner enemies killed.
    /// Specific enemy type achievement tracking.
    /// </summary>
    [SerializeField]
    private int totalExecutionersKilled;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new StatisticsData instance by extracting data from a StatisticsHandler.
    /// Captures all current statistics for persistence across game sessions.
    /// </summary>
    /// <param name="statisticsHandler">The StatisticsHandler component to extract data from.</param>
    public StatisticsData(StatisticsHandler statisticsHandler)
    {
        // Extract core gameplay statistics
        totalDistance = statisticsHandler.TotalDistance;
        totalTimePlayed = statisticsHandler.TotalTimePlayed;
        totalJumps = statisticsHandler.TotalJumps;
        totalDeaths = statisticsHandler.TotalDeaths;

        // Extract combat achievement statistics
        totalEnemiesKilled = statisticsHandler.TotalEnemiesKilled;
        totalBearsKilled = statisticsHandler.TotalBearsKilled;
        totalWolfesKilled = statisticsHandler.TotalWolfesKilled;
        totalTrollsKilled = statisticsHandler.TotalTrollsKilled;
        totalHobGoblinsKilled = statisticsHandler.TotalHobGoblinsKilled;
        totalGoblinsKilled = statisticsHandler.TotalGoblinsKilled;
        totalMonsterMutantsKilled = statisticsHandler.TotalMonsterMutantsKilled;
        totalExecutionersKilled = statisticsHandler.TotalExecutionersKilled;
    }
    #endregion

    #region Public Properties - Core Statistics
    /// <summary>
    /// Gets the total distance traveled by the player.
    /// </summary>
    public float TotalDistance => totalDistance;

    /// <summary>
    /// Gets the total time played across all game sessions.
    /// </summary>
    public float TotalTimePlayed => totalTimePlayed;

    /// <summary>
    /// Gets the total number of jumps performed by the player.
    /// </summary>
    public int TotalJumps => totalJumps;

    /// <summary>
    /// Gets the total number of player deaths.
    /// </summary>
    public int TotalDeaths => totalDeaths;
    #endregion

    #region Public Properties - Combat Statistics
    /// <summary>
    /// Gets the total number of enemies killed across all types.
    /// </summary>
    public int TotalEnemiesKilled => totalEnemiesKilled;

    /// <summary>
    /// Gets the total number of Bear enemies killed.
    /// </summary>
    public int TotalBearsKilled => totalBearsKilled;

    /// <summary>
    /// Gets the total number of Wolf enemies killed.
    /// </summary>
    public int TotalWolfesKilled => totalWolfesKilled;

    /// <summary>
    /// Gets the total number of Troll enemies killed.
    /// </summary>
    public int TotalTrollsKilled => totalTrollsKilled;

    /// <summary>
    /// Gets the total number of HobGoblin enemies killed.
    /// </summary>
    public int TotalHobGoblinsKilled => totalHobGoblinsKilled;

    /// <summary>
    /// Gets the total number of Goblin enemies killed.
    /// </summary>
    public int TotalGoblinsKilled => totalGoblinsKilled;

    /// <summary>
    /// Gets the total number of MonsterMutant enemies killed.
    /// </summary>
    public int TotalMonsterMutantsKilled => totalMonsterMutantsKilled;

    /// <summary>
    /// Gets the total number of Executioner enemies killed.
    /// </summary>
    public int TotalExecutionersKilled => totalExecutionersKilled;
    #endregion
}
