using UnityEngine;

[System.Serializable]
public class StatisticsData
{
    public float totalDistance;
    public float totalTimePlayed;
    public int totalJumps;
    public int totalDeaths;
    public int totalEnemiesKilled;
    public int totalBearsKilled;
    public int totalWolfesKilled;
    public int totalTrollsKilled;
    public int totalHobGoblinsKilled;
    public int totalGoblinsKilled;
    public int totalMonsterMutantsKilled;
    public int totalExecutionersKilled;

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
}
