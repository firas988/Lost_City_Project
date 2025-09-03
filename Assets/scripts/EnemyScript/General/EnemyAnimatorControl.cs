using System.Collections;
using UnityEngine;

/// <summary>
/// Controls animation states of an enemy based on its movement and attack state.
/// Integrates with EnemyMovement, EnemyAttackBehavior, Entity, and DissolvingController components.
/// </summary>
public class EnemyAnimatorControl : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the Animator component for controlling animations.</summary>
    private Animator animator;

    /// <summary>Reference to the enemy's movement behavior.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the enemy's attack behavior.</summary>
    private EnemyAttackBehavior enemyAttackBehavior;

    /// <summary>Reference to the StartNpc component for NPC instance management.</summary>
    private StartNpc startNpc;

    /// <summary>Reference to the Entity component for health and death status.</summary>
    private Entity entity;

    /// <summary>Reference to the statistics handler for tracking enemy statistics.</summary>
    private StatisticsHandler statisticsHandler;

    /// <summary>Reference to the DissolvingController for death dissolve effects.</summary>
    private DissolvingController dissolvingController;

    /// <summary>Reference to the AudioManager script for playing enemy sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component for playing audio.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the LevelManager component for XP management.</summary>
    private LevelManager levelManager;
    #endregion

    #region State Variables
    /// <summary>Flag indicating if the enemy is already dead to prevent multiple death animations.</summary>
    private bool isDead = false;

    /// <summary>Test field for debugging purposes.</summary>
    private bool isTest = false;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references to components on start.
    /// </summary>
    private void Start()
    {
        // Get required components
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttackBehavior = GetComponent<EnemyAttackBehavior>();
        startNpc = GetComponent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        dissolvingController = GetComponent<DissolvingController>();

        // Find and store audio manager
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        // Log warning if attack behavior not found (interface not attached)
        if (enemyAttackBehavior == null)
        {
            Debug.LogWarning("EnemyAttackBehavior component not found!");
        }

        // Find and store level manager
        levelManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<LevelManager>();
    }

    /// <summary>
    /// Called once per frame to update animation states.
    /// </summary>
    void Update()
    {
        // Debug key for testing (set health to 0)
        if (Input.GetKeyDown(KeyCode.R) && !isTest)
        {
            entity.setHealth(0);
            StartCoroutine(startCoolDown());
        }

        // Update all animation states
        animashionIsChassing();
        animashionIsAttacking();
        animashionIsDead();
    }
    #endregion

    #region Movement Animation Control
    /// <summary>
    /// Updates animator parameters based on chasing state.
    /// </summary>
    private void animashionIsChassing()
    {
        if (enemyMovement.getIsChassing())
        {
            // If enemy is chasing: enable chasing animation, disable walking
            animator.SetBool("isChassing", true);
            animator.SetBool("isWalking", false);
        }
        else
        {
            // If not chasing: disable chasing animation
            animator.SetBool("isChassing", false);

            // Logic to re-enable walking if not attacking (commented out)
            // if (!enemyMovement.getIsAttacking())
            // {
            //     animator.SetBool("isWalking", true);
            // }
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Updates animator parameters based on attack state and selected attack.
    /// </summary>
    private void animashionIsAttacking()
    {
        if (enemyMovement.getIsAttacking())
        {
            // Trigger current attack animation by name
            animator.SetBool(enemyAttackBehavior.getAttackName(), true);

            // Ensure other bool parameters are set to false
            setAllBooleanParamToFalse(enemyAttackBehavior.getAttackName());
        }
        else
        {
            // Stop attack animation when no longer attacking
            animator.SetBool(enemyAttackBehavior.getAttackName(), false);
        }
    }
    #endregion

    #region Animation State Queries
    /// <summary>
    /// Gets the name of the current playing animation clip.
    /// </summary>
    /// <returns>Animation clip name as string.</returns>
    public string GetCurrentAnimationClipInfo()
    {
        // Returns the first animation clip currently playing on layer 0
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }
    #endregion

    #region Animation Parameter Management
    /// <summary>
    /// Sets all boolean parameters in the Animator to false, except the one provided.
    /// </summary>
    /// <param name="ignoreParam">The parameter name to ignore (leave it true).</param>
    public void setAllBooleanParamToFalse(string ignoreParam = "")
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

    #region Death Animation Control
    /// <summary>
    /// Controls the death animation and handles death-related effects.
    /// </summary>
    private void animashionIsDead()
    {
        if (entity.isDead() && !isDead)
        {

            int xpToAdd = Random.Range(250, 400);
            levelManager.addXP(xpToAdd);
            // Stop current audio and play death sound
            audioSource.Stop();
            audioManager.playEnemy(audioSource, transform.tag + "_Death");

            // Notify enemy handler and disable movement
            KillEnemyHandler.KilledEnemy(transform.tag);
            enemyMovement.setCanMove(false);

            // Trigger death animation and mark as dead
            animator.SetTrigger("isDead");
            isDead = true;

            // Start dissolve effect if object is active
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Dissolve());
            }
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine that starts the dissolve effect after a delay.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator Dissolve()
    {
        // Wait before starting dissolve effect
        yield return new WaitForSeconds(4f);
        dissolvingController.StartDissolve();
    }

    /// <summary>
    /// Coroutine that manages the test cooldown to prevent rapid health setting.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator startCoolDown()
    {
        isTest = true;
        yield return new WaitForSeconds(2f);
        isTest = false;
    }
    #endregion
}
