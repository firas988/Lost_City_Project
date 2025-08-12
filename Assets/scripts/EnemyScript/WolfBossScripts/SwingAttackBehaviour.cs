using UnityEngine;

public class SwingAttackBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    ) { 
      
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Swing", false);

        animator.GetComponentInParent<WolfBossAttacking>().activateNavMeshAgent();
        animator.GetComponentInParent<WolfBossAttacking>().setIsAttacking(false);
        animator
            .GetComponentInParent<WolfBossAttacking>()
            .StartCoroutine(
                animator.GetComponentInParent<WolfBossAttacking>().SwingAttackCooldown()
            );
        animator.GetComponentInParent<WolfBossAttacking>().GetComponentInChildren<HandCollisionObserver>().disableHandCollider();
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
}
