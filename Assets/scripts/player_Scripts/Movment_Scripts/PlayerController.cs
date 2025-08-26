using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Manages player movement, camera control, and audio feedback for the character controller.
/// Handles walking, sprinting, jumping, gravity, and camera rotation with smooth transitions.
/// Integrates with input system, animation controller, and audio manager for comprehensive movement.
/// </summary>
// Require necessary components to ensure they're attached to the GameObject
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AnimateController))]
public class PlayerController : MonoBehaviour
{
    #region Component References
    [Header("Component References")]
    /// <summary>
    /// Reference to CharacterController component for physics-based movement.
    /// Handles collision detection and ground checking.
    /// </summary>
    private CharacterController characterController;

    /// <summary>
    /// Reference to AnimateController script for movement animations.
    /// Controls walking, running, and jumping animations.
    /// </summary>
    private AnimateController animateController;

    /// <summary>
    /// Reference to InputListener script for player input handling.
    /// Provides movement, sprint, and jump input values.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to StatisticsHandler script for tracking player actions.
    /// Records jumping and other movement statistics.
    /// </summary>
    private StatisticsHandler statisticsHandler;

    /// <summary>
    /// Player instance containing stats and speed modifiers.
    /// Used for applying speed bonuses from items and effects.
    /// </summary>
    private Player player;

    /// <summary>
    /// AudioSource component for playing movement sound effects.
    /// Handles footstep and landing sound playback.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Reference to AudioManager for sound effect coordination.
    /// Manages SFX playback and audio system integration.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to camera for directional movement calculations.
    /// Used to determine movement direction relative to camera orientation.
    /// </summary>
    [SerializeField]
    private new Transform camera;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    /// <summary>
    /// Speed while walking in units per second.
    /// Base walking speed before any modifiers.
    /// </summary>
    [SerializeField]
    private float walkSpeed = 1.5f;

    /// <summary>
    /// Base walking speed value for resetting after modifiers.
    /// Stored separately from current walkSpeed for restoration.
    /// </summary>
    private float walkSpeedBase = 1.5f;

    /// <summary>
    /// Speed while sprinting in units per second.
    /// Base sprinting speed before any modifiers.
    /// </summary>
    [SerializeField]
    private float sprintSpeed = 6f;

    /// <summary>
    /// Base sprinting speed value for resetting after modifiers.
    /// Stored separately from current sprintSpeed for restoration.
    /// </summary>
    private float sprintSpeedBase = 6f;

    /// <summary>
    /// How quickly we switch between walk/sprint speeds.
    /// Controls the smoothness of speed transitions.
    /// </summary>
    [SerializeField]
    private float sprintTransitSpeed = 2.5f;

    /// <summary>
    /// Rotation smoothing speed for character turning.
    /// Higher values result in faster, more responsive turning.
    /// </summary>
    [SerializeField]
    private float turningSpeed = 100f;

    /// <summary>
    /// Downward gravity force applied to the player.
    /// Controls how quickly the player falls when airborne.
    /// </summary>
    [SerializeField]
    private float gravity = 20f;

    /// <summary>
    /// Maximum jump height in world units.
    /// Used to calculate initial jump velocity.
    /// </summary>
    [SerializeField]
    private float jumpHeight = 1.5f;

    /// <summary>
    /// Tracks if the player was stopping last frame.
    /// Used to adjust turning speed for smoother rotation.
    /// </summary>
    private bool wasStopping = true;
    #endregion

    #region Input Settings
    [Header("Input Settings")]
    /// <summary>
    /// Forward/backward input value (W/S keys).
    /// Positive for forward, negative for backward.
    /// </summary>
    private float moveInput;

    /// <summary>
    /// Left/right input value (A/D keys).
    /// Positive for right, negative for left.
    /// </summary>
    private float turnInput;
    #endregion

    #region State Variables
    /// <summary>
    /// Tracks current Y velocity for gravity and jumping.
    /// Positive when jumping up, negative when falling down.
    /// </summary>
    private float verticalVelocity;

