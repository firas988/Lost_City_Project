using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles NPC interaction with the player, including detection, navigation control, and player-facing behavior.
/// Manages NPC state transitions when players enter/exit interaction range.
/// </summary>
public class NPCinteraction : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Layer mask for detecting the player.
    /// </summary>
    [SerializeField]
    private LayerMask playerLayer;

    /// <summary>
    /// Sphere GameObject used for interaction detection.
    /// </summary>
    [SerializeField]
    private GameObject sphere;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the player's transform for positioning calculations.
    /// </summary>
    private Transform playerTransform;

    /// <summary>
    /// Reference to the player GameObject.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Animator component for controlling NPC animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// NPC navigation component for controlling movement behavior.
    /// </summary>
    private NPCnavigation NPCnavigation;

    /// <summary>
    /// NavMeshAgent component for pathfinding and movement.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Flag indicating if the NPC is currently occupied with a player interaction.
    /// </summary>
    private bool isOccupied;

    /// <summary>
    /// Reference to the player's script component.
    /// </summary>
    private playerScript ps;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes NPC components and references on startup.
    /// </summary>
    void Start()
    {
        NPCnavigation = GetComponent<NPCnavigation>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Continuously monitors for player interaction and manages NPC behavior.
    /// </summary>
    void Update()
    {
        float radius = 2f;
        isOccupied = Physics.CheckSphere(sphere.transform.position, radius, playerLayer);

        if (isOccupied)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, playerLayer);

            foreach (Collider hit in hits)
            {
                playerTransform = hit.gameObject.transform;
                player = hit.gameObject;
                ps = hit.GetComponent<playerScript>();

                if (ps != null && ps.getInteractingWith() == null)
                {
                    // Interact with player!
                    ps.setInteractingWith(this.gameObject);
                }
            }

            if (
                ps != null
                && ps.getInteractingWith() != null
                && ps.getInteractingWith().tag == this.gameObject.tag
                && ps.getInteractingWith().GetComponent<StartNpc>().GetNpcsInstance()
                    == this.gameObject.GetComponent<StartNpc>().GetNpcsInstance()
            )
            {
                toggleOffNavigation();

                if (playerTransform != null)
                {
                    Vector3 direction = playerTransform.position - transform.position;
                    direction.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2);
                }
            }
        }
        else
        {
            HandleExit();
        }
    }

    #endregion

    #region Navigation Control

    /// <summary>
    /// Disables NPC navigation and makes the NPC face the player.
    /// </summary>
    public void toggleOffNavigation()
    {
        if (NPCnavigation == null)
            return;

        NPCnavigation.setIsWandering(false);
        playerTransform = player.transform;
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool("isWalking", false);
        isOccupied = true;
    }

    #endregion

    #region Player Interaction Management

    /// <summary>
    /// Handles cleanup when the player exits the NPC's interaction range.
    /// </summary>
    private void HandleExit()
    {
        if (NPCnavigation != null)
            NPCnavigation.setIsWandering(true);

        if (player != null)
        {
            player = null;
        }
        if (playerTransform != null)
        {
            playerTransform = null;
        }

        if (agent != null)
            agent.isStopped = false;

        isOccupied = false;
    }

    /// <summary>
    /// Gets the current player GameObject that the NPC is interacting with.
    /// </summary>
    /// <returns>The player GameObject, or null if no player is interacting.</returns>
    public GameObject getPlayer()
    {
        return player;
    }

    #endregion
}
