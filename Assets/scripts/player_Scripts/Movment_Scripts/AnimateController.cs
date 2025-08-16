using System;
using System.Collections;
using UnityEngine;

// Require necessary components to ensure they're attached to the GameObject
[RequireComponent(typeof(Animator))]
public class AnimateController : MonoBehaviour
{
    private InputListener inputListener;

    // ===== GROUND CHECK SETTINGS =====
    [Header("Ground Check Settings")]
    public LayerMask groundLayer; // Layer to determine what counts as "ground"

    [SerializeField]
    private float groundCheckRadius = 0.3f; // Radius for ground sphere check

    [SerializeField]
    private Vector3 groundCheckOffset = new Vector3(0, -0.1f, 0); // Offset from player position for ground check

    // ===== MOVEMENT SETTINGS =====
    [Header("Movement Settings")]
    [SerializeField]
    private float walkMaxSpeed = 0.5f; // Max walking speed

    [SerializeField]
    private float runMaxSpeed = 2.0f; // Max running speed

    [SerializeField]
    private float acceleration = 6.0f; // Speed increase per second

    [SerializeField]
    private float deceleration = 6.5f; // Speed decrease per second

    // ===== COMPONENT REFERENCES =====
    private Animator playerAnimator; // Reference to the Animator
    private Coroutine activeTimer;

    // ===== JUMPING SETTINGS =====
    private bool isGrounded = false; // Whether player is on the ground
    private bool isJumping = false; // Whether player is currently jumping
    private bool readyToJump = true; // Whether player is ready to jump
    private bool isTimeingtoFreeFall = false; // Whether player is timing to free fall
    private bool isFreeFall = false; // Whether player is currently free falling

    private bool inTimer = false;

    private Coroutine activeTimerForIdle;

    void Start()
    {
        inputListener = FindAnyObjectByType<InputListener>();
        // Grab the Animator component from the same GameObject
        playerAnimator = GetComponent<Animator>();

        // // Set initial jumping state from Animator
        // isJumping = playerAnimator.GetBool("Jump");
    }

    void Update()
    {
        UpdateGroundStatus(); // Continuously check if the player is grounded
        HandleMovement(); // Handle movement and jump input
    }

