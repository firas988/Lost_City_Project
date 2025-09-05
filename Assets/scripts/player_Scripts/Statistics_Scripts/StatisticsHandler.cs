using UnityEngine;

/// <summary>
/// Manages comprehensive player statistics tracking including movement, combat, and gameplay metrics.
/// Tracks distance traveled, time played, jumps, deaths, and enemy kills by type.
/// Integrates with save system for persistent statistics across game sessions.
/// </summary>
[RequireComponent(typeof(PlayerDistanceTracker))]
public class StatisticsHandler : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to PlayerDistanceTracker for monitoring player movement distance.
    /// Required component for distance calculation and tracking.
    /// </summary>
    private PlayerDistanceTracker playerDistanceTracker;

    /// <summary>
    /// Reference to Player for accessing player stats.
    /// Used for calculating total health and strength.
    /// </summary>
    private Player player;
    #endregion

    #region Core Statistics
    /// <summary>
    /// Distance traveled from previous save sessions.
    /// Loaded from save data and added to current session distance.
    /// </summary>
    private float loadedTotalDistance = 0f;

    /// <summary>
    /// Total distance traveled across all game sessions.
    /// Combines loaded distance with current session distance.
    /// </summary>
    private float totalDistance = 0f;

    /// <summary>
    /// Total time played across all game sessions.
    /// Uses unscaled time to track actual play time regardless of game speed.
    /// </summary>
    private float totalTimePlayed = 0f;

    /// <summary>
    /// Total number of jumps performed by the player.
    /// Incremented each time the player jumps.
    /// </summary>
    private int totalJumps = 0;

    /// <summary>
    /// Total number of player deaths across all game sessions.
    /// Incremented each time the player dies.
    /// </summary>
    private int totalDeaths = 0;
    #endregion

    #region Enemy Kill Statistics
    /// <summary>
    /// Total number of enemies killed across all types.
    /// General counter for all enemy eliminations.
    /// </summary>
    private int totalEnemiesKilled = 0;

    /// <summary>
    /// Total number of Bear enemies killed.
    /// Specific counter for Bear-type enemies.
    /// </summary>
    private int totalBearsKilled = 0;

    /// <summary>
    /// Total number of Wolf enemies killed.
    /// Specific counter for Wolf-type enemies.
    /// </summary>
    private int totalWolfesKilled = 0;

    /// <summary>
    /// Total number of Troll enemies killed.
    /// Specific counter for Troll-type enemies.
    /// </summary>
    private int totalTrollsKilled = 0;

    /// <summary>
    /// Total number of HobGoblin enemies killed.
    /// Specific counter for HobGoblin-type enemies.
    /// </summary>
    private int totalHobGoblinsKilled = 0;

    /// <summary>
    /// Total number of Goblin enemies killed.
    /// Specific counter for Goblin-type enemies.
    /// </summary>
    private int totalGoblinsKilled = 0;

    /// <summary>
    /// Total number of MonsterMutant enemies killed.
    /// Specific counter for MonsterMutant-type enemies.
    /// </summary>
    private int totalMonsterMutantsKilled = 0;

    /// <summary>
    /// Total number of Executioner enemies killed.
    /// Specific counter for Executioner-type enemies.
    /// </summary>
    private int totalExecutionersKilled = 0;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by getting required component references.
    /// Sets up the distance tracker for movement monitoring.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(1)
    private void Awake()
    {
        // Get the required PlayerDistanceTracker component
        playerDistanceTracker = GetComponent<PlayerDistanceTracker>();

        // Get the required Player component
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
    }

    /// <summary>
    /// Sets up event subscriptions and initializes the statistics system.
    /// Subscribes to enemy death events for kill tracking.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(1)
    void Start()
    {
        // Subscribe to enemy death events for kill tracking
        KillEnemyHandler.Subscribe(KilledEnemy);
    }

    /// <summary>
    /// Updates statistics each frame including distance and time tracking.
    /// Continuously monitors player progress and gameplay metrics.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Ensure player reference exists (fallback if lost)
        if (player == null)
        {
            player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
            return;
        }

        // Update distance and time statistics each frame
        updateTotalDistance();
        updateTotalTimePlayed();
    }
    #endregion

    #region Statistics Updates
    /// <summary>
    /// Updates the total distance traveled by combining current and loaded distances.
    /// Calculates cumulative distance across all game sessions.
    /// </summary>
    // COMPLEXITY ANALYSIS: updateTotalDistance() - O(1)
    private void updateTotalDistance()
    {
        // Combine current session distance with previously saved distance
        totalDistance = playerDistanceTracker.getDistance() + loadedTotalDistance;
    }

    /// <summary>
    /// Updates the total time played using unscaled delta time.
    /// Ensures accurate time tracking regardless of game speed or pausing.
    /// </summary>
    // COMPLEXITY ANALYSIS: updateTotalTimePlayed() - O(1)
    private void updateTotalTimePlayed()
    {
        // Add frame time to total play time (unscaled for accuracy)
        totalTimePlayed += Time.unscaledDeltaTime;
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Increments the total jump counter when called.
    /// Called by the movement system when player performs a jump.
    /// </summary>
    // COMPLEXITY ANALYSIS: Jumping() - O(1)
    public void Jumping()
    {
        totalJumps++;
    }

    /// <summary>
    /// Increments the total death counter when called.
    /// Called by the death system when player dies.
    /// </summary>
    // COMPLEXITY ANALYSIS: Death() - O(1)
    public void Death()
    {
        totalDeaths++;
    }

    /// <summary>
    /// Handles enemy death events and updates specific enemy type counters.
    /// Increments both general enemy counter and specific type counter.
    /// </summary>
    /// <param name="tag">The enemy type tag for specific counter updates.</param>
    // COMPLEXITY ANALYSIS: KilledEnemy() - O(1)
    public void KilledEnemy(string tag)
    {
        // Increment general enemy kill counter
        totalEnemiesKilled++;

        // Update specific enemy type counter based on tag
        switch (tag)
        {
            case "Bear":
                totalBearsKilled++;
                break;
            case "Wolf":
                totalWolfesKilled++;
                break;
            case "Troll":
                totalTrollsKilled++;
                break;
            case "HobGoblin":
                totalHobGoblinsKilled++;
                break;
            case "Goblin":
                totalGoblinsKilled++;
                break;
            case "MonsterMutant":
                totalMonsterMutantsKilled++;
                break;
            case "Executioner":
                totalExecutionersKilled++;
                break;
        }
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the total distance traveled across all game sessions.
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
    /// Gets the total number of player deaths across all game sessions.
    /// </summary>
    public int TotalDeaths => totalDeaths;

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

    /// <summary>
    /// Gets the total health of the player.
    /// </summary>
    /// <returns>The total health of the player.</returns>
    public float TotalHealth => player.getMaxHealth();

    /// <summary>
    /// Gets the total strength of the player.
    /// </summary>
    /// <returns>The total strength of the player.</returns>
    public float TotalStrength => player.getDamage();

    /// <summary>
    /// Gets the total defense of the player.
    /// </summary>
    /// <returns>The total defense of the player.</returns>
    public float TotalDefense => player.getCurrentDefense();

    /// <summary>
    /// Gets the total speed of the player.
    /// </summary>
    /// <returns>The total speed of the player.</returns>
    public float TotalSpeed => player.getCurrentSpeed();
    #endregion

    #region Save and Load System
    /// <summary>
    /// Saves current statistics to the save system.
    /// Persists all tracked metrics for future game sessions.
    /// </summary>
    // COMPLEXITY ANALYSIS: SaveStatistics() - O(1)
    public void SaveStatistics()
    {
        // Save current statistics to persistent storage
        SaveSystem.SaveStatistics(this);
    }

    /// <summary>
    /// Gets a reference to this StatisticsHandler instance.
    /// Used by other systems to access statistics data.
    /// </summary>
    /// <returns>Reference to this StatisticsHandler component.</returns>
    // COMPLEXITY ANALYSIS: GetStatisticsHandler() - O(1)
    public StatisticsHandler GetStatisticsHandler()
    {
        return this;
    }

    /// <summary>
    /// Loads statistics from saved data and initializes counters.
    /// Restores previous session statistics on game load.
    /// </summary>
    /// <param name="statisticsData">Saved statistics data to load from.</param>
    // COMPLEXITY ANALYSIS: LoadStatistics() - O(1)
    public void LoadStatistics(StatisticsData statisticsData)
    {
        if (statisticsData != null)
        {
            // Load all saved statistics from previous sessions
            loadedTotalDistance = statisticsData.TotalDistance;
            totalTimePlayed = statisticsData.TotalTimePlayed;
            totalJumps = statisticsData.TotalJumps;
            totalDeaths = statisticsData.TotalDeaths;
            totalEnemiesKilled = statisticsData.TotalEnemiesKilled;
            totalBearsKilled = statisticsData.TotalBearsKilled;
            totalWolfesKilled = statisticsData.TotalWolfesKilled;
            totalTrollsKilled = statisticsData.TotalTrollsKilled;
            totalHobGoblinsKilled = statisticsData.TotalHobGoblinsKilled;
            totalGoblinsKilled = statisticsData.TotalGoblinsKilled;
            totalMonsterMutantsKilled = statisticsData.TotalMonsterMutantsKilled;
            totalExecutionersKilled = statisticsData.TotalExecutionersKilled;
        }
    }
    #endregion
}
