using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls a patrol trigger zone. When an enemy enters or exits the zone,
/// it updates their movement behavior accordingly.
/// Manages enemy AI state transitions between patrolling, chasing, and returning.
/// </summary>
public class PatrolZone_Trigger : MonoBehaviour
{
    #region Configuration
    /// <summary>
    /// The radius of the patrol area in world units.
    /// </summary>
    [SerializeField]
    private float patrolRange = 20f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Sets the capsule collider radius to match the patrol range at start.
    /// </summary>
    private void Start()
    {
        // Get capsule collider and set radius to match patrol range
        CapsuleCollider sc = GetComponent<CapsuleCollider>();
        if (sc != null)
            sc.radius = patrolRange;
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Returns the patrol range value.
    /// </summary>
    /// <returns>The patrol range in world units.</returns>
    public float getPatrolRange()
    {
        return patrolRange;
    }
    #endregion

    #region Trigger Event Handling
    /// <summary>
    /// Triggered when another collider exits this trigger.
    /// If it's an enemy, the enemy stops chasing or attacking and starts returning.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is an enemy
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            // Get required components from the enemy
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            NavMeshAgent navMeshAgent = other.GetComponent<NavMeshAgent>();
            Animator animator = other.GetComponent<Animator>();

            if (enemyMovement != null && navMeshAgent != null)
            {
                // Stop chasing and attacking, start returning to patrol zone
                enemyMovement.setIsChassing(false);
                enemyMovement.setIsAttacking(false);
                enemyMovement.setIsReturn(true);

                // Set walking animation and return to center of patrol zone
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
        // Check if the entering object is an enemy
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                // Stop returning and reset walk timer
                enemyMovement.setIsReturn(false);
                StartCoroutine(resetTime(other));
            }
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine that waits a random amount of time before resetting the NPC's walk time.
    /// </summary>
    /// <param name="other">The collider containing the NPCnavigation component.</param>
    /// <returns>IEnumerator used for coroutine timing.</returns>
    private IEnumerator resetTime(Collider other)
    {
        // Get NPC navigation component
        NPCnavigation npcNavigation = other.GetComponent<NPCnavigation>();

        // Wait for random time between 3-5 seconds
        yield return new WaitForSeconds(Random.Range(3f, 5f));

        // Reset walk time to allow new patrol behavior
        npcNavigation.setWalkTime(0f);
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the patrol zone in the editor
    /// <summary>
    /// Draws the patrol zone radius in the editor using Gizmos (for visualization).
    /// </summary>
    void OnDrawGizmos()
    {
        // Get capsule collider for visualization
        CapsuleCollider sc = GetComponent<CapsuleCollider>();
        if (sc != null)
        {
            // Draw blue wire sphere to show patrol zone
            Gizmos.color = Color.blue;
            Vector3 center = transform.position + sc.center;
            Gizmos.DrawWireSphere(center, sc.radius * transform.lossyScale.x);
        }
    }
    #endregion
}
