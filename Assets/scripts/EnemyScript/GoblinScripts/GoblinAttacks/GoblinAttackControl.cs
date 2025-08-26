using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Goblin enemy,
/// managing attack selection, hit detection, and cooldowns.
/// Implements the EnemyAttackBehavior interface for standardized attack behavior.
/// </summary>
public class GoblinAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Current number of attacks performed.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum attack count before reset.</summary>
    private int attackCountMax = 0;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;
    #endregion

    #region Attack Data
    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject marking the origin point of the current attack (e.g., hand).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the EnemyAttackesConvert script that provides attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to the Goblin.</summary>
    private List<Attack> goblinAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the EnemyMovement script for movement control.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the EnemyAnimatorControl script for animation control.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script controlling player data.</summary>
    private Player player = null;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script for playing goblin sounds.</summary>
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
    /// Initializes components and loads attack data on start.
    /// </summary>
    void Start()
    {
        // Get required components
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        // Get attack data converter
        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();

        // Load goblin-specific attacks
        goblinAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set default attack to hand attack
        currentAttack = goblinAttacks.Find(attack => attack.attackName == "attackHand");
    }

    /// <summary>
    /// Called every frame to update attack choice, attack origin, and hit detection.
    /// </summary>
    void Update()
    {
        // Select appropriate attack and find attack origin
        attackPick();
        attackPlacePick();

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
    /// Finds and sets the GameObject representing the attack origin point by name.
    /// </summary>
    private void attackPlacePick()
    {
        currentAttackPlace = FindDeepChild(transform, currentAttack.attackName).gameObject;
    }

    /// <summary>
    /// Recursively searches for a child Transform by name.
    /// </summary>
    /// <param name="parent">Parent transform to search under.</param>
    /// <param name="name">Name of the child transform to find.</param>
    /// <returns>Found Transform or null if none found.</returns>
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
    /// Checks for hit detection on the player within attack radius and handles cooldown.
    /// </summary>
    /// <returns>True if a hit was detected this frame; otherwise false.</returns>
    private bool hitCheck()
    {
        // Only check for hits if currently attacking and animation is playing
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect colliders within attack radius
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                // Check if player was hit and not already hitting
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Mark as hitting to prevent multiple hits
                    isHitting = true;

                    // Get player reference if not already stored
                    if (player == null)
                    {
                        player = col.GetComponent<StartPlayer>().getPlayer();
                    }

                    // Increment attack count and reset if max reached
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
    /// Plays the goblin attack sound effect.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 0)
        {
            audioManager.playEnemy(audioSource, "Goblin_Attack");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks if the current attack animation is playing.
    /// </summary>
    /// <returns>True if the animation is playing; otherwise false.</returns>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Selects the current attack based on the attack count.
    /// </summary>
    private void attackPick()
    {
        if (attackCount <= 0)
        {
            currentAttack = goblinAttacks.Find(attack => attack.attackName == "attackHand");
        }
    }
    #endregion

    #region Interface Implementation
    /// <summary>Gets the current attack's name.</summary>
    /// <returns>The name of the current attack.</returns>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>Gets the current attack's range.</summary>
    /// <returns>The range of the current attack.</returns>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>Gets the current attack's duration/time.</summary>
    /// <returns>The duration of the current attack.</returns>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the current attack's damage value.</summary>
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
    /// <summary>Starts the Goblin's attack animation.</summary>
    public void startAttackGoblin()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Goblin's attack animation.</summary>
    public void endAttackGoblin()
    {
        isHitting = false;
        isAttacking = false;
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the attack range in the editor
    /// <summary>
    /// Draws Gizmos in the editor to visualize the attack radius.
    /// Red indicates a hit detected this frame, green otherwise.
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
