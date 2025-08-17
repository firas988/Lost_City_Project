using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls a patrol trigger zone. When an enemy enters or exits the zone,
/// it updates their movement behavior accordingly.
/// </summary>
public class PatrolZone_Trigger : MonoBehaviour
{
    /// <summary>
    /// The radius of the patrol area.
    /// </summary>
    [SerializeField]
    private float patrolRange = 20f;

    /// <summary>
    /// Sets the capsule collider radius to match the patrol range at start.
    /// </summary>
    private void Start()
    {
        CapsuleCollider sc = GetComponent<CapsuleCollider>();
        if (sc != null)
            sc.radius = patrolRange;
    }

    /// <summary>
    /// Returns the patrol range.
    /// </summary>
    /// <returns>The patrol range.</returns>
    public float getPatrolRange()
    {
        return patrolRange;
    }

    /// <summary>
    /// Triggered when another collider exits this trigger.
    /// If it's an enemy, the enemy stops chasing or attacking and starts returning.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            NavMeshAgent navMeshAgent = other.GetComponent<NavMeshAgent>();
            Animator animator = other.GetComponent<Animator>();
            if (enemyMovement != null && navMeshAgent != null)
            {
                enemyMovement.setIsChassing(false);
                enemyMovement.setIsAttacking(false);
                enemyMovement.setIsReturn(true);
                animator.SetBool("isWalking", true);
                navMeshAgent.SetDestination(transform.position);
            }
        }
    }

    /// <summary>
    /// Triggered when another collider enters this trigger.
    /// If it's an enemy, it stops returning and its walk timer is reset.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.setIsReturn(false);
                StartCoroutine(resetTime(other));
            }
        }
    }

    /// <summary>
    /// Coroutine that waits a random amount of time before resetting the NPC's walk time.
    /// </summary>
    /// <param name="other">The collider containing the NPCnavigation component.</param>
    /// <returns>IEnumerator used for coroutine timing.</returns>
    private IEnumerator resetTime(Collider other)
    {
        NPCnavigation npcNavigation = other.GetComponent<NPCnavigation>();
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        npcNavigation.setWalkTime(0f);
    }

    /// enable this to see the patrol zone in the editor
    /// <summary>
    /// Draws the patrol zone radius in the editor using Gizmos (for visualization).
    /// </summary>
    void OnDrawGizmos()
    {
        CapsuleCollider sc = GetComponent<CapsuleCollider>();
        if (sc != null)
        {
            Gizmos.color = Color.blue;
            Vector3 center = transform.position + sc.center;
            Gizmos.DrawWireSphere(center, sc.radius * transform.lossyScale.x);
        }
    }
}
