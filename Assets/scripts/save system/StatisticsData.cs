using UnityEngine;

[System.Serializable]
public class StatisticsData
{
    [SerializeField]
    private float totalDistance;
    [SerializeField]
    private float totalTimePlayed;
    [SerializeField]
    private int totalJumps;
    [SerializeField]
    private int totalDeaths;
    [SerializeField]
    private int totalEnemiesKilled;
    [SerializeField]
    private int totalBearsKilled;
    [SerializeField]
    private int totalWolfesKilled;
    [SerializeField]
    private int totalTrollsKilled;
    [SerializeField]
    private int totalHobGoblinsKilled;
    [SerializeField]
    private int totalGoblinsKilled;
    [SerializeField]
    private int totalMonsterMutantsKilled;
    [SerializeField]
    private int totalExecutionersKilled;

    public StatisticsData(StatisticsHandler statisticsHandler)
    {
        totalDistance = statisticsHandler.TotalDistance;
        totalTimePlayed = statisticsHandler.TotalTimePlayed;
        totalJumps = statisticsHandler.TotalJumps;
        totalDeaths = statisticsHandler.TotalDeaths;
        totalEnemiesKilled = statisticsHandler.TotalEnemiesKilled;
        totalBearsKilled = statisticsHandler.TotalBearsKilled;
        totalWolfesKilled = statisticsHandler.TotalWolfesKilled;
        totalTrollsKilled = statisticsHandler.TotalTrollsKilled;
        totalHobGoblinsKilled = statisticsHandler.TotalHobGoblinsKilled;
        totalGoblinsKilled = statisticsHandler.TotalGoblinsKilled;
        totalMonsterMutantsKilled = statisticsHandler.TotalMonsterMutantsKilled;
        totalExecutionersKilled = statisticsHandler.TotalExecutionersKilled;
    }

    public float TotalDistance => totalDistance;
    public float TotalTimePlayed => totalTimePlayed;
    public int TotalJumps => totalJumps;
    public int TotalDeaths => totalDeaths;
    public int TotalEnemiesKilled => totalEnemiesKilled;
    public int TotalBearsKilled => totalBearsKilled;
    public int TotalWolfesKilled => totalWolfesKilled;
    public int TotalTrollsKilled => totalTrollsKilled;
    public int TotalHobGoblinsKilled => totalHobGoblinsKilled;
    public int TotalGoblinsKilled => totalGoblinsKilled;
    public int TotalMonsterMutantsKilled => totalMonsterMutantsKilled;
    public int TotalExecutionersKilled => totalExecutionersKilled;
}
