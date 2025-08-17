using UnityEngine;

[RequireComponent(typeof(PlayerDistanceTracker))]
public class StatisticsHandler : MonoBehaviour
{
    private PlayerDistanceTracker playerDistanceTracker;

    private float loadedTotalDistance = 0f;
    private float totalDistance = 0f;
    private float totalTimePlayed = 0f;
    private int totalJumps = 0;
    private int totalDeaths = 0;

    //  =====ENEMIES KILLED =====
    private int totalEnemiesKilled = 0;
    private int totalBearsKilled = 0;
    private int totalWolfesKilled = 0;
    private int totalTrollsKilled = 0;
    private int totalHobGoblinsKilled = 0;
    private int totalGoblinsKilled = 0;
    private int totalMonsterMutantsKilled = 0;
    private int totalExecutionersKilled = 0;

    private void Awake()
    {
        playerDistanceTracker = GetComponent<PlayerDistanceTracker>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadStatistics();
        KillEnemyHandler.Subscribe(KilledEnemy);
    }

    // Update is called once per frame
    void Update()
    {
        updateTotalDistance();
        updateTotalTimePlayed();

        if (Input.GetKeyDown(KeyCode.H))
        {
            SaveStatistics();
        }
    }

    private void updateTotalDistance()
    {
        totalDistance = playerDistanceTracker.getDistance() + loadedTotalDistance;
    }

    private void updateTotalTimePlayed()
    {
        totalTimePlayed += Time.unscaledDeltaTime;
    }

    public void Jumping()
    {
        totalJumps++;
    }

    public void Death()
    {
        totalDeaths++;
    }

    public void KilledEnemy(string tag)
    {
        totalEnemiesKilled++;
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

    public void SaveStatistics()
    {
        SaveSystem.SaveStatistics(this);
    }

    public void LoadStatistics()
    {
        StatisticsData statisticsData = SaveSystem.LoadStatistics();
        if (statisticsData != null)
        {
            loadedTotalDistance = statisticsData.totalDistance;
            totalTimePlayed = statisticsData.totalTimePlayed;
            totalJumps = statisticsData.totalJumps;
            totalDeaths = statisticsData.totalDeaths;
            totalEnemiesKilled = statisticsData.totalEnemiesKilled;
            totalBearsKilled = statisticsData.totalBearsKilled;
            totalWolfesKilled = statisticsData.totalWolfesKilled;
            totalTrollsKilled = statisticsData.totalTrollsKilled;
            totalHobGoblinsKilled = statisticsData.totalHobGoblinsKilled;
            totalGoblinsKilled = statisticsData.totalGoblinsKilled;
            totalMonsterMutantsKilled = statisticsData.totalMonsterMutantsKilled;
            totalExecutionersKilled = statisticsData.totalExecutionersKilled;
        }
    }
}
