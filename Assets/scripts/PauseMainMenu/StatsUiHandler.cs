using TMPro;
using UnityEngine;

public class StatsUiHandler : MonoBehaviour
{
    private StatisticsHandler statisticsHandler;

    [SerializeField]
    private TextMeshPro totalTimePlayedText;

    [SerializeField]
    private TextMeshPro totalDistanceText;

    [SerializeField]
    private TextMeshPro totalJumpsText;

    [SerializeField]
    private TextMeshPro totalDeathsText;

    [SerializeField]
    private TextMeshPro totalEnemiesKilledText;

    [SerializeField]
    private TextMeshPro totalBearsKilledText;

    [SerializeField]
    private TextMeshPro totalWolfesKilledText;

    [SerializeField]
    private TextMeshPro totalTrollsKilledText;

    [SerializeField]
    private TextMeshPro totalHobGoblinsKilledText;

    [SerializeField]
    private TextMeshPro totalGoblinsKilledText;

    [SerializeField]
    private TextMeshPro totalMonsterMutantsKilledText;

    [SerializeField]
    private TextMeshPro totalExecutionersKilledText;

    private void Awake()
    {
        statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
    }

    private void Update()
    {
        if (statisticsHandler == null)
        {
            statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
            return;
        }
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        totalDistanceText.text = statisticsHandler.TotalDistance.ToString("F2") + " m";
        float totalTimePlayed = statisticsHandler.TotalTimePlayed;
        int hours = Mathf.FloorToInt(totalTimePlayed / 3600);
        int minutes = Mathf.FloorToInt((totalTimePlayed % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalTimePlayed % 60);
        totalTimePlayedText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
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
    }
}
