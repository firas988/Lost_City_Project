using UnityEngine;

/// <summary>
/// Controls the attack and death animations for a character using Animator triggers and booleans.
/// Manages animation states for combat, death, and spawning through the Unity Animator system.
/// Provides a clean interface for other components to trigger character animations.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimateAttackController : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Reference to the Animator component for controlling character animations.
    /// Automatically required by the RequireComponent attribute.
    /// </summary>
    private Animator animator;
    #endregion

    #region Animation State
    /// <summary>
    /// Indicates whether the character has already died to prevent repeating the death animation.
    /// Prevents multiple death animation triggers during the same death sequence.
    /// </summary>
    private bool isDead = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the Animator component reference.
    /// Gets the Animator component from this GameObject for animation control.
    /// </summary>
    void Start()
    {
        // Get the Animator component for animation control
        animator = GetComponent<Animator>();
    }
    #endregion

    #region Death Animation
    /// <summary>
    /// Plays the death animation if the character hasn't already died.
    /// Sets the death state to prevent multiple death animation triggers.
    /// </summary>
    public void DeathAnimation()
    {
        // Only play death animation if not already dead
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("Death");
        }
    }
    #endregion

    #region Combat Animations
    /// <summary>
    /// Triggers a simple hit animation (usually for when the character is attacked).
    /// Uses the "isHit" trigger parameter for immediate hit feedback.
    /// </summary>
    public void AttackAnimation()
    {
        // Trigger hit animation for when character is attacked
        animator.SetTrigger("isHit");
    }

    /// <summary>
    /// Starts the attack animation sequence using a boolean and a trigger.
    /// Sets the attack state and triggers the attack start animation.
    /// </summary>
    public void StartAttackAnimation()
    {
        // Set attack state and trigger attack start
        animator.SetBool("isAttack", true);
        animator.SetTrigger("StartAttack");
    }

    /// <summary>
    /// Stops the attack animation sequence.
    /// Resets the attack state and triggers the attack stop animation.
    /// </summary>
    public void StopAttackAnimation()
    {
        // Reset attack state and trigger attack stop
        animator.SetBool("isAttack", false);
        animator.SetTrigger("StopAttack");
    }
    #endregion

    #region Spawn Animation
    /// <summary>
    /// Triggers the spawn animation for the character.
    /// Used when the character respawns or appears in the scene.
    /// </summary>
    public void spawnAnimation()
    {
        // Trigger spawn animation for character appearance
        animator.SetTrigger("Spawn");
    }
    #endregion
}
