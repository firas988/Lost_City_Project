using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles enemy AI movement behavior, including chasing the player,
/// attacking when in range, and returning to patrol mode.
/// Integrates with NavMeshAgent, NPCnavigation, and EnemyAttackBehavior for comprehensive AI.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    #region Component References
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
    #endregion

    #region Movement Configuration
    /// <summary>Distance within which chasing is triggered.</summary>
    [SerializeField]
    private float activateChassingRange = 10f;

    /// <summary>Maximum distance to keep chasing the player before giving up.</summary>
    [SerializeField]
    private float chassingRange = 20f;
    #endregion

    #region Movement State Variables
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
    #endregion

    #region Public Interface Methods
    /// <summary>Sets the canMove state.</summary>
    /// <param name="canMove">Whether the enemy should be able to move.</param>
    public void setCanMove(bool canMove)
    {
        // COMPLEXITY ANALYSIS: setCanMove() - O(1)
        this.canMove = canMove;
    }

    /// <summary>Sets the return-to-patrol state.</summary>
    /// <param name="isReturn">Whether the enemy should return to patrol.</param>
    public void setIsReturn(bool isReturn)
    {
        // COMPLEXITY ANALYSIS: setIsReturn() - O(1)
        this.isReturn = isReturn;
    }

    /// <summary>Returns whether the enemy is chasing the player.</summary>
    /// <returns>True if chasing; otherwise false.</returns>
    public bool getIsChassing()
    {
        // COMPLEXITY ANALYSIS: getIsChassing() - O(1)
        return isChassing;
    }

    /// <summary>Sets the chasing state.</summary>
    /// <param name="isChassing">Whether the enemy should be chasing.</param>
    public void setIsChassing(bool isChassing)
    {
        // COMPLEXITY ANALYSIS: setIsChassing() - O(1)
        this.isChassing = isChassing;
    }

    /// <summary>Returns whether the enemy is attacking.</summary>
    /// <returns>True if attacking; otherwise false.</returns>
    public bool getIsAttacking()
    {
        // COMPLEXITY ANALYSIS: getIsAttacking() - O(1)
        return isAttacking;
    }

    /// <summary>Sets the attacking state.</summary>
    /// <param name="isAttacking">Whether the enemy should be attacking.</param>
    public void setIsAttacking(bool isAttacking)
    {
        // COMPLEXITY ANALYSIS: setIsAttacking() - O(1)
        this.isAttacking = isAttacking;
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references to required components.
    /// </summary>
    private void Awake()
    {
        // COMPLEXITY ANALYSIS: Awake() - O(1)
        // Get NavMeshAgent for movement
        agent = GetComponent<NavMeshAgent>();

        // Get this enemy's transform
        enemyTransform = GetComponent<Transform>();

        // Get reference to NPC patrol movement controller
        npcNavigation = GetComponent<NPCnavigation>();

        // Get reference to attack logic
        enemyAttackBehavior = GetComponent<EnemyAttackBehavior>();

        // Get reference to animator control
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();

        // Warn if no NavMeshAgent is attached
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
        // COMPLEXITY ANALYSIS: Update() - O(1)
        // Exit if required components are missing
        if (agent == null || enemyTransform == null)
            return;

        // Exit if movement is disabled
        if (!canMove)
        {
            agent.SetDestination(enemyTransform.position);
            return;
        }

        // If returning to patrol zone, reset chasing and player tracking
        if (isReturn)
        {
            isChassing = false;
            playerTransform = null;
            StartCoroutine(returnToPatrolZone());
        }

        // If not attacking, allow chasing logic
        if (!isAttacking && !inCooldown)
        {
            npcNavigation.itWalk();
            actvateChassing();

            if (isChassing)
            {
                chassing();
            }
        }

        // If a player is tracked, check for attack opportunity
        if (playerTransform != null)
        {
            activateAttack();
        }

        checkCooldown();
    }
    #endregion

    #region Patrol and Return Logic
    /// <summary>
    /// Coroutine that delays resetting to patrol mode for 10 seconds.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator returnToPatrolZone()
    {
        // COMPLEXITY ANALYSIS: returnToPatrolZone() - O(1)
        // Wait for 10 seconds before allowing wandering again
        yield return new WaitForSeconds(10f);
        npcNavigation.setIsWandering(true);
    }
    #endregion

    #region Chasing Behavior
    /// <summary>
    /// Scans the nearby area for the player to start chasing if in range.
    /// </summary>
    private void actvateChassing()
    {
        // COMPLEXITY ANALYSIS: actvateChassing() - O(c) where c = number of colliders in range
        // Check all colliders in a sphere around the enemy
        Collider[] hits = Physics.OverlapSphere(enemyTransform.position, activateChassingRange);
        foreach (Collider col in hits)
        {
            // If the collider is the player and the enemy is not returning
            if (col.CompareTag("Player") && !isReturn)
            {
                playerTransform = col.transform;

                // Set destination toward player
                agent.SetDestination(playerTransform.position);

                // Start chasing
                isChassing = true;

                // Stop patrolling if active
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
        // COMPLEXITY ANALYSIS: chassing() - O(1)
        if (isChassing)
        {
            npcNavigation.itRun();

            // Check distance to player
            float distance = Vector3.Distance(enemyTransform.position, playerTransform.position);
            if (distance <= chassingRange)
            {
                // Keep chasing the player
                agent.SetDestination(playerTransform.position);
            }
            else
            {
                // Stop chasing and reset player
                isChassing = false;
                playerTransform = null;

                // Go back to original position and resume wandering
                if (!npcNavigation.getIsWandering())
                {
                    agent.SetDestination(enemyTransform.position);
                }

                npcNavigation.setIsWandering(true);
            }
        }
    }
    #endregion

    #region Attack Logic
    /// <summary>
    /// Checks if the player is in attack range and within field of view,
    /// then triggers attack or resets state if out of range.
    /// </summary>
    private void activateAttack()
    {
        // COMPLEXITY ANALYSIS: activateAttack() - O(1)
        // Measure distance to player
        float distance = Vector3.Distance(enemyTransform.position, playerTransform.position);

        // Get direction and angle between enemy and player
        Vector3 directionToPlayer = (playerTransform.position - enemyTransform.position).normalized;
        float angle = Vector3.Angle(enemyTransform.forward, directionToPlayer);
        float visionAngle = 100f; // Field of view angle

        // If within attack range and FOV and not returning, start attacking
        if (
            distance <= enemyAttackBehavior.getAttackRange()
            && angle <= visionAngle
            && !isReturn
            && !inCooldown
        )
        {
            isChassing = false;
            isAttacking = true;

            // Stop movement while attacking
            agent.SetDestination(enemyTransform.position);
        }
        else if (isAttacking)
        {
            // Stop attacking if player moved out of range or view
            isAttacking = false;
            playerTransform = null;
        }

        // Lock position if animation is still playing (e.g., bite animation)
        if (enemyAttackBehavior.isAttackAnimationPlaying())
        {
            agent.SetDestination(enemyTransform.position);
        }
        else if (inCooldown)
        {
            // Reset animation parameters and lock position during cooldown
            enemyAnimatorControl.setAllBooleanParamToFalse();
            agent.SetDestination(enemyTransform.position);
        }
    }
    #endregion

    #region Cooldown Management
    /// <summary>
    /// Checks if the enemy should enter cooldown state after attacking.
    /// </summary>
    private void checkCooldown()
    {
        // COMPLEXITY ANALYSIS: checkCooldown() - O(1)
        if (enemyAttackBehavior.getIsAttacking())
        {
            inCooldown = true;
            StartCoroutine(cooldown());
        }
    }

    /// <summary>
    /// Coroutine that manages the attack cooldown period.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator cooldown()
    {
        // COMPLEXITY ANALYSIS: cooldown() - O(1)
        // Wait for the attack time duration before allowing new attacks
        yield return new WaitForSeconds(enemyAttackBehavior.getAttackTime());
        inCooldown = false;
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the detection and chase ranges
    /// <summary>
    /// Draws gizmos in the editor to visualize detection and chase ranges.
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw red sphere for detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activateChassingRange);

        // Draw green sphere for chase range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chassingRange);

        // Draw blue line to player if one is being tracked
        if (playerTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
    #endregion
}
