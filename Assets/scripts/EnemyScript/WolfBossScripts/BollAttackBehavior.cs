using System.Collections;
using UnityEngine;

/// <summary>
/// StateMachineBehaviour that controls the jump attack (bolt) animation state for the Wolf Boss.
/// Manages attack completion, cooldown initiation, and cleanup when the jump attack ends.
/// Integrates with WolfBossAttacking for comprehensive attack management.
/// </summary>
public class BollAttackBehavior : StateMachineBehaviour
{
    #region Animation State Callbacks
    /// <summary>
    /// Called when entering the jump attack animation state.
    /// Currently unused but available for future implementation.
    /// </summary>
    /// <param name="animator">The Animator component.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The layer index of the animation state.</param>
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    ) { }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    /// <summary>
    /// Called when exiting the jump attack animation state.
    /// Performs cleanup operations including animation reset, movement re-enabling,
    /// attack state reset, and cooldown initiation.
    /// </summary>
    /// <param name="animator">The Animator component.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The layer index of the animation state.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset jump attack animation parameter
        animator.SetBool("JumpAttack", false);

        // Re-enable NavMeshAgent for movement
        animator.GetComponentInParent<WolfBossAttacking>().activateNavMeshAgent();

        // Reset attacking state
        animator.GetComponentInParent<WolfBossAttacking>().setIsAttacking(false);

        // Start jump attack cooldown
        animator
            .GetComponentInParent<WolfBossAttacking>()
            .StartCoroutine(
                animator.GetComponentInParent<WolfBossAttacking>().JumpAttackCooldown()
            );
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    #endregion
}
