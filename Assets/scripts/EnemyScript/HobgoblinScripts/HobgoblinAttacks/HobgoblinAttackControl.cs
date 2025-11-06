using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Hobgoblin enemy,
/// including attack selection, hit detection, and cooldown management.
/// Implements the EnemyAttackBehavior interface for standardized attack behavior.
/// </summary>
public class HobgoblinAttackControl : MonoBehaviour, EnemyAttackBehavior
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

    /// <summary>List of attack places.</summary>
    [SerializeField]
    private List<GameObject> attackPlace;
    #endregion

    #region Attack Data
    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject indicating the point where attack originates (e.g., hand).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the EnemyAttackesConvert script for attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to this Hobgoblin.</summary>
    private List<Attack> hobgoblinAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the EnemyMovement script controlling movement.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the EnemyAnimatorControl script controlling animations.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script controlling player data.</summary>
    private Player player = null;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script for playing hobgoblin sounds.</summary>
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
    /// Initializes components and retrieves the list of available attacks at start.
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

        // Load the list of attacks for this enemy based on its tag
        hobgoblinAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set the default current attack
        currentAttack = hobgoblinAttacks.Find(attack => attack.attackName == "attackHand");
        attackPlacePick(0);
    }

    /// <summary>
    /// Called every frame to update attack selection, origin, and hit detection.
    /// </summary>
    void Update()
    {
        // Check for hits and deal damage if needed
        if (hitCheck())
        {
            dealDamage();

            // Select appropriate attack and find attack origin
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
    /// Finds and assigns the GameObject that marks the origin point of the current attack.
    /// </summary>
    private void attackPlacePick(int index)
    {
        currentAttackPlace = attackPlace[index];
    }

    #endregion

    #region Hit Detection
    /// <summary>
    /// Checks if the current attack hits the player, handles attack cooldown and counts.
    /// </summary>
    /// <returns>True if a hit was detected during this frame.</returns>
    private bool hitCheck()
    {
        // Only check for hits if currently attacking and attack animation is playing
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect colliders within the attack radius at the attack origin
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                // If collider belongs to the player and cooldown is over
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Reset cooldown and increase attack count
                    isHitting = true;
                    if (player == null)
                    {
                        player = col.GetComponent<StartPlayer>().getPlayer();
                    }
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
    /// Plays the hobgoblin attack sound effect.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 0)
        {
            audioManager.playEnemy(audioSource, "HobGoblin_Attack");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Returns true if the current attack animation is playing.
    /// </summary>
    /// <returns>True if the animation is playing; otherwise false.</returns>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Picks the current attack based on the attack count.
    /// </summary>
    private void attackPick()
    {
        // Always use "attackHand" since there is no other attack in this class
        if (attackCount <= 0)
        {
            currentAttack = hobgoblinAttacks.Find(attack => attack.attackName == "attackHand");
            attackPlacePick(0);
        }
    }
    #endregion

    #region Interface Implementation
    /// <summary>Gets the name of the current attack.</summary>
    /// <returns>The name of the current attack.</returns>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>Gets the range of the current attack.</summary>
    /// <returns>The range of the current attack.</returns>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>Gets the attack duration/time.</summary>
    /// <returns>The duration of the current attack.</returns>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the damage of the current attack.</summary>
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
    /// <summary>Starts the Hobgoblin's attack animation.</summary>
    public void startAttackHobgoblin()
    {
        ClearAudioSource();
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Hobgoblin's attack animation.</summary>
    public void endAttackHobgoblin()
    {
        isHitting = false;
        isAttacking = false;
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
    /// Visualizes the attack radius in the editor with Gizmos.
    /// Draws red if hit detected this frame, otherwise green.
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
