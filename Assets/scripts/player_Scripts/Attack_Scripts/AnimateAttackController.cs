using UnityEngine;

/// <summary>
/// Controls the attack and death animations for a character using Animator triggers and booleans.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimateAttackController : MonoBehaviour
{
    // ===== INSTANCE VARIABLES =====
    /// <summary>
    /// Reference to the Animator component.
    /// </summary>
    private Animator animator;

    // ===== BOOLEANS =====
    /// <summary>
    /// Indicates whether the character has already died to prevent repeating the death animation.
    /// </summary>
    private bool isDead = false;

    /// <summary>
    /// Initializes the Animator component.
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Plays the death animation if the character hasn't already died.
    /// </summary>
    public void DeathAnimation()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("Death");
        }
    }

    /// <summary>
    /// Triggers a simple hit animation (usually for when the character is attacked).
    /// </summary>
    public void AttackAnimation()
    {
        animator.SetTrigger("isHit");
    }

    /// <summary>
    /// Starts the attack animation sequence using a boolean and a trigger.
    /// </summary>
    public void StartAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        animator.SetTrigger("StartAttack");
    }

    /// <summary>
    /// Stops the attack animation sequence.
    /// </summary>
    public void StopAttackAnimation()
    {
        animator.SetBool("isAttack", false);
        animator.SetTrigger("StopAttack");
    }

    public void spawnAnimation()
    {
        animator.SetTrigger("Spawn");
    }
}
