using UnityEngine;

public class WolfBossChasing : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool canMove = true;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        if (player != null && navMeshAgent.enabled && !navMeshAgent.isStopped && canMove)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("IsChasing", true);
        }
        else
        {
            animator.SetBool("IsChasing", false);
        }
    }

    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
