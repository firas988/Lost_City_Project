using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack logic for the Monster Mutant enemy,
/// including selecting attacks, detecting hits, and managing cooldowns.
/// Implements the EnemyAttackBehavior interface for standardized attack behavior.
/// </summary>
public class MonsterMutantAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Current count of attacks performed.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum allowed attack count before resetting.</summary>
    private int attackCountMax = 2;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;
    #endregion

    #region Attack Data
    /// <summary>The current attack being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject representing the attack origin point (e.g., hand).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the script that provides enemy attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available for this monster mutant.</summary>
    private List<Attack> monsterMutantAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the enemy movement controller.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the enemy animation controller.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script for damage interaction.</summary>
    private Player player = null;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script for playing monster mutant sounds.</summary>
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
    /// Initializes references and loads attack data on start.
    /// </summary>
    void Start()
    {
        // Get references to movement and animation scripts
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();

        // Retrieve the list of attacks for this enemy based on its tag
        monsterMutantAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set the default attack to "attackHand"
        currentAttack = monsterMutantAttacks.Find(attack => attack.attackName == "attackHand");
    }

    /// <summary>
    /// Called once per frame to handle attack selection, origin, and hit detection.
    /// </summary>
    void Update()
    {
        attackPick(); // Choose appropriate attack based on attack count
        attackPlacePick(); // Find the attack origin GameObject

        // Check for hits and deal damage if needed
        if (hitCheck())
        {
            dealDamage();
        }
    }
    #endregion

    #region Damage System
    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    private void dealDamage()
    {
        player.takeDamage(currentAttack.attackDamage);
    }
    #endregion

    #region Attack Origin Management
    /// <summary>
    /// Finds and sets the current attack origin GameObject by attack name.
    /// </summary>
    private void attackPlacePick()
    {
        currentAttackPlace = FindDeepChild(transform, currentAttack.attackName).gameObject;
    }

    /// <summary>
    /// Recursively searches for a child transform by name within the hierarchy.
    /// </summary>
    /// <param name="parent">Parent transform to start search from.</param>
    /// <param name="name">Name of the child to find.</param>
    /// <returns>Transform if found; otherwise, null.</returns>
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
    #endregion

    #region Hit Detection
    /// <summary>
    /// Checks if the current attack hits the player and manages cooldown and attack count.
    /// </summary>
    /// <returns>True if a hit occurred this frame; otherwise false.</returns>
    private bool hitCheck()
    {
        // Only check for hits if currently attacking and animation is playing
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect all colliders within the attack radius at the attack origin
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                // If a collider tagged "Player" is found and cooldown is finished
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Reset cooldown timer to the current attack's duration
                    isHitting = true;
                    if (player == null)
                    {
                        player = col.GetComponent<StartPlayer>().getPlayer();
                    }
                    // Increment attack count and reset if exceeded max
                    attackCount++;
                    if (attackCount > attackCountMax)
                    {
                        attackCount = 0;
                    }
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region Audio Management
    /// <summary>
    /// Plays the appropriate attack sound based on attack count.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 1)
        {
            audioManager.playEnemy(audioSource, "MonsterMutant_AttackHand");
        }
        else if (attackCount == 2)
        {
            audioManager.playEnemy(audioSource, "MonsterMutant_AttackSpike");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks if the current attack animation is playing.
    /// </summary>
    /// <returns>True if current animation matches current attack name.</returns>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Selects the current attack based on the attack count.
    /// </summary>
    private void attackPick()
    {
        if (attackCount <= 1)
        {
            currentAttack = monsterMutantAttacks.Find(attack => attack.attackName == "attackHand");
        }
        else if (attackCount == 2)
        {
            currentAttack = monsterMutantAttacks.Find(attack => attack.attackName == "attackSpike");
        }
    }
    #endregion

    #region Interface Implementation
    /// <summary>Returns the name of the current attack.</summary>
    /// <returns>The name of the current attack.</returns>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>Returns the attack range of the current attack.</summary>
    /// <returns>The range of the current attack.</returns>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>Returns the attack duration/time of the current attack.</summary>
    /// <returns>The duration of the current attack.</returns>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Returns the damage amount of the current attack.</summary>
    /// <returns>The damage value of the current attack.</returns>
    public float getAttackDamage()
    {
        return currentAttack.attackDamage;
    }

    /// <summary>Gets whether the enemy is currently attacking.</summary>
    /// <returns>True if attacking; otherwise false.</returns>
    public bool getIsAttacking()
    {
        return isAttacking;
    }
    #endregion

    #region Attack Control Methods
    /// <summary>Starts the Monster Mutant's attack animation.</summary>
    public void startAttackMonsterMutant()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Monster Mutant's attack animation.</summary>
    public void endAttackMonsterMutant()
    {
        isHitting = false;
        isAttacking = false;
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the detection and chase ranges
    /// <summary>
    /// Draws gizmos in the scene view to visualize the attack range.
    /// </summary>
    // void OnDrawGizmos()
    // {
    //     if (currentAttackPlace == null || currentAttack == null)
    //         return;

    //     if (hitCheck())
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawWireSphere(
    //             currentAttackPlace.transform.position,
    //             currentAttack.attackRadius
    //         );
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireSphere(
    //             currentAttackPlace.transform.position,
    //             currentAttack.attackRadius
    //         );
    //     }
    // }
    #endregion
}
