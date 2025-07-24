using System.Collections;
using UnityEngine;

/// <summary>
/// Controls animation states of an enemy based on its movement and attack state.
/// </summary>
public class EnemyAnimatorControl : MonoBehaviour
{
    /// <summary>Reference to the Animator component.</summary>
    private Animator animator;

    /// <summary>Reference to the enemy's movement behavior.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the enemy's attack behavior.</summary>
    private EnemyAttackBehavior enemyAttackBehavior;

    /// <summary>Reference to the StartNpc component.</summary>
    private StartNpc startNpc;

    /// <summary>Reference to the Entity component.</summary>
    private Entity entity;

    /// <summary>Reference to the statistics handler.</summary>
    private StatisticsHandler statisticsHandler;

    private bool isDead = false;

    private DissolvingController dissolvingController;

    /// <summary>Reference to the AudioManager script.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component.</summary>
    private AudioSource audioSource;

    /// <summary>
    /// Initializes references to components.
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttackBehavior = GetComponent<EnemyAttackBehavior>();
        startNpc = GetComponent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        dissolvingController = GetComponent<DissolvingController>();
        audioManager = FindAnyObjectByType<AudioManager>();
        audioSource = GetComponent<AudioSource>();
        // Log warning if attack behavior not found (interface not attached).
        if (enemyAttackBehavior == null)
        {
            Debug.Log("EnemyAttackBehavior component not found!");
        }
    }

    /// <summary>
    /// Called once per frame to update animation states.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.setHealth(0);
        }
        animashionIsChassing();
        animashionIsAttacking();
        animashionIsDead();
    }

    /// <summary>
    /// Updates animator parameters based on chasing state.
    /// </summary>
    private void animashionIsChassing()
    {
        if (enemyMovement.getIsChassing())
        {
            // If enemy is chasing: enable chasing animation, disable walking.
            animator.SetBool("isChassing", true);
            animator.SetBool("isWalking", false);
        }
        else
        {
            // If not chasing: disable chasing animation.
            animator.SetBool("isChassing", false);

            // //logic to re-enable walking if not attacking.
            // if (!enemyMovement.getIsAttacking())
            // {
            //     animator.SetBool("isWalking", true);
            // }
        }
    }

    /// <summary>
    /// Updates animator parameters based on attack state and selected attack.
    /// </summary>
    private void animashionIsAttacking()
    {
        if (enemyMovement.getIsAttacking())
        {
            // Trigger current attack animation by name.
            animator.SetBool(enemyAttackBehavior.getAttackName(), true);

            // Ensure other bool parameters are set to false.
            setAllBooleanParamToFalse(enemyAttackBehavior.getAttackName());
        }
        else
        {
            // Stop attack animation when no longer attacking.
            animator.SetBool(enemyAttackBehavior.getAttackName(), false);
        }
    }

    /// <summary>
    /// Gets the name of the current playing animation clip.
    /// </summary>
    /// <returns>Animation clip name as string.</returns>
    public string GetCurrentAnimationClipInfo()
    {
        // Returns the first animation clip currently playing on layer 0.
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
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

    private void animashionIsDead()
    {
        if (entity.isDead() && !isDead)
        {
            audioManager.playEnemy(audioSource, transform.tag + "_Death");
            KillEnemyHandler.KilledEnemy(transform.tag);
            enemyMovement.setCanMove(false);
            animator.SetTrigger("isDead");
            isDead = true;
            StartCoroutine(Dissolve());
        }
    }

    private IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(4f);
        dissolvingController.StartDissolve();
    }
}
