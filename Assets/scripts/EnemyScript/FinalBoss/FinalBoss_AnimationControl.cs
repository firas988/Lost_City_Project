using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FinalBoss_AnimationControl : MonoBehaviour
{
    private Animator animator;
    private FinalBossControl finalBossControl;
    private FinalBoss_AttackControl finalBoss_AttackControl;
    private Entity entity;
    private bool isDead = false;
    private AudioManager audioManager;
    private AudioSource audioSource;
    private string gameManagerTag = "GameManager";

    void Start()
    {
        animator = GetComponent<Animator>();
        finalBossControl = GetComponent<FinalBossControl>();
        entity = (Entity)finalBossControl.getNpcsInstance();
        finalBoss_AttackControl = GetComponent<FinalBoss_AttackControl>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        animashionIsRunning();
        animashionIsAttacking();
        animashionIsDead();
    }

    private void animashionIsRunning()
    {
        if (finalBossControl.getMoveToPlayer())
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void animashionIsAttacking()
    {
        if (
            finalBossControl.getIsAttacking() && !finalBoss_AttackControl.isAttackAnimationPlaying()
        )
        {
            // Trigger current attack animation by name.
            animator.SetTrigger(finalBoss_AttackControl.getAttackName());

            // Ensure other bool parameters are set to false.
            setAllBooleanParamToFalse(finalBoss_AttackControl.getAttackName());
        }
    }

    private void animashionIsDead()
    {
        if (entity.isDead() && !isDead)
        {
            audioSource.Stop();
            audioManager.playEnemy(audioSource, transform.tag + "_Death");
            KillEnemyHandler.KilledEnemy(transform.tag);
            finalBossControl.setCanMove(false);
            animator.SetTrigger("isDead");
            isDead = true;
        }
    }

    public string GetCurrentAnimationClipInfo()
    {
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void startSummoningAnimation()
    {
        animator.SetTrigger("summon");
    }

    public void startGetHitAnimation()
    {
        animator.SetTrigger("getHit");
    }

    /// <summary>
    /// Sets all boolean parameters in the Animator to false, except the one provided.
    /// </summary>
    /// <param name="ignoreParam">The parameter name to ignore (leave it true).</param>
    public void setAllBooleanParamToFalse(string ignoreParam)
    {
        // Loop through all parameters in the Animator.
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            // Only affect boolean parameters, and skip the one to ignore.
            if (param.type == AnimatorControllerParameterType.Bool && param.name != ignoreParam)
            {
                animator.SetBool(param.name, false);
            }
        }
    }
}
