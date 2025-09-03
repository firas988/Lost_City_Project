using TMPro;
using UnityEngine;

/// <summary>
/// Handles the display of player statistics in the UI.
/// Updates various stat displays including time played, distance, jumps, deaths, and enemy kills.
/// </summary>
public class StatsUiHandler : MonoBehaviour
{
    #region Private Fields

    /// <summary>
    /// Reference to the statistics handler for retrieving player stats.
    /// </summary>
    private StatisticsHandler statisticsHandler;

    #endregion

    #region Serialized Fields

    /// <summary>
    /// Text component for displaying total time played.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalTimePlayedText;

    /// <summary>
    /// Text component for displaying total distance traveled.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalDistanceText;

    /// <summary>
    /// Text component for displaying total number of jumps.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalJumpsText;

    /// <summary>
    /// Text component for displaying total number of deaths.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalDeathsText;

    /// <summary>
    /// Text component for displaying total number of enemies killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalEnemiesKilledText;

    /// <summary>
    /// Text component for displaying total number of bears killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalBearsKilledText;

    /// <summary>
    /// Text component for displaying total number of wolves killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalWolfesKilledText;

    /// <summary>
    /// Text component for displaying total number of trolls killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalTrollsKilledText;

    /// <summary>
    /// Text component for displaying total number of hobgoblins killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalHobGoblinsKilledText;

    /// <summary>
    /// Text component for displaying total number of goblins killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalGoblinsKilledText;

    /// <summary>
    /// Text component for displaying total number of monster mutants killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalMonsterMutantsKilledText;

    /// <summary>
    /// Text component for displaying total number of executioners killed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalExecutionersKilledText;

    /// <summary>
    /// Text component for displaying total health.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalHealthText;

    /// <summary>
    /// Text component for displaying total strength.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalStrengthText;

    /// <summary>
    /// Text component for displaying total defense.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalDefenseText;

    /// <summary>
    /// Text component for displaying total speed.
    /// </summary>
    [SerializeField]
    private TextMeshPro totalSpeedText;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the stats UI handler by finding the statistics handler component.
    /// </summary>
    private void Awake()
    {
        statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
    }

    /// <summary>
    /// Updates the statistics UI each frame, ensuring the statistics handler is available.
    /// </summary>
    private void Update()
    {
        // Ensure statistics handler is available
        if (statisticsHandler == null)
        {
            statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
            return;
        }

        UpdateStatsUI();
    }

    #endregion

    #region UI Update Methods

    /// <summary>
    /// Updates all statistics UI elements with current values from the statistics handler.
    /// </summary>
    private void UpdateStatsUI()
    {
        // Update distance display with appropriate units
        float totalDistance = statisticsHandler.TotalDistance;
        if (totalDistance > 1000)
        {
            totalDistanceText.text = (totalDistance / 1000).ToString("F2") + " km";
        }
        else
        {
            totalDistanceText.text = totalDistance.ToString("F2") + " m";
        }

        // Update time played display in HH:MM:SS format
        float totalTimePlayed = statisticsHandler.TotalTimePlayed;
        int hours = Mathf.FloorToInt(totalTimePlayed / 3600);
        int minutes = Mathf.FloorToInt((totalTimePlayed % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalTimePlayed % 60);
        totalTimePlayedText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        // Update all kill count displays
        totalJumpsText.text = statisticsHandler.TotalJumps.ToString();
        totalDeathsText.text = statisticsHandler.TotalDeaths.ToString();
        totalEnemiesKilledText.text = statisticsHandler.TotalEnemiesKilled.ToString();
        totalBearsKilledText.text = statisticsHandler.TotalBearsKilled.ToString();
        totalWolfesKilledText.text = statisticsHandler.TotalWolfesKilled.ToString();
        totalTrollsKilledText.text = statisticsHandler.TotalTrollsKilled.ToString();
        totalHobGoblinsKilledText.text = statisticsHandler.TotalHobGoblinsKilled.ToString();
        totalGoblinsKilledText.text = statisticsHandler.TotalGoblinsKilled.ToString();
        totalMonsterMutantsKilledText.text = statisticsHandler.TotalMonsterMutantsKilled.ToString();
        totalExecutionersKilledText.text = statisticsHandler.TotalExecutionersKilled.ToString();
        try
        {
            totalHealthText.text = statisticsHandler.TotalHealth.ToString();
            totalStrengthText.text = statisticsHandler.TotalStrength.ToString();
            totalDefenseText.text = statisticsHandler.TotalDefense.ToString();
            totalSpeedText.text = statisticsHandler.TotalSpeed.ToString();
        }
        catch (System.Exception)
        {
            totalHealthText.text = "0";
            totalStrengthText.text = "0";
            totalDefenseText.text = "0";
            totalSpeedText.text = "0";
        }
    }

    #endregion
}
