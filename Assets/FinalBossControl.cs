using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StartNpc))]
[RequireComponent(typeof(NavMeshAgent))]
public class FinalBossControl : MonoBehaviour
{
    private StartNpc startNpc;
    private NavMeshAgent navMeshAgent;
    private Entity entity;

    private GameObject player;
    private string playerTag = "Player";

    private bool stayInPlace = false;

    private void Start()
    {
        startNpc = GetComponent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        player = GameObject.FindGameObjectWithTag(playerTag);
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (stayInPlace)
        {
            lookAtPlayer();
        }
        moveToPlayer();
    }

    private void lookAtPlayer()
    {
        transform.LookAt(player.transform);
    }

    private void moveToPlayer()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private void respawnEnemy() { }
}