    /// <summary>
    /// Current movement speed, smoothed between walking and sprinting.
    /// Continuously updated based on input and sprint state.
    /// </summary>
    private float currentSpeed;
    #endregion

    #region System References
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Audio Settings
    [Header("Audio Settings")]
    /// <summary>
    /// List of footstep sound effect names for variety.
    /// Cycles through different sounds for natural movement audio.
    /// </summary>
    [SerializeField]
    private List<string> footStepSounds;

    /// <summary>
    /// Sound effect name for landing after a jump.
    /// Played when the player touches the ground.
    /// </summary>
    [SerializeField]
    private string landSound;

    /// <summary>
    /// Current index in the footstep sounds array.
    /// Cycles through sounds to prevent repetition.
    /// </summary>
    private int footStepSoundIndex = 0;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by setting up references and locking the cursor.
    /// Grabs required components and sets up the movement system.
    /// </summary>
    void Start()
    {
        // Lock cursor for gameplay immersion
        Cursor.lockState = CursorLockMode.Locked;

        // Grab required components at runtime
        characterController = GetComponent<CharacterController>();
        animateController = GetComponent<AnimateController>();
        inputListener = FindAnyObjectByType<InputListener>();
        statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
        audioSource = GetComponent<AudioSource>();

        // Find audio manager for sound effect coordination
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();

        // Get player instance for stats and modifiers
        player = GetComponent<StartPlayer>().getPlayer();
    }

    /// <summary>
    /// Updates player movement, input processing, and speed calculations each frame.
    /// Handles the main movement loop and input management.
    /// </summary>
    void Update()
    {
        // Gather input and process movement every frame
        InputMangagement();
        Movement();
        updateSpeed();
    }
    #endregion

    #region Speed Management
    /// <summary>
    /// Updates movement speeds based on player's current speed modifiers.
    /// Applies speed bonuses from items, potions, or other effects.
    /// </summary>
    public void updateSpeed()
    {
        // Apply speed modifiers to base movement speeds
        walkSpeed = walkSpeedBase + player.getCurrentSpeed();
        sprintSpeed = sprintSpeedBase + player.getCurrentSpeed();
    }
    #endregion

    #region Movement System
    /// <summary>
    /// Handles horizontal/vertical movement and facing direction.
    /// Coordinates ground movement and camera rotation.
    /// </summary>
    private void Movement()
    {
        // Handle ground-based movement and physics
        GroundMovement();

        // Handle character rotation toward camera direction
        CameraRotation();
    }

    /// <summary>
    /// Handles ground-based movement including walking, sprinting, and jumping.
    /// Builds movement vectors and applies physics calculations.
    /// </summary>
    private void GroundMovement()
    {
        // Build a move vector using player input
        Vector3 move = new Vector3(turnInput, 0, moveInput);

        // Disable horizontal movement if walking backwards (S key is pressed)
        if (inputListener.isPressingBackward())
        {
            move.x = 0;
        }

        // Convert local movement direction to world space
        move = transform.TransformDirection(move);

        // Check whether the player is airborne or jumping
        bool inAir = !characterController.isGrounded || verticalVelocity > 0;
        bool isPressingLeftRight =
            inputListener.isPressingLeft() || inputListener.isPressingRight();

        // Handle sprint logic if holding shift and not moving backward or airborne
        if (
            inputListener.isSprinting()
            && !inputListener.isPressingBackward()
            && (!inAir || !isPressingLeftRight)
        )
        {
            // Smoothly ramp up to sprint speed
            currentSpeed = Mathf.Lerp(
                currentSpeed,
                sprintSpeed,
                Time.deltaTime * sprintTransitSpeed
            );
        }
        else
        {
            // Smoothly return to walking speed
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * sprintTransitSpeed);
        }

        // Scale movement vector by the current speed
        move *= currentSpeed;

        // Apply gravity and jumping
        move.y = VerticalForceCalculation();

