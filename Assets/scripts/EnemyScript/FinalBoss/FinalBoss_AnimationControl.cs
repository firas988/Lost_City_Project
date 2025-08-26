using UnityEngine;

/// <summary>
/// Controls the animation states of the Final Boss enemy,
/// managing movement, attack, death, and special animations.
/// Integrates with FinalBossControl and FinalBoss_AttackControl components.
/// </summary>
[RequireComponent(typeof(Animator))]
public class FinalBoss_AnimationControl : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the Animator component for controlling animations.</summary>
    private Animator animator;

    /// <summary>Reference to the FinalBossControl script for boss behavior control.</summary>
    private FinalBossControl finalBossControl;

    /// <summary>Reference to the FinalBoss_AttackControl script for attack management.</summary>
    private FinalBoss_AttackControl finalBoss_AttackControl;

    /// <summary>Reference to the Entity component for health and death status.</summary>
    private Entity entity;
    #endregion

    #region State Variables
    /// <summary>Flag indicating if the boss is already dead to prevent multiple death animations.</summary>
    private bool isDead = false;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script for playing boss sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component for playing audio.</summary>
    private AudioSource audioSource;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components and sets up references on start.
    /// </summary>
    void Start()
    {
        // Get required components
        animator = GetComponent<Animator>();
        finalBossControl = GetComponent<FinalBossControl>();
        entity = finalBossControl.getNpcsInstance();
        finalBoss_AttackControl = GetComponent<FinalBoss_AttackControl>();

        // Find and store audio manager
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called every frame to update animation states based on boss behavior.
    /// </summary>
    void Update()
    {
        // Ensure entity reference is valid
        if (entity == null)
        {
            entity = finalBossControl.getNpcsInstance();
            return;
        }

        // Update all animation states
        animashionIsRunning();
        animashionIsAttacking();
        animashionIsDead();
    }
    #endregion

    #region Movement Animation Control
    /// <summary>
    /// Controls the running animation based on boss movement state.
    /// </summary>
    private void animashionIsRunning()
    {
        if (finalBossControl.getMoveToPlayer())
        {
            // Boss is moving to player: disable walking, enable running
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            // Boss is not moving: disable running animation
            animator.SetBool("isRunning", false);
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Controls the attack animation based on boss attack state.
    /// </summary>
    private void animashionIsAttacking()
    {
        if (
            finalBossControl.getIsAttacking() && !finalBoss_AttackControl.isAttackAnimationPlaying()
        )
        {
            // Trigger current attack animation by name
            animator.SetTrigger(finalBoss_AttackControl.getAttackName());

            // Ensure other bool parameters are set to false
            setAllBooleanParamToFalse(finalBoss_AttackControl.getAttackName());
        }
    }
    #endregion

    #region Death Animation Control
    /// <summary>
    /// Controls the death animation and handles death-related effects.
    /// </summary>
    private void animashionIsDead()
    {
        if (entity.isDead() && !isDead)
        {
            // Stop current audio and play death sound
            audioSource.Stop();
            audioManager.playEnemy(audioSource, "FinalBosss_Death");

            // Notify enemy handler and disable movement
            KillEnemyHandler.KilledEnemy(transform.tag);
            finalBossControl.setCanMove(false);

            // Trigger death animation and mark as dead
            animator.SetTrigger("isDead");
            isDead = true;
        }
    }
    #endregion

    #region Animation State Queries
    /// <summary>
    /// Gets the name of the currently playing animation clip.
    /// </summary>
    /// <returns>Name of the current animation clip.</returns>
    public string GetCurrentAnimationClipInfo()
    {
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }
    #endregion

    #region Special Animation Triggers
    /// <summary>
    /// Triggers the summoning animation for the boss.
    /// </summary>
    public void startSummoningAnimation()
    {
        animator.SetTrigger("summon");
    }

    /// <summary>
    /// Triggers the get hit animation for the boss.
    /// </summary>
    public void startGetHitAnimation()
    {
        animator.SetTrigger("getHit");
    }
    #endregion

    #region Animation Parameter Management
    /// <summary>
    /// Sets all boolean parameters in the Animator to false, except the one provided.
    /// </summary>
    /// <param name="ignoreParam">The parameter name to ignore (leave it true).</param>
    public void setAllBooleanParamToFalse(string ignoreParam)
    {
        // Loop through all parameters in the Animator
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            // Only affect boolean parameters, and skip the one to ignore
            if (param.type == AnimatorControllerParameterType.Bool && param.name != ignoreParam)
            {
                animator.SetBool(param.name, false);
            }
        }
    }
    #endregion
}
