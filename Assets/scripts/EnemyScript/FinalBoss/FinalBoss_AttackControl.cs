using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Final Boss enemy,
/// managing attack selection, hit detection, and cooldowns.
/// Implements the EnemyAttackBehavior interface.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FinalBoss_AttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>Counter for the number of attacks.</summary>
    private int attackCount = 0;

    /// <summary>Maximum number of attacks.</summary>
    private int attackCountMax = 1;
    #endregion

    #region Attack Data
    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject marking the origin point of the current attack (e.g., paw or mouth).</summary>
    private GameObject currentAttackPlace;

    /// <summary>List of attacks available to the Final Boss.</summary>
    private List<Attack> FinalBossAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the EnemyAttackesConvert script that provides attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>Reference to the Player script for interacting with the player.</summary>
    private Player player = null;

    /// <summary>Reference to the AudioSource component for playing attack sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing attack sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the FinalBossControl script for boss behavior control.</summary>
    private FinalBossControl finalBossControl;

    /// <summary>Reference to the FinalBoss_AnimationControl script for animation management.</summary>
    private FinalBoss_AnimationControl finalBoss_AnimationControl;
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
        audioSource = GetComponent<AudioSource>();

        // Find and store enemy attack converter
        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();

        // Load final boss-specific attacks
        FinalBossAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set initial attack to final boss attack
        currentAttack = FinalBossAttacks.Find(attack => attack.attackName == "Final_Boss_attack");

        // Find and store audio manager
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();

        // Get boss control components
        finalBossControl = GetComponent<FinalBossControl>();
        finalBoss_AnimationControl = GetComponent<FinalBoss_AnimationControl>();
    }

    /// <summary>
    /// Called every frame to update attack choice, attack origin, and hit detection.
    /// </summary>
    void Update()
    {
        // Update attack selection and origin point
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
        // Find the child object that represents the attack origin point
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
        // Search through all direct children
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            // Recursively search deeper in the hierarchy
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
        // Only check for hits when attacking and animation is playing
        if (finalBossControl.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Create a sphere around the attack origin point
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            // Check each collider in the attack radius
            foreach (Collider col in hitColliders)
            {
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
    /// Plays the appropriate attack sound based on attack count.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 1)
        {
            // Play final boss attack sound
            audioManager.playEnemy(audioSource, "FinalBoss_Attack");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks if the current attack animation is playing.
    /// </summary>
    public bool isAttackAnimationPlaying()
    {
        return finalBoss_AnimationControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Selects the current attack based on the attack count.
    /// </summary>
    private void attackPick()
    {
        if (attackCount <= 1)
        {
            // Set attack to final boss attack
            currentAttack = FinalBossAttacks.Find(attack =>
                attack.attackName == "Final_Boss_attack"
            );
        }
    }
    #endregion

    #region Interface Implementation
    /// <summary>Gets the current attack's name.</summary>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>Gets the current attack's range.</summary>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>Gets the current attack's duration/time.</summary>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the current attack's damage value.</summary>
    public float getAttackDamage()
    {
        return currentAttack.attackDamage;
    }
    #endregion

    #region Attack Control Methods
    /// <summary>Starts the Final Boss's attack animation.</summary>
    public void startAttackFinalBoss()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Final Boss's attack animation.</summary>
    public void endAttackFinalBoss()
    {
        isHitting = false;
        isAttacking = false;
    }

    /// <summary>Gets the current attacking state.</summary>
    public bool getIsAttacking()
    {
        return isAttacking;
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
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.green;
    //     }

    //     Gizmos.DrawWireSphere(currentAttackPlace.transform.position, currentAttack.attackRadius);
    // }
    #endregion
}
