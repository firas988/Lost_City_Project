using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

// Require necessary components to ensure they're attached to the GameObject
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AnimateController))]
public class PlayerController : MonoBehaviour
{
    // ===== COMPONENT REFERENCES =====
    [Header("Component References")]
    private CharacterController characterController; // Reference to CharacterController component
    private AnimateController animateController; // Reference to AnimateController script
    private InputListener inputListener; // Reference to InputListener script
    private StatisticsHandler statisticsHandler; // Reference to StatisticsHandler script
    private Player player;
    private AudioSource audioSource;
    private AudioManager audioManager;

    [SerializeField]
    private new Transform camera; // Reference to camera for directional movement

    // ===== MOVEMENT SETTINGS =====
    [Header("Movement Settings")]
    [SerializeField]
    private float walkSpeed = 1.5f; // Speed while walking

    private float walkSpeedBase = 1.5f;

    [SerializeField]
    private float sprintSpeed = 6f; // Speed while sprinting

    private float sprintSpeedBase = 6f;

    [SerializeField]
    private float sprintTransitSpeed = 2.5f; // How quickly we switch between walk/sprint speeds

    [SerializeField]
    private float turningSpeed = 100f; // Rotation smoothing speed

    [SerializeField]
    private float gravity = 20f; // Downward gravity force

    [SerializeField]
    private float jumpHeight = 1.5f; // Max jump height

    private bool wasStopping = true; // Tracks if the player was stopping last frame

    // ===== INPUT SETTINGS =====
    [Header("Input Settings")]
    private float moveInput; // Forward/backward input (W/S)
    private float turnInput; // Left/right input (A/D)

    // ===== STATE VARIABLES =====
    private float verticalVelocity; // Tracks current Y velocity
    private float currentSpeed; // Current speed, smoothed between walking and sprinting

    private string gameManagerTag = "GameManager";

    [Header("Audio Settings")]
    [SerializeField]
    private List<string> footStepSounds;

    [SerializeField]
    private string landSound;

    private int footStepSoundIndex = 0;

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
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        player = GetComponent<StartPlayer>().getPlayer();
    }

    void Update()
    {
        // Gather input and process movement every frame
        InputMangagement();
        Movement();
    }

    public void updateSpeed()
    {
        walkSpeed = walkSpeedBase + player.getCurrentSpeed();
        sprintSpeed = sprintSpeedBase + player.getCurrentSpeed();
    }

    private void Movement()
    {
        // Handles horizontal/vertical movement and facing direction
        GroundMovement();

        CameraRotation();
    }

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
                float turnSpeed = wasStopping ? turningSpeed / 10 : turningSpeed;
                wasStopping = false;
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

    private float VerticalForceCalculation()
    {
        if (characterController.isGrounded)
        {
            // Keep grounded with slight downward force
            verticalVelocity = -1f;

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

    public void stopMovement()
    {
        inputListener.setCanMove(false);
    }

    private void InputMangagement()
    {
        // Read forward/backward input (W/S)
        moveInput = inputListener.vertical();

        // Read left/right input (A/D)
        turnInput = inputListener.horizontal();
    }

    public void PlayFootStepSound()
    {
        audioManager.playSFX(audioSource, footStepSounds[footStepSoundIndex]);
        footStepSoundIndex++;
        if (footStepSoundIndex >= footStepSounds.Count)
        {
            footStepSoundIndex = 0;
        }
    }

    public void PlayLandSound()
    {
        audioManager.playSFX(audioSource, landSound);
    }

    public void stopCameraRotation()
    {
        camera.transform.parent.GetComponentInChildren<CinemachineOrbitalFollow>().enabled = false;
    }

    public void startCameraRotation()
    {
        camera.transform.parent.GetComponentInChildren<CinemachineOrbitalFollow>().enabled = true;
    }
}
