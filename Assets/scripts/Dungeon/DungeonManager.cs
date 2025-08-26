using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> rooms;

    // private string bossTag = "WolfBoss";

    [SerializeField]
    private string roomExitTag = "RoomExit";

    [SerializeField]
    private string enemiesName = "Enemies";

    [SerializeField]
    private GameObject finalBossEnter;

    private QuestManager questManager;
    private Player player;

    private int currentRoomIndex;

    [SerializeField]
    private GameObject boss;

    void Start()
    {
        currentRoomIndex = 0;

        player = GameObject.FindWithTag("Player").GetComponent<StartPlayer>().getPlayer();

        questManager = GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponentInChildren<QuestManager>();

        // boss = GameObject.FindWithTag(bossTag);
        foreach (GameObject room in rooms)
        {
            GameObject enemies = room.transform.Find(enemiesName).gameObject;
            if (enemies != null)
            {
                foreach (Transform child in enemies.transform)
                {
                    child.gameObject.GetComponent<DissolvingController>().setDissolveAmount(1f);

                    EnemyMovement enemyMovement = child.gameObject.GetComponent<EnemyMovement>();
                    EnemyHealthBar enemyHealthBar =
                        child.gameObject.GetComponentInChildren<EnemyHealthBar>();
                    if (enemyMovement != null && enemyHealthBar != null)
                    {
                        enemyMovement.setCanMove(false);
                        enemyHealthBar.hideHealthBar();
                    }
                    else
                    {
                        child.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    }
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartDungeon()
    { //find the levelExit in the current room
        if (currentRoomIndex < rooms.Count)
        {
            if (currentRoomIndex == rooms.Count - 1)
            {
                StartFinallBossScene();

                return;
            }

            if (questManager.checkingCompletedStoryQuest(player.getCurrentMainQuest().GetType()))
            {
                NextRoom();
                return;
            }
            blockCurrentRoom();

            GameObject enemies = rooms[currentRoomIndex].transform.Find(enemiesName).gameObject;
            if (enemies != null)
            {
                foreach (Transform child in enemies.transform)
                {
                    child.gameObject.SetActive(true);

                    StartCoroutine(WaitForEnemiesToDeDissolve(child.gameObject));
                }
            }
        }
    }

    public IEnumerator WaitForEnemiesToDeDissolve(GameObject enemy)
    {
        enemy.GetComponent<DissolvingController>().StartDeDissolve();

        yield return new WaitForSeconds(5f);

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        EnemyHealthBar enemyHealthBar = enemy.GetComponentInChildren<EnemyHealthBar>();
        if (enemyMovement != null && enemyHealthBar != null)
        {
            enemyHealthBar.showHealthBar();
            enemyMovement.setCanMove(true);
        }
        else
        {
            WolfBossChasing wolfBossChasing = enemy.GetComponent<WolfBossChasing>();
            if (wolfBossChasing != null)
            {
                wolfBossChasing.setCanMove(true);
            }

            UnityEngine.AI.NavMeshAgent navMeshAgent =
                enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
        }
    }

    public void blockCurrentRoom()
    {
        foreach (Transform child in rooms[currentRoomIndex].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void spawnBoss()
    {
        boss.SetActive(true);
        boss.GetComponent<WolfBossChasing>().setCanMove(false);
        blockCurrentRoom();
    }

    public void DeDissolveBoss()
    {
        StartCoroutine(WaitForEnemiesToDeDissolve(boss));
    }

    public void NextRoom()
    {
        GameObject enemies = rooms[currentRoomIndex].transform.Find(enemiesName).gameObject;
        enemies.SetActive(false);
        //find the levelExit in the current room
        foreach (Transform child in rooms[currentRoomIndex].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(false);
            }
        }

        currentRoomIndex++;

        Debug.Log("currentRoomIndex: " + currentRoomIndex);
    }

    public void StartFinallBossScene()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerController>().transform.position = rooms[rooms.Count - 1]
            .transform.Find("Entrance")
            .position;
        player.SetActive(false);

        Debug.Log("StartFinallBossScene");
        finalBossEnter.SetActive(true);
    }

    public void StopFinallBossScene()
    {
        Debug.Log("StopFinallBossScene");
        finalBossEnter.SetActive(false);
    }

    public void closeFinalRoom()
    {
        foreach (Transform child in rooms[rooms.Count - 1].transform)
        {
            if (child.gameObject.tag == roomExitTag)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
