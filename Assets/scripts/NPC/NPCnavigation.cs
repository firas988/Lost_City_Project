using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls an NPC that autonomously wanders within a defined radius on the NavMesh,
/// optionally constrained to a specific area type. Includes randomized idle timing.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NPCnavigation : MonoBehaviour
{
    #region Serialized Fields

    [Header("Wandering Settings")]
    [Tooltip("Maximum distance the NPC can move from its current position when wandering.")]
    [SerializeField]
    private float walkRadius;

    [Tooltip("Minimum and maximum wait times (in seconds) between movements.")]
    [SerializeField]
    private Vector2 waitTimeRange;

    [Tooltip(
        "Optional NavMesh area name to restrict movement to specific areas (e.g., 'Walkroads')."
    )]
    [SerializeField]
    private string navMeshAreaName = "";

    [Tooltip("If true, NPC will start wandering automatically on Start.")]
    [SerializeField]
    private bool isWandering = true;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the NavMeshAgent component for pathfinding and movement.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Reference to the Animator component for animation control.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Bitmask used to limit valid areas for movement.
    /// </summary>
    private int areaMask = NavMesh.AllAreas;

    /// <summary>
    /// Indicates whether the NPC is in a waiting state.
    /// </summary>
    [SerializeField]
    private bool isWaiting = false;

    /// <summary>
    /// Time left before NPC reassesses destination.
    /// </summary>
    private float walkTime;

    /// <summary>
    /// Holds data from StartNpc (walk radius, area mask, wait range).
    /// </summary>
    private NPC npcsInstance;

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the wandering state of the NPC.
    /// </summary>
    /// <param name="isWandering">True to enable wandering, false to disable.</param>
    public void setIsWandering(bool isWandering)
    {
        this.isWandering = isWandering;
    }

    /// <summary>
    /// Gets the wandering state of the NPC.
    /// </summary>
    /// <returns>True if the NPC is wandering, false otherwise.</returns>
    public bool getIsWandering()
    {
        return this.isWandering;
    }

    /// <summary>
    /// Sets the walk time of the NPC.
    /// </summary>
    /// <param name="walkTime">The walk time to set.</param>
    public void setWalkTime(float walkTime)
    {
        this.walkTime = walkTime;
    }

    /// <summary>
    /// Sets the NPC to run at maximum speed.
    /// </summary>
    public void itRun()
    {
        agent.speed = npcsInstance.GetMaxSpeed();
    }

    /// <summary>
    /// Sets the NPC to walk at normal speed.
    /// </summary>
    public void itWalk()
    {
        agent.speed = npcsInstance.GetSpeed();
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the NPC's navigation behavior and starts the wander loop if enabled.
    /// </summary>
    private void Start()
    {
        // Fetch NPC data from linked StartNpc component
        npcsInstance = GetComponent<StartNpc>().GetNpcsInstance();

        agent = GetComponent<NavMeshAgent>();
        // Get Animator component reference
        animator = GetComponent<Animator>();

        // Setup areaMask if a specific area is defined
        if (!string.IsNullOrEmpty(navMeshAreaName))
            areaMask = npcsInstance.GetAreaMask();

        // Pull configuration
        walkRadius = npcsInstance.GetWalkRadius();
        waitTimeRange = npcsInstance.GetWaitingTimeRange();

        itWalk();

        // Snap to nearest valid NavMesh position
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, areaMask))
        {
            transform.position = hit.position;
        }
        else
        {
            // NPC not placed on a valid NavMesh area. Destroying object.
            Destroy(gameObject);
            return;
        }

        // Begin wandering if enabled
        if (isWandering)
            TrySetNewDestination();
    }

    /// <summary>
    /// Called every frame. Manages movement completion and idle timing.
    /// </summary>
    private void Update()
    {
        if (!isWandering || isWaiting)
        {
            return;
        }

        walkTime -= Time.deltaTime;

        // Check if destination reached or timed out
        bool hasArrived =
            !agent.pathPending
            && agent.remainingDistance <= agent.stoppingDistance + 0.15f
            && !agent.hasPath;

        if ((hasArrived || walkTime <= 0) && isWandering)
            StartCoroutine(WaitAndMoveAgain());
    }

    #endregion

    #region Navigation Methods

    /// <summary>
    /// Attempts to pick a new random destination and sets it as the agent's goal.
    /// </summary>
    private void TrySetNewDestination()
    {
        if (TrySetRandomDestination(out float newTime))
        {
            walkTime = newTime;
            // Set animation to walking
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Failed to find a valid destination within the NavMesh.
        }
    }

    /// <summary>
    /// Finds a random point within walk radius on the NavMesh and moves the agent there.
    /// </summary>
    /// <param name="newWalkTime">Returns estimated travel time to the new destination.</param>
    /// <returns>True if a valid point was found and set, false otherwise.</returns>
    public bool TrySetRandomDestination(out float newWalkTime)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius + transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, areaMask))
            {
                float distance = Vector3.Distance(transform.position, hit.position);
                newWalkTime = distance / agent.speed;
                agent.SetDestination(hit.position);
                // Draw navigation line to destination
                return true;
            }
        }

        newWalkTime = 0f;
        return false;
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Handles the wait time between movements.
    /// Stops movement, waits randomly, and resumes with a new destination.
    /// </summary>
    private IEnumerator WaitAndMoveAgain()
    {
        agent.ResetPath();
        isWandering = false;
        isWaiting = true;
        animator.SetBool("isWalking", false);
        float waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);

        yield return new WaitForSeconds(waitTime);

        TrySetNewDestination();
        isWaiting = false;
        isWandering = true;
    }

    #endregion
}
