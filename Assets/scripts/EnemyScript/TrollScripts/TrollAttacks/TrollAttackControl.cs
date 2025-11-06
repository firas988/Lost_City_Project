using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls attack logic for the Troll enemy,
/// including picking attacks, hit detection, and cooldown handling.
/// Implements the EnemyAttackBehavior interface for standardized attack behavior.
/// </summary>
public class TrollAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    #region Attack State Variables
    /// <summary>Counter for how many times the troll has attacked.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum allowed attack count before reset.</summary>
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
    /// <summary>Currently selected attack from the list.</summary>
    private Attack currentAttack;

    /// <summary>GameObject representing the attack origin (e.g., hand, weapon).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Script that provides attack data for enemies.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to this enemy.</summary>
    private List<Attack> trollAttacks;
    #endregion

    #region Component References
    /// <summary>Reference to the movement controller of the enemy.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the animation controller of the enemy.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script for damage interaction.</summary>
    private Player player = null;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioManager script for playing troll sounds.</summary>
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
    /// Initializes references and loads attack data based on the enemy's tag.
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

        // Get the list of attacks based on this enemy's tag
        trollAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set a default attack (e.g., "attackHand")
        currentAttack = trollAttacks.Find(attack => attack.attackName == "attackHand");
        attackPlacePick(0);
    }

    /// <summary>
    /// Called every frame to manage attack logic and cooldowns.
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
    /// Finds the GameObject (Transform) where the attack should originate from.
    /// </summary>
    private void attackPlacePick(int index)
    {
        currentAttackPlace = attackPlace[index];
    }
    #endregion

    #region Hit Detection
    /// <summary>
    /// Checks if the current attack hits the player and handles cooldown logic.
    /// </summary>
    /// <returns>True if a hit is detected this frame.</returns>
    private bool hitCheck()
    {
        // Only detect hits if attacking and the animation is playing
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect colliders in range of the attack origin
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Register the hit and start cooldown
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
    /// Plays the troll attack sound effect.
    /// </summary>
    private void playAttackSound()
    {
        if (attackCount <= 0)
        {
            audioManager.playEnemy(audioSource, "Troll_Attack");
        }
    }
    #endregion

    #region Attack Animation Control
    /// <summary>
    /// Checks whether the current attack animation is playing.
    /// </summary>
    /// <returns>True if the animation for the current attack is active.</returns>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Picks the appropriate attack from the list based on current state.
    /// </summary>
    private void attackPick()
    {
        // If attack count is reset, choose default attack
        if (attackCount <= 0)
        {
            currentAttack = trollAttacks.Find(attack => attack.attackName == "attackHand");
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

    /// <summary>Gets the duration (cooldown) of the current attack.</summary>
    /// <returns>The duration of the current attack.</returns>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the damage value of the current attack.</summary>
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
    /// <summary>Starts the Troll's attack animation.</summary>
    public void startAttackTroll()
    {
        ClearAudioSource();
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Troll's attack animation.</summary>
    public void endAttackTroll()
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
    // Enable this to see the attack range
    /// <summary>
    /// Draws gizmos in the scene view to visualize the attack range.
    /// </summary>
    // void OnDrawGizmos()
    // {
    //     if (currentAttackPlace == null || currentAttack == null)
    //         return;

    //     if (hitCheck())
    //     {
    //         Gizmos.color = Color.red; // Visual cue for hit
    //         Gizmos.DrawWireSphere(
    //             currentAttackPlace.transform.position,
    //             currentAttack.attackRadius
    //         );
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.green; // Idle/ready visualization
    //         Gizmos.DrawWireSphere(
    //             currentAttackPlace.transform.position,
    //             currentAttack.attackRadius
    //         );
    //     }
    // }
    #endregion
}