        // Smooth stop when no input is provided (horizontal only)
        if (Mathf.Abs(move.x) < 0.01f && Mathf.Abs(move.z) < 0.01f)
        {
            move.x = Mathf.Lerp(move.x, 0, Time.deltaTime * 5f);
            move.z = Mathf.Lerp(move.z, 0, Time.deltaTime * 5f);
        }

        // Move the character in the world
        if (characterController.enabled)
        {
            characterController.Move(move * Time.deltaTime);
        }
    }

    /// <summary>
    /// Rotates character toward camera direction if there's movement.
    /// Provides smooth turning with adjustable speed based on movement state.
    /// </summary>
    private void CameraRotation()
    {
        // Rotate character toward camera direction if there's movement
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0)
        {
            Vector3 lookDirection = camera.forward;
            lookDirection.y = 0f; // Flatten the look direction

            // Check if there's meaningful direction input
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                // Adjust turning speed based on whether player was stopping
                float turnSpeed = wasStopping ? turningSpeed / 10 : turningSpeed;
                wasStopping = false;

                // Calculate target rotation and smoothly interpolate
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * turnSpeed
                );
            }
        }
        else
        {
            wasStopping = true;
        }
    }
    #endregion

    #region Physics and Jumping
    /// <summary>
    /// Calculates vertical forces including gravity and jumping.
    /// Handles ground detection and jump velocity calculations.
    /// </summary>
    /// <returns>The calculated vertical velocity for this frame.</returns>
    private float VerticalForceCalculation()
    {
        if (characterController.isGrounded)
        {
            // Keep grounded with slight downward force
            verticalVelocity = -1f;

            // Handle jumping input and animation
            if (inputListener.isJumping() && animateController.isReadyToJump())
            {
                // Calculate initial velocity needed to reach jumpHeight
                verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
                animateController.HandleJump();
                statisticsHandler.Jumping();
            }
        }
        else
        {
            // Apply gravity while airborne
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }
    #endregion

    #region Movement Control
    /// <summary>
    /// Stops player movement by disabling input.
    /// Used for cutscenes or other game states that require movement restriction.
    /// </summary>
    public void stopMovement()
    {
        inputListener.setCanMove(false);
    }

    /// <summary>
    /// Manages input reading for movement and turning.
    /// Updates input values each frame for movement calculations.
    /// </summary>
    private void InputMangagement()
    {
        // Read forward/backward input (W/S)
        moveInput = inputListener.vertical();

        // Read left/right input (A/D)
        turnInput = inputListener.horizontal();
    }
    #endregion

    #region Audio Management
    /// <summary>
    /// Plays footstep sound effect and cycles through available sounds.
    /// Called by animation events during movement.
    /// </summary>
    public void PlayFootStepSound()
    {
        // Play current footstep sound and advance to next
        audioManager.playSFX(audioSource, footStepSounds[footStepSoundIndex]);
        footStepSoundIndex++;

        // Loop back to first sound when reaching the end
        if (footStepSoundIndex >= footStepSounds.Count)
        {
            footStepSoundIndex = 0;
        }
    }

    /// <summary>
    /// Plays landing sound effect when player touches the ground.
    /// Called by animation events or physics detection.
    /// </summary>
    public void PlayLandSound()
    {
        audioManager.playSFX(audioSource, landSound);
    }
    #endregion

    #region Camera Control
    /// <summary>
    /// Stops camera rotation by disabling Cinemachine orbital follow.
    /// Used for cutscenes or other situations requiring camera control.
    /// </summary>
    public void stopCameraRotation()
    {
        camera.transform.parent.GetComponentInChildren<CinemachineOrbitalFollow>().enabled = false;
    }

    /// <summary>
    /// Starts camera rotation by enabling Cinemachine orbital follow.
    /// Restores normal camera behavior after being stopped.
    /// </summary>
    public void startCameraRotation()
    {
        camera.transform.parent.GetComponentInChildren<CinemachineOrbitalFollow>().enabled = true;
    }
    #endregion
}
