using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls player movement animations and ground detection for smooth character movement.
/// Manages walking, running, jumping, and free-fall animations with physics-based ground checking.
/// Provides smooth acceleration and deceleration for natural movement feel.
/// </summary>
// Require necessary components to ensure they're attached to the GameObject
[RequireComponent(typeof(Animator))]
public class AnimateController : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to InputListener for reading player input values.
    /// Provides movement direction and sprint state information.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to the Animator component for controlling animation parameters.
    /// Manages all movement and state-based animations.
    /// </summary>
    private Animator playerAnimator;
    #endregion

    #region Ground Check Settings
    [Header("Ground Check Settings")]
    /// <summary>
    /// Layer mask to determine what counts as "ground" for the player.
    /// Used in physics sphere checks for ground detection.
    /// </summary>
    public LayerMask groundLayer;

    /// <summary>
    /// Radius for the ground sphere check in world units.
    /// Larger values provide more forgiving ground detection.
    /// </summary>
    [SerializeField]
    private float groundCheckRadius = 0.3f;

    /// <summary>
    /// Offset from player position for ground check sphere.
    /// Positioned below the player for accurate ground detection.
    /// </summary>
    [SerializeField]
    private Vector3 groundCheckOffset = new Vector3(0, -0.1f, 0);
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    /// <summary>
    /// Maximum walking speed for forward/backward movement.
    /// Controls the speed cap when not sprinting.
    /// </summary>
    [SerializeField]
    private float walkMaxSpeed = 0.5f;

    /// <summary>
    /// Maximum running speed for forward/backward movement.
    /// Controls the speed cap when sprinting.
    /// </summary>
    [SerializeField]
    private float runMaxSpeed = 2.0f;

    /// <summary>
    /// Speed increase per second for acceleration.
    /// Higher values result in faster response to input.
    /// </summary>
    [SerializeField]
    private float acceleration = 6.0f;

    /// <summary>
    /// Speed decrease per second for deceleration.
    /// Higher values result in faster stopping.
    /// </summary>
    [SerializeField]
    private float deceleration = 6.5f;
    #endregion

    #region Jumping State Variables
    /// <summary>
    /// Whether the player is currently touching the ground.
    /// Updated each frame through physics sphere checks.
    /// </summary>
    private bool isGrounded = false;

    /// <summary>
    /// Whether the player is currently in a jumping state.
    /// Set when jump input is received and cleared on landing.
    /// </summary>
    private bool isJumping = false;

    /// <summary>
    /// Whether the player is ready to perform another jump.
    /// Prevents multiple jumps while airborne.
    /// </summary>
    private bool readyToJump = true;

    /// <summary>
    /// Whether the player is timing to enter free-fall state.
    /// Used to delay free-fall animation after leaving ground.
    /// </summary>
    private bool isTimeingtoFreeFall = false;

    /// <summary>
    /// Whether the player is currently in free-fall state.
    /// Set after a delay when airborne without jumping.
    /// </summary>
    private bool isFreeFall = false;
    #endregion

    #region Timer Management
    /// <summary>
    /// Whether the idle timer is currently active.
    /// Used to prevent multiple idle timers from starting.
    /// </summary>
    private bool inTimer = false;

    /// <summary>
    /// Coroutine reference for the idle animation timer.
    /// Used to stop the timer when movement resumes.
    /// </summary>
    private Coroutine activeTimerForIdle;

    /// <summary>
    /// Coroutine reference for the free-fall timer.
    /// Used to stop the timer when landing.
    /// </summary>
    private Coroutine activeTimer;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by finding required references.
    /// Sets up input listener and animator component connections.
    /// </summary>
    void Start()
    {
        // Find input listener for movement input
        inputListener = FindAnyObjectByType<InputListener>();

        // Get the Animator component for animation control
        playerAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates ground status and handles movement each frame.
    /// Continuously monitors player state and updates animations.
    /// </summary>
    void Update()
    {
        // Continuously check if the player is grounded
        UpdateGroundStatus();

        // Handle movement and jump input processing
        HandleMovement();
    }
    #endregion

    #region Ground Detection
    /// <summary>
    /// Checks if player is touching ground using sphere collision detection.
    /// Updates the grounded state for jump and fall logic.
    /// </summary>
    private void UpdateGroundStatus()
    {
        // Use physics sphere check to determine ground contact
        isGrounded = Physics.CheckSphere(
            transform.position + groundCheckOffset, // Position for the sphere
            groundCheckRadius, // Radius of the sphere
            groundLayer, // What counts as ground
            QueryTriggerInteraction.Ignore // Ignore triggers during check
        );
    }
    #endregion

    #region Movement Handling
    /// <summary>
    /// Handles both directional movement and jumping logic.
    /// Coordinates horizontal movement and airborne state management.
    /// </summary>
    private void HandleMovement()
    {
        // Handle WASD + Shift movement logic
        HandleHorizontalMovement();

        // Handle free-fall and grounded state transitions
        HandleFreeFallandGrounded();
    }

    /// <summary>
    /// Controls animator parameters for horizontal movement based on input.
    /// Provides smooth acceleration and deceleration for natural movement.
    /// </summary>
    private void HandleHorizontalMovement()
    {
        // Get current velocity values from Animator
        float currentVelocityX = playerAnimator.GetFloat("VelocityX");
        float currentVelocityY = playerAnimator.GetFloat("VelocityY");

        // Get movement input states
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

        // Left and right acceleration (only when not moving backward)
        if (isMovingLeft && currentVelocityX > -targetMaxSpeed && !isMovingBackward)
        {
            currentVelocityX -= Time.deltaTime * acceleration;
        }
        else if (isMovingLeft && currentVelocityX < -targetMaxSpeed && !isMovingBackward)
        {
            // Decelerate to target speed if over limit
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
            // Decelerate to target speed if over limit
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
        if (!isMovingLeft && !isMovingRight && Mathf.Abs(currentVelocityX) < 0.23f)
        {
            currentVelocityX = 0.0f;
        }

        // Cap forward running speed and handle deceleration
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

        // Snap forward/backward velocity to zero when very low
        if (!isMovingForward && !isMovingBackward && Mathf.Abs(currentVelocityY) < 0.09f)
        {
            currentVelocityY = 0.0f;
        }

        // Apply updated movement values to Animator
        playerAnimator.SetFloat("VelocityX", currentVelocityX);
        playerAnimator.SetFloat("VelocityY", currentVelocityY);

        // Handle idle animation timer
        if (!isMovingForward && !isMovingBackward && !isMovingLeft && !isMovingRight && !inTimer)
        {
            inTimer = true;
            activeTimerForIdle = StartCoroutine(TimerForIdle(0.1f));
        }
        else if ((isMovingForward || isMovingBackward || isMovingLeft || isMovingRight) && inTimer)
        {
            stopTimer();
            playerAnimator.SetFloat("VelocityX", 0);
            playerAnimator.SetFloat("VelocityY", 0);
            inTimer = false;
        }
    }
    #endregion

    #region Timer Management
    /// <summary>
    /// Coroutine that sets velocity to zero after a delay for idle animation.
    /// Provides smooth transition to idle state when movement stops.
    /// </summary>
    /// <param name="time">Delay before setting velocity to zero.</param>
    IEnumerator TimerForIdle(float time)
    {
        yield return new WaitForSeconds(time);
        playerAnimator.SetFloat("VelocityX", 0);
        playerAnimator.SetFloat("VelocityY", 0);
    }

    /// <summary>
    /// Stops the idle timer coroutine if it's active.
    /// Called when movement resumes to prevent idle state.
    /// </summary>
    private void stopTimer()
    {
        if (activeTimerForIdle != null)
        {
            StopCoroutine(activeTimerForIdle);
        }
    }
    #endregion

    #region Jumping System
    /// <summary>
    /// Marks the player as no longer ready to jump.
    /// Called when jump animation starts to prevent multiple jumps.
    /// </summary>
    private void jumping()
    {
        readyToJump = false;
    }

    /// <summary>
    /// Sets the player as ready to jump again.
    /// Called when jump animation completes.
    /// </summary>
    private void setReadyToJump()
    {
        inputListener.setCanMove(true);
        readyToJump = true;
    }

    /// <summary>
    /// Gets whether the player is ready to perform a jump.
    /// </summary>
    /// <returns>True if player can jump, false otherwise.</returns>
    public bool isReadyToJump()
    {
        return readyToJump;
    }

    /// <summary>
    /// Handles all jumping and airborne transitions.
    /// Triggers jump animation and updates jump state.
    /// </summary>
    public void HandleJump()
    {
        if (isGrounded && readyToJump)
        {
            playerAnimator.SetTrigger("Jump");
            isJumping = true;
        }
    }
    #endregion

    #region Free-Fall and Landing
    /// <summary>
    /// Handles free-fall and grounded state transitions.
    /// Manages the timing for entering free-fall state and landing detection.
    /// </summary>
    private void HandleFreeFallandGrounded()
    {
        // Handle landing from jump or free-fall
        if ((isJumping || isFreeFall) && isGrounded)
        {
            StopTimer();
            isJumping = false;
            isFreeFall = false;
            playerAnimator.SetTrigger("Grounded");
        }

        // Start free-fall timer when leaving ground
        if (!isTimeingtoFreeFall && !isGrounded && !isFreeFall)
        {
            isTimeingtoFreeFall = true;
            activeTimer = StartCoroutine(TimerForFreeFall(1.5f));
        }
    }

    /// <summary>
    /// Stops the free-fall timer coroutine if it's active.
    /// Called when landing to prevent free-fall state.
    /// </summary>
    private void StopTimer()
    {
        if (activeTimer != null)
        {
            StopCoroutine(activeTimer);
        }
    }

    /// <summary>
    /// Coroutine that triggers free-fall animation after a delay.
    /// Provides smooth transition to free-fall state when airborne.
    /// </summary>
    /// <param name="time">Delay before entering free-fall state.</param>
    IEnumerator TimerForFreeFall(float time)
    {
        yield return new WaitForSeconds(time);

        // Only enter free-fall if still airborne
        if (!isGrounded && !isFreeFall)
        {
            playerAnimator.SetTrigger("FreeFall");
            isFreeFall = true;
        }
        isTimeingtoFreeFall = false;
    }
    #endregion

    #region Animation Control
    /// <summary>
    /// Immediately stops player animation by setting velocity to zero.
    /// Used for cutscenes or other situations requiring animation control.
    /// </summary>
    public void stopPlayerAnimation()
    {
        StartCoroutine(TimerForIdle(0f));
    }
    #endregion

    #region Debug Visualization
    /// <summary>
    /// Draws gizmo in the editor to visualize ground check sphere.
    /// Shows ground detection area and current grounded state.
    /// </summary>
    // private void OnDrawGizmosSelected()
    // {
    //     Vector3 checkPosition = transform.position + groundCheckOffset;
    //     // Red = grounded, Green = airborne
    //     Gizmos.color = isGrounded ? Color.red : Color.green;
    //     Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    // }
    #endregion
}
