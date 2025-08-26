using UnityEngine;

/// <summary>
/// Controls the chasing behavior of the Wolf Boss enemy, managing movement towards the player
/// and animation states during pursuit. Integrates with NavMeshAgent for AI navigation.
/// </summary>
public class WolfBossChasing : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>Reference to the player GameObject for targeting.</summary>
    [SerializeField]
    private GameObject player;

    /// <summary>Reference to the Animator component for controlling chase animations.</summary>
    [SerializeField]
    private Animator animator;

    /// <summary>Flag indicating if the boss can currently move and chase.</summary>
    [SerializeField]
    private bool canMove = true;
    #endregion

    #region Component References
    /// <summary>Reference to the NavMeshAgent for AI navigation towards the player.</summary>
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components and finds the player reference.
    /// </summary>
    void Start()
    {
        // Get required components
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find player reference for targeting
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Called every frame to update chasing behavior and animation states.
    /// </summary>
    void Update()
    {
        // Check if the player is null
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        // Check if chasing conditions are met
        if (player != null && navMeshAgent.enabled && !navMeshAgent.isStopped && canMove)
        {
            // Set destination to player position and enable chase animation
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("IsChasing", true);
        }
        else
        {
            // Disable chase animation when not chasing
            animator.SetBool("IsChasing", false);
        }
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Sets whether the boss can move and chase the player.
    /// </summary>
    /// <param name="canMove">Whether the boss should be able to move.</param>
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
    #endregion
}