    // Check if player is touching ground using sphere collision detection
    private void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(
            transform.position + groundCheckOffset, // Position for the sphere
            groundCheckRadius, // Radius of the sphere
            groundLayer, // What counts as ground
            QueryTriggerInteraction.Ignore // Ignore triggers during check
        );
    }

    // Handle both directional movement and jumping logic
    private void HandleMovement()
    {
        HandleHorizontalMovement(); // WASD + Shift logic
        HandleFreeFallandGrounded();
    }

    // Controls animator parameters for horizontal movement based on input
    private void HandleHorizontalMovement()
    {
        // Get current values from Animator
        float currentVelocityX = playerAnimator.GetFloat("VelocityX");
        float currentVelocityY = playerAnimator.GetFloat("VelocityY");

        // Get movement input
        bool isMovingForward = inputListener.isPressingForward();
        bool isMovingBackward = inputListener.isPressingBackward();
        bool isMovingLeft = inputListener.isPressingLeft();
        bool isMovingRight = inputListener.isPressingRight();
        bool isRunning = inputListener.isSprinting();

        // Determine movement speed cap based on running or walking
        float targetMaxSpeed = isRunning ? runMaxSpeed : walkMaxSpeed;

        // Forward and backward acceleration
        if (isMovingForward && currentVelocityY < targetMaxSpeed)
        {
            currentVelocityY += Time.deltaTime * acceleration;
        }
        else if (isMovingBackward && currentVelocityY > -walkMaxSpeed)
        {
            currentVelocityY -= Time.deltaTime * acceleration;
        }

        // Left and right acceleration
        if (isMovingLeft && currentVelocityX > -targetMaxSpeed && !isMovingBackward)
        {
            currentVelocityX -= Time.deltaTime * acceleration;
        }
        else if (isMovingLeft && currentVelocityX < -targetMaxSpeed && !isMovingBackward)
        {
            currentVelocityX += Time.deltaTime * deceleration;
            if (currentVelocityX > -targetMaxSpeed - 0.05f)
            {
                currentVelocityX = -targetMaxSpeed;
            }
        }
        else if (isMovingRight && currentVelocityX < targetMaxSpeed && !isMovingBackward)
        {
            currentVelocityX += Time.deltaTime * acceleration;
        }
        else if (isMovingRight && currentVelocityX > targetMaxSpeed && !isMovingBackward)
        {
            currentVelocityX -= Time.deltaTime * deceleration;
            if (currentVelocityX < targetMaxSpeed + 0.05f)
            {
                currentVelocityX = targetMaxSpeed;
            }
        }

        // Deceleration when no input is pressed
        if (!isMovingForward && currentVelocityY > 0.0f)
        {
            currentVelocityY -= Time.deltaTime * deceleration;
        }
        if (!isMovingBackward && currentVelocityY < 0.0f)
        {
            currentVelocityY += Time.deltaTime * deceleration;
        }
        if (!isMovingLeft && currentVelocityX < 0.0f)
        {
            currentVelocityX += Time.deltaTime * deceleration;
        }
        if (!isMovingRight && currentVelocityX > 0.0f)
        {
            currentVelocityX -= Time.deltaTime * deceleration;
        }

        // Snap to zero if speed is too low (to prevent jittering)
        if (!isMovingLeft && !isMovingRight && Mathf.Abs(currentVelocityX) < 0.05f)
        {
            currentVelocityX = 0.0f;
        }

        // Cap forward running speed
        if (isMovingForward && isRunning && currentVelocityY > targetMaxSpeed)
        {
            currentVelocityY = targetMaxSpeed;
        }
        // Decelerate to targetMaxSpeed if over it slightly
        else if (isMovingForward && currentVelocityY > targetMaxSpeed)
        {
            currentVelocityY -= Time.deltaTime * deceleration;
            if (currentVelocityY < targetMaxSpeed + 0.05f)
            {
                currentVelocityY = targetMaxSpeed;
            }
        }
        // Snap to exact max speed when close enough
        else if (
            isMovingForward
            && currentVelocityY < targetMaxSpeed
            && currentVelocityY > targetMaxSpeed - 0.05f
        )
        {
            currentVelocityY = targetMaxSpeed;
        }
        if (!isMovingForward && !isMovingBackward && Mathf.Abs(currentVelocityY) < 0.09f)
        {
            currentVelocityY = 0.0f;
        }

        // Apply updated movement values to Animator
        playerAnimator.SetFloat("VelocityX", currentVelocityX);
        playerAnimator.SetFloat("VelocityY", currentVelocityY);

        if (!isMovingForward && !isMovingBackward && !isMovingLeft && !isMovingRight && !inTimer)
        {
            inTimer = true;
            activeTimerForIdle = StartCoroutine(TimerForIdle(0.7f));
        }
        else if ((isMovingForward || isMovingBackward || isMovingLeft || isMovingRight) && inTimer)
        {
            stopTimer();
            inTimer = false;
        }
    }

    IEnumerator TimerForIdle(float time)
    {
        yield return new WaitForSeconds(time);
        playerAnimator.SetFloat("VelocityX", 0);
        playerAnimator.SetFloat("VelocityY", 0);
    }

    private void stopTimer()
    {
        if (activeTimerForIdle != null)
        {
            StopCoroutine(activeTimerForIdle);
        }
    }

    private void jumping()
    {
        readyToJump = false;
    }

    private void setReadyToJump()
    {
        inputListener.setCanMove(true);
        readyToJump = true;
    }

    public bool isReadyToJump()
    {
        return readyToJump;
    }

    // Handles all jumping and airborne transitions
    public void HandleJump()
    {
        if (isGrounded && readyToJump)
        {
            playerAnimator.SetTrigger("Jump");
            isJumping = true;
        }
    }

    private void HandleFreeFallandGrounded()
    {
        if ((isJumping || isFreeFall) && isGrounded)
        {
            StopTimer();
            isJumping = false;
            isFreeFall = false;
            playerAnimator.SetTrigger("Grounded");
        }
        if (!isTimeingtoFreeFall && !isGrounded && !isFreeFall)
        {
            isTimeingtoFreeFall = true;
            activeTimer = StartCoroutine(TimerForFreeFall(1.5f));
        }
    }

    private void StopTimer()
    {
        if (activeTimer != null)
        {
            StopCoroutine(activeTimer);
        }
    }

    IEnumerator TimerForFreeFall(float time)
    {
        yield return new WaitForSeconds(time);

        if (!isGrounded && !isFreeFall)
        {
            playerAnimator.SetTrigger("FreeFall");
            isFreeFall = true;
        }
        isTimeingtoFreeFall = false;
    }

    public void stopPlayerAnimation()
    {
        StartCoroutine(TimerForIdle(0f));
    }

    // Draw gizmo in the editor to visualize ground check sphere
    private void OnDrawGizmosSelected()
    {
        Vector3 checkPosition = transform.position + groundCheckOffset;
        Gizmos.color = isGrounded ? Color.red : Color.green;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}
