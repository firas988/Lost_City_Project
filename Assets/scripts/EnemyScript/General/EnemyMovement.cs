using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles enemy AI movement behavior, including chasing the player,
/// attacking when in range, and returning to patrol mode.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    /// <summary>Reference to the NavMeshAgent controlling movement.</summary>
    private NavMeshAgent agent;

    /// <summary>Transform of the player (target).</summary>
    private Transform playerTransform = null;

    /// <summary>Transform of the enemy itself.</summary>
    private Transform enemyTransform = null;

    /// <summary>Reference to the NPC navigation script for patrolling.</summary>
    private NPCnavigation npcNavigation;

    /// <summary>Reference to the current enemy attack behavior.</summary>
    private EnemyAttackBehavior enemyAttackBehavior;

    /// <summary>Reference to the enemy animator control.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Distance within which chasing is triggered.</summary>
    [SerializeField]
    private float activateChassingRange = 10f;

    /// <summary>Maximum distance to keep chasing the player before giving up.</summary>
    [SerializeField]
    private float chassingRange = 20f;

    /// <summary>Whether the enemy is returning to the patrol zone.</summary>
    [SerializeField]
    private bool isReturn = false;

    /// <summary>Whether the enemy is currently chasing the player.</summary>
    [SerializeField]
    private bool isChassing = false;

    /// <summary>Whether the enemy is currently attacking.</summary>
    [SerializeField]
    private bool isAttacking = false;

    /// <summary>Whether the enemy can move.</summary>
    [SerializeField]
    private bool canMove = true;

    /// <summary>Whether the enemy is in cooldown.</summary>
    [SerializeField]
    private bool inCooldown = false;

    /// <summary>Sets the canMove state.</summary>
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    /// <summary>Sets the return-to-patrol state.</summary>
    public void setIsReturn(bool isReturn)
    {
        this.isReturn = isReturn;
    }

    /// <summary>Returns whether the enemy is chasing the player.</summary>
    public bool getIsChassing()
    {
        return isChassing;
    }

    /// <summary>Sets the chasing state.</summary>
    public void setIsChassing(bool isChassing)
    {
        this.isChassing = isChassing;
    }

    /// <summary>Returns whether the enemy is attacking.</summary>
    public bool getIsAttacking()
    {
        return isAttacking;
    }

    /// <summary>Sets the attacking state.</summary>
    public void setIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    /// <summary>
    /// Initializes references to required components.
    /// </summary>
    private void Awake()
    {
        // Get NavMeshAgent for movement.
        agent = GetComponent<NavMeshAgent>();

        // Get this enemy's transform.
        enemyTransform = GetComponent<Transform>();

        // Get reference to NPC patrol movement controller.
        npcNavigation = GetComponent<NPCnavigation>();

        // Get reference to attack logic.
        enemyAttackBehavior = GetComponent<EnemyAttackBehavior>();

        // Get reference to animator control.
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();

        // Warn if no NavMeshAgent is attached.
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
        }
    }

    /// <summary>
    /// Updates movement and state logic every frame.
    /// </summary>
    private void Update()
    {
        if (agent == null || enemyTransform == null)
            return;

        if (!canMove)
        {
            agent.SetDestination(enemyTransform.position);
            return;
        }

        // If returning to patrol zone, reset chasing and player tracking.
        if (isReturn)
        {
            isChassing = false;
            playerTransform = null;
            StartCoroutine(returnToPatrolZone());
        }

        // If not attacking, allow chasing logic.
        if (!isAttacking && !inCooldown)
        {
            npcNavigation.itWalk();
            actvateChassing();

            if (isChassing)
            {
                chassing();
            }
        }

        // If a player is tracked, check for attack opportunity.
        if (playerTransform != null)
        {
            activateAttack();
        }

        checkCooldown();
    }

    /// <summary>
    /// Coroutine that delays resetting to patrol mode for 5 seconds.
    /// </summary>
    private IEnumerator returnToPatrolZone()
    {
        // Wait for 5 seconds before allowing wandering again.
        yield return new WaitForSeconds(5f);
        npcNavigation.setIsWandering(true);
    }

    /// <summary>
    /// Scans the nearby area for the player to start chasing if in range.
    /// </summary>
    private void actvateChassing()
    {
        // Check all colliders in a sphere around the enemy.
        Collider[] hits = Physics.OverlapSphere(enemyTransform.position, activateChassingRange);
        foreach (Collider col in hits)
        {
            // If the collider is the player and the enemy is not returning.
            if (col.CompareTag("Player") && !isReturn)
            {
                playerTransform = col.transform;

                // Set destination toward player.
                agent.SetDestination(playerTransform.position);

                // Start chasing.
                isChassing = true;

                // Stop patrolling if active.
                if (npcNavigation.getIsWandering())
                {
                    npcNavigation.setIsWandering(false);
                }
            }
        }
    }

    /// <summary>
    /// Handles chasing behavior while the player is within chase range.
    /// </summary>
    private void chassing()
    {
        if (isChassing)
        {
            npcNavigation.itRun();
            // Check distance to player.
            float distance = Vector3.Distance(enemyTransform.position, playerTransform.position);
            if (distance <= chassingRange)
            {
                // Keep chasing the player.
                agent.SetDestination(playerTransform.position);
            }
            else
            {
                // Stop chasing and reset player.
                isChassing = false;
                playerTransform = null;

                // Go back to original position and resume wandering.
                if (!npcNavigation.getIsWandering())
                {
                    agent.SetDestination(enemyTransform.position);
                }

                npcNavigation.setIsWandering(true);
            }
        }
    }

    /// <summary>
    /// Checks if the player is in attack range and within field of view,
    /// then triggers attack or resets state if out of range.
    /// </summary>
    private void activateAttack()
    {
        // Measure distance to player.
        float distance = Vector3.Distance(enemyTransform.position, playerTransform.position);

        // Get direction and angle between enemy and player.
        Vector3 directionToPlayer = (playerTransform.position - enemyTransform.position).normalized;
        float angle = Vector3.Angle(enemyTransform.forward, directionToPlayer);
        float visionAngle = 100f; // Field of view angle.

        // If within attack range and FOV and not returning, start attacking.
        if (
            distance <= enemyAttackBehavior.getAttackRange()
            && angle <= visionAngle
            && !isReturn
            && !inCooldown
        )
        {
            isChassing = false;
            isAttacking = true;

            // Stop movement while attacking.
            agent.SetDestination(enemyTransform.position);
        }
        else if (isAttacking)
        {
            // Stop attacking if player moved out of range or view.
            isAttacking = false;
            playerTransform = null;
        }

        // Lock position if animation is still playing (e.g., bite animation).
        if (enemyAttackBehavior.isAttackAnimationPlaying())
        {
            agent.SetDestination(enemyTransform.position);
        }
        else if (inCooldown)
        {
            enemyAnimatorControl.setAllBooleanParamToFalse();
            agent.SetDestination(enemyTransform.position);
        }
    }

    private void checkCooldown()
    {
        if (enemyAttackBehavior.getIsAttacking())
        {
            inCooldown = true;
            StartCoroutine(cooldown());
        }
    }

    private IEnumerator cooldown()
    {
        yield return new WaitForSeconds(enemyAttackBehavior.getAttackTime());
        inCooldown = false;
    }

    ///enable this to see the detection and chase ranges
    /// <summary>
    /// Draws gizmos in the editor to visualize detection and chase ranges.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activateChassingRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chassingRange);

        if (playerTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}
