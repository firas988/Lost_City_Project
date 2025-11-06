using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Bear enemy,
/// managing attack selection, hit detection, and cooldowns.
/// Implements the EnemyAttackBehavior interface.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BearAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Current number of attacks performed.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum attack count before reset.</summary>
    private int attackCountMax = 3;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>List of attack places.</summary>
    [SerializeField]
    private List<GameObject> attackPlace;
    #endregion

    #region Attack Data
    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject marking the origin point of the current attack (e.g., paw or mouth).</summary>
    private GameObject currentAttackPlace;

    /// <summary>List of attacks available to the Bear.</summary>
    private List<Attack> bearAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the EnemyAttackesConvert script that provides attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>Reference to the EnemyMovement script for movement control.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the EnemyAnimatorControl script for animation control.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script for interacting with the player.</summary>
    private Player player = null;

    /// <summary>Reference to the AudioSource component for playing attack sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing attack sounds.</summary>
    private AudioManager audioManager;
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
        audioSource = GetComponent<AudioSource>();

        // Find and store enemy attack converter
        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();

        // Load bear-specific attacks
        bearAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set initial attack to hand attack
        currentAttack = bearAttacks.Find(attack => attack.attackName == "attackHand");
        attackPlacePick(0);

        // Find and store audio manager
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
    }

    /// <summary>
    /// Called every frame to update attack choice, attack origin, and hit detection.
    /// </summary>
    void Update()
    {
        // Check for hits and deal damage if needed
        if (hitCheck())
        {
            dealDamage();
            // Update attack selection and origin point
            attackPick();
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
    private void attackPlacePick(int index)
    {
        // Find the child object that represents the attack origin point
        currentAttackPlace = attackPlace[index];
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
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
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
            audioManager.playEnemy(audioSource, "Bear_Attack1");
        }
        else if (attackCount == 2)
        {
            audioManager.playEnemy(audioSource, "Bear_Attack2");
        }
        else if (attackCount == 3)
        {
            audioManager.playEnemy(audioSource, "Bear_Attack3");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks if the current attack animation is playing.
    /// </summary>
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
            // First attack: hand attack
            currentAttack = bearAttacks.Find(attack => attack.attackName == "attackHand");
            attackPlacePick(0);
        }
        else if (attackCount == 2)
        {
            // Second attack: bite attack
            currentAttack = bearAttacks.Find(attack => attack.attackName == "attackBite");
            attackPlacePick(1);
        }
        else if (attackCount == 3)
        {
            // Third attack: jump attack
            currentAttack = bearAttacks.Find(attack => attack.attackName == "attackJump");
            attackPlacePick(2);
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
    /// <summary>Starts the Bear's attack animation.</summary>
    public void startAttackBear()
    {
        ClearAudioSource();
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Bear's attack animation.</summary>
    public void endAttackBear()
    {
        isHitting = false;
        isAttacking = false;
    }

    /// <summary>Gets the current attacking state.</summary>
    public bool getIsAttacking()
    {
        return isAttacking;
    }

    /// <summary>Clears the audio source.</summary>
    private void ClearAudioSource()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }
    #endregion

    #region Debug Visualization
    // Enable this to see the attack range in the editor
    /// <summary>
    /// Draws Gizmos in the editor to visualize the attack radius.
    /// Red indicates a hit detected this frame, green otherwise.
    /// </summary>
    void OnDrawGizmos()
    {
        if (currentAttackPlace == null || currentAttack == null)
            return;

        if (hitCheck())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireSphere(currentAttackPlace.transform.position, currentAttack.attackRadius);
    }
    #endregion
}
