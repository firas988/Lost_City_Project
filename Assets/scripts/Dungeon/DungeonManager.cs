using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> rooms;

    private int currentRoomIndex;

    private GameObject boss;

    void Start()
    {
        currentRoomIndex = 0;
        boss = GameObject.FindWithTag("WolfBoss");
        foreach (GameObject room in rooms)
        {
            GameObject enemies = room.transform.Find("Enemies").gameObject;
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
            foreach (Transform child in rooms[currentRoomIndex].transform)
            {
                if (child.gameObject.tag == "RoomExit")
                {
                    child.gameObject.SetActive(true);
                }
            }

            GameObject enemies = rooms[currentRoomIndex].transform.Find("Enemies").gameObject;
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

        yield return new WaitForSeconds(2.5f);

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

    public void spawnBoss()
    {
        boss.SetActive(true);
        boss.GetComponent<WolfBossChasing>().setCanMove(false);
        StartCoroutine(WaitForEnemiesToDeDissolve(boss));
    }

    public void NextRoom()
    {
        //find the levelExit in the current room
        foreach (Transform child in rooms[currentRoomIndex].transform)
        {
            if (child.gameObject.tag == "RoomExit")
            {
                child.gameObject.SetActive(false);
            }
        }

        currentRoomIndex++;

        Debug.Log("currentRoomIndex: " + currentRoomIndex);
    }

    public void StartFinallBossScene()
    {
        GameObject.Find("dungeon").transform.Find("FinalBossEnter").gameObject.SetActive(true);
    }

    public void StopFinallBossScene()
    {
        GameObject.Find("dungeon").transform.Find("FinalBossEnter").gameObject.SetActive(false);
    }
}
