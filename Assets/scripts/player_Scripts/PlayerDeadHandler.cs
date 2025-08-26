using System.Collections;
using UnityEngine;

/// <summary>
/// Manages player death, respawning, and death-related state transitions.
/// Handles death detection, input disabling, death animations, and respawn sequence.
/// Coordinates with UI system for death/respawn visual effects and statistics tracking.
/// </summary>
public class PlayerDeadHandler : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to the Player instance for health status and stats management.
    /// Used to check death state and reset health on respawn.
    /// </summary>
    private Player player;

    /// <summary>
    /// Reference to StatisticsHandler for recording death statistics.
    /// Tracks total player deaths across game sessions.
    /// </summary>
    private StatisticsHandler statisticsHandler;

    /// <summary>
    /// Reference to InputListener for controlling player input during death/respawn.
    /// Disables movement and attack input when player is dead.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to AnimateAttackController for death and spawn animations.
    /// Plays death animation and spawn animation during respawn sequence.
    /// </summary>
    private AnimateAttackController animateAttackController;

    /// <summary>
    /// Reference to UIManager for death/respawn visual effects.
    /// Controls black screen fades during respawn sequence.
    /// </summary>
    private UIManager uiManager;
    #endregion

    #region Death State Management
    /// <summary>
    /// Tracks whether the player is currently in a dead state.
    /// Prevents multiple death processing and controls respawn logic.
    /// </summary>
    private bool isDead = false;

    /// <summary>
    /// Tracks whether the respawn sequence is currently in progress.
    /// Prevents multiple respawn attempts and controls respawn flow.
    /// </summary>
    private bool isSpawned = false;
    #endregion

    #region Spawn Point Management
    /// <summary>
    /// GameObject representing the player's respawn location.
    /// Used to position the player after death and respawn.
    /// </summary>
    private GameObject playerSpawnPoint;

    /// <summary>
    /// Tag used to identify the player spawn point in the scene.
    /// </summary>
    private string playerSpawnPointTag = "Respawn";
    #endregion

    #region System References
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the death handler by finding required component references.
    /// Sets up connections to player, statistics, input, animation, and UI systems.
    /// </summary>
    void Start()
    {
        // Get player instance for health and stats management
        player = GetComponent<StartPlayer>().getPlayer();

        // Get statistics handler for death tracking
        statisticsHandler = GetComponentInChildren<StatisticsHandler>();

        // Find input listener for input control during death
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();

        // Get animation controller for death/spawn animations
        animateAttackController = GetComponent<AnimateAttackController>();

        // Find UI manager for death/respawn visual effects
        uiManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();

        // Find player spawn point for respawn positioning
        playerSpawnPoint = GameObject.FindGameObjectWithTag(playerSpawnPointTag);
    }

    /// <summary>
    /// Updates death state and manages respawn sequence each frame.
    /// Monitors player death status and initiates respawn when appropriate.
    /// </summary>
    void Update()
    {
        // Ensure spawn point reference exists
        if (playerSpawnPoint == null)
        {
            playerSpawnPoint = GameObject.FindGameObjectWithTag(playerSpawnPointTag);
        }

        // Check for death state changes
        checkDeath();

        // Start respawn sequence if player is dead and not already respawning
        if (player.isDead() && !isSpawned && isDead)
        {
            isSpawned = true;
            StartCoroutine(spawnPlayer());
        }
    }
    #endregion

    #region Respawn System
    /// <summary>
    /// Coroutine that handles the complete player respawn sequence.
    /// Manages timing, visual effects, health reset, and input restoration.
    /// </summary>
    private IEnumerator spawnPlayer()
    {
        // Disable character controller during respawn sequence
        GetComponent<CharacterController>().enabled = false;

        // Wait before starting respawn sequence
        yield return new WaitForSeconds(3f);

        // Fade to black screen for respawn effect
        uiManager.startFadeInBlackScreen(1f);
        yield return new WaitForSeconds(3f);

        // Reset player health and play spawn animation
        player.resetHealth();
        animateAttackController.spawnAnimation();

        // Move player to spawn point
        transform.position = playerSpawnPoint.transform.position;
        yield return new WaitForSeconds(3f);

        // Fade out black screen to reveal respawned player
        uiManager.startFadeOutBlackScreen(0f);
        yield return new WaitForSeconds(1f);

        // Re-enable player input and movement
        inputListener.setCanAttack(true);
        inputListener.setCanMove(true);

        // Reset respawn state and re-enable character controller
        isSpawned = false;
        GetComponent<CharacterController>().enabled = true;
    }
    #endregion

    #region Death Detection
    /// <summary>
    /// Disables input and plays death animation if the player is dead.
    /// Manages death state transitions and prevents multiple death processing.
    /// </summary>
    public void checkDeath()
    {
        // Handle initial death detection
        if (player.isDead() && !isDead)
        {
            // Disable player input during death
            inputListener.setCanAttack(false);
            inputListener.setCanMove(false);

            // Play death animation and record death statistics
            animateAttackController.DeathAnimation();
            statisticsHandler.Death();

            // Mark player as dead to prevent multiple processing
            isDead = true;
        }
        // Handle recovery from death state
        else if (!player.isDead() && isDead)
        {
            isDead = false;
        }
    }
    #endregion
}
