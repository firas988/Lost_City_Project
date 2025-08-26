using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player movement and scene transitions when entering bridge trigger zones
/// </summary>
public class TriggerZone : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Target position for the player to move towards
    /// </summary>
    [SerializeField]
    private Transform PointToMoveTo;

    /// <summary>
    /// Portal GameObject that appears when player is close to destination
    /// </summary>
    [SerializeField]
    private GameObject Portal; //10

    /// <summary>
    /// Magic circle effect that follows the player
    /// </summary>
    [SerializeField]
    private GameObject magicCircle; //30

    /// <summary>
    /// Star effect that appears during movement
    /// </summary>
    [SerializeField]
    private GameObject star; //20

    /// <summary>
    /// Audio source for portal sound effects
    /// </summary>
    [SerializeField]
    private AudioSource audioSourcePortal;

    /// <summary>
    /// Audio source for magic circle sound effects
    /// </summary>
    [SerializeField]
    private AudioSource audioSourceMagicCircle;

    /// <summary>
    /// Audio source for star sound effects
    /// </summary>
    [SerializeField]
    private AudioSource audioSourceStar;
    #endregion

    #region Component References
    /// <summary>
    /// Input listener component for controlling player movement
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Player's animator component for walking animations
    /// </summary>
    private Animator animator;

    /// <summary>
    /// NavMesh agent component for player movement
    /// </summary>
    private NavMeshAgent navMeshAgent;

    /// <summary>
    /// Audio manager for playing sound effects
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Scene handler for managing scene transitions
    /// </summary>
    private SceneHandler sceneHandler;
    #endregion

    #region Game Objects
    /// <summary>
    /// Reference to the player GameObject
    /// </summary>
    private GameObject player;
    #endregion

    #region Configuration
    /// <summary>
    /// Tag for finding the GameManager GameObject
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Tag for identifying the player GameObject
    /// </summary>
    private string playerTag = "Player";
    #endregion

    #region State Variables
    /// <summary>
    /// Whether the player is currently moving to the destination
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Whether a scene is currently loading to prevent multiple loads
    /// </summary>
    private bool isLoading = false;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Find and store references to required components
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();
        player = GameObject.FindGameObjectWithTag(playerTag);
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        sceneHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<SceneHandler>();
    }

    void Update()
    {
        // Handle player movement and effects while moving
        if (isMoving && !navMeshAgent.pathPending)
        {
            // Set walking animation and check progress for effects
            animator.SetBool("isWalking", true);
            checkTheProgressToPlayTheEffect();
            followThePlayer();
        }

        // Check if player has reached destination and load scene
        if (
            isMoving
            && !navMeshAgent.pathPending
            && navMeshAgent.remainingDistance <= 0.1f
            && !isLoading
        )
        {
            // Stop walking animation and all audio sources
            animator.SetBool("isWalking", false);
            audioSourceStar.Stop();
            audioSourceMagicCircle.Stop();
            audioSourcePortal.Stop();

            LoadScene();
        }
    }
    #endregion

    #region Trigger Detection
    /// <summary>
    /// Called when player enters the trigger zone
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isMoving)
        {
            // Get player's animator component
            animator = other.GetComponent<Animator>();
            if (inputListener != null)
            {
                // Disable character controller and input to prevent manual movement
                other.gameObject.GetComponent<CharacterController>().enabled = false;
                inputListener.setCanMove(false);

                // Add NavMesh agent for automatic movement
                other.gameObject.AddComponent<NavMeshAgent>();
                navMeshAgent = other.gameObject.GetComponent<NavMeshAgent>();
                navMeshAgent.agentTypeID = -1372625422;
                navMeshAgent.speed = 1.7f;

                // Start movement on next frame to ensure setup is complete
                StartCoroutine(StartMovingNextFrame());
            }
        }
    }
    #endregion

    #region Movement Control
    /// <summary>
    /// Coroutine to start movement on the next frame
    /// </summary>
    private IEnumerator StartMovingNextFrame()
    {
        yield return new WaitForSeconds(0.1f);
        MoveThePlayerToTheBridge();
    }

    /// <summary>
    /// Initiates player movement towards the bridge destination
    /// </summary>
    private void MoveThePlayerToTheBridge()
    {
        // Set destination and start movement
        navMeshAgent.SetDestination(PointToMoveTo.position);
        isMoving = true;
    }
    #endregion

    #region Effect Management
    /// <summary>
    /// Checks player progress and activates visual/audio effects accordingly
    /// </summary>
    private void checkTheProgressToPlayTheEffect()
    {
        // Activate portal when player is within 8 units of destination
        if (navMeshAgent.remainingDistance <= 8f && !Portal.activeSelf)
        {
            Portal.SetActive(true);
            audioManager.playSFX(audioSourcePortal, "Portal");

            // Complete bridge quest if bridge handler exists
            BridgeHandler bridgeHandler = gameObject.transform.parent.GetComponent<BridgeHandler>();
            if (bridgeHandler != null)
            {
                bridgeHandler.completeQuest();
            }
        }

        // Activate star effect when player is within 20 units
        if (navMeshAgent.remainingDistance <= 20f && !star.activeSelf)
        {
            star.SetActive(true);
            audioManager.playSFX(audioSourceStar, "Star", true);
        }

        // Activate magic circle when player is within 25 units
        if (navMeshAgent.remainingDistance <= 25f && !magicCircle.activeSelf)
        {
            magicCircle.SetActive(true);
            audioManager.playSFX(audioSourceMagicCircle, "MagicCircle");
        }
    }

    /// <summary>
    /// Makes visual effects follow the player's position
    /// </summary>
    private void followThePlayer()
    {
        // Update magic circle position to follow player
        if (magicCircle.activeSelf)
        {
            Vector3 pos = player.transform.position;
            pos.y += 0.5f;
            magicCircle.transform.position = pos;
        }

        // Update star position to follow player
        if (star.activeSelf)
        {
            Vector3 pos = player.transform.position;
            pos.y += 0.5f;
            star.transform.position = pos;
        }
    }
    #endregion

    #region Scene Management
    /// <summary>
    /// Loads the appropriate scene based on current scene index
    /// </summary>
    private void LoadScene()
    {
        if (!isLoading)
        {
            // Stop walking animation and prevent multiple scene loads
            animator.SetBool("isWalking", false);
            isLoading = true;

            // Toggle between scene 2 and 3
            if (SceneManager.GetActiveScene().buildIndex == 2)
                sceneHandler.LoadScene(3);
            else
                sceneHandler.LoadScene(2);
        }
    }
    #endregion
}
