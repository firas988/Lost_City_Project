using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the wolf enemy attack logic, including selecting attacks,
/// checking for player hits, and integrating with movement and animation systems.
/// Implements the EnemyAttackBehavior interface for standardized attack behavior.
/// </summary>
public class WolfAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Counter for how many times the current attack has occurred.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>The maximum number of times an attack can be repeated before resetting.</summary>
    private int attackCountMax = 0;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;
    #endregion

    #region Attack Data
    /// <summary>The currently selected attack.</summary>
    private Attack currentAttack;

    /// <summary>The position in the hierarchy where the attack originates from.</summary>
    private GameObject currentAttackPlace;

    /// <summary>Component responsible for providing enemy attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of all attacks available to the wolf.</summary>
    private List<Attack> wolfAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the enemy movement controller.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the enemy animation controller.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script for interacting with the player.</summary>
    private Player player = null;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component.</summary>
    private AudioSource audioSource;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components and loads attack data for the wolf.
    /// </summary>
    void Start()
    {
        // Get required components
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();
        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();
        wolfAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);
        currentAttack = wolfAttacks.Find(attack => attack.attackName == "attackBite");
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called every frame to handle attack logic and hit detection.
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
    /// Finds the transform of the attack location based on the current attack's name.
    /// </summary>
    private void attackPlacePick()
    {
        currentAttackPlace = FindDeepChild(transform, currentAttack.attackName).gameObject;
    }

    /// <summary>
    /// Recursively searches for a child transform by name.
    /// </summary>
    /// <param name="parent">The parent transform to search in.</param>
    /// <param name="name">The name of the child to find.</param>
    /// <returns>The matching child transform, or null if not found.</returns>
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
    /// Checks if the enemy is currently attacking and whether it hits the player.
    /// Applies cooldown and counts hits.
    /// </summary>
    /// <returns>True if the attack hits the player, otherwise false.</returns>
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
    /// Plays the wolf attack sound effect.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 0)
        {
            audioManager.playEnemy(audioSource, "Wolf_Attack");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks if the current attack animation is playing.
    /// </summary>
    /// <returns>True if the animation is playing, otherwise false.</returns>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Picks the default attack if no attacks have been used yet.
    /// </summary>
    private void attackPick()
    {
        if (attackCount <= 0)
        {
            currentAttack = wolfAttacks.Find(attack => attack.attackName == "attackBite");
        }
    }
    #endregion

    #region Interface Implementation
    /// <summary>
    /// Gets the name of the current attack.
    /// </summary>
    /// <returns>The attack name.</returns>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>
    /// Gets the range of the current attack.
    /// </summary>
    /// <returns>The attack range.</returns>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>
    /// Gets the time duration of the current attack.
    /// </summary>
    /// <returns>The attack time.</returns>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>
    /// Gets the damage value of the current attack.
    /// </summary>
    /// <returns>The attack damage.</returns>
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
    /// <summary>Starts the Wolf's attack animation.</summary>
    public void startAttackWolf()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Wolf's attack animation.</summary>
    public void endAttackWolf()
    {
        isAttacking = false;
        isHitting = false;
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the attack hit radius in the editor
    /// <summary>
    /// Draws gizmos in the editor to visualize the attack hit radius.
    /// Red if a hit is detected, green otherwise.
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
