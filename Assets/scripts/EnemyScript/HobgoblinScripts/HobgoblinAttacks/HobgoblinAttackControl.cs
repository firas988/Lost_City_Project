using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Hobgoblin enemy,
/// including attack selection, hit detection, and cooldown management.
/// Implements the EnemyAttackBehavior interface.
/// </summary>
public class HobgoblinAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    /// <summary>Current number of attacks performed.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum attack count before reset.</summary>
    private int attackCountMax = 0;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject indicating the point where attack originates (e.g., hand).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the EnemyAttackesConvert script for attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to this Hobgoblin.</summary>
    private List<Attack> hobgoblinAttacks;

    /// <summary>Reference to the EnemyMovement script controlling movement.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the EnemyAnimatorControl script controlling animations.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script controlling player data.</summary>
    private Player player = null;

    private AudioManager audioManager;

    private AudioSource audioSource;

    /// <summary>
    /// Initializes components and retrieves the list of available attacks at start.
    /// </summary>
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();
        audioManager = FindAnyObjectByType<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        enemyAttackesConvert = FindAnyObjectByType<EnemyAttackesConvert>();

        // Load the list of attacks for this enemy based on its tag.
        hobgoblinAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set the default current attack.
        currentAttack = hobgoblinAttacks.Find(attack => attack.attackName == "attackHand");
    }

    /// <summary>
    /// Called every frame to update attack selection, origin, and hit detection.
    /// </summary>
    void Update()
    {
        attackPick(); // Selects the current attack.
        attackPlacePick(); // Finds the GameObject representing the attack origin.

        if (hitCheck())
        {
            dealDamage();
        }
    }

    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    private void dealDamage()
    {
        player.takeDamage(currentAttack.attackDamage);
    }

    /// <summary>
    /// Finds and assigns the GameObject that marks the origin point of the current attack.
    /// </summary>
    private void attackPlacePick()
    {
        currentAttackPlace = FindDeepChild(transform, currentAttack.attackName).gameObject;
    }

    /// <summary>
    /// Recursively searches for a child transform by name in the hierarchy.
    /// </summary>
    /// <param name="parent">The parent transform to start the search.</param>
    /// <param name="name">The name of the child to find.</param>
    /// <returns>The found Transform or null if not found.</returns>
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

    /// <summary>
    /// Checks if the current attack hits the player, handles attack cooldown and counts.
    /// </summary>
    /// <returns>True if a hit was detected during this frame.</returns>
    private bool hitCheck()
    {
        // Only check for hits if currently attacking and attack animation is playing.
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect colliders within the attack radius at the attack origin.
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                // If collider belongs to the player and cooldown is over.
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Reset cooldown and increase attack count.
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

    private void playAttackSound()
    {
        if (attackCount <= 0)
        {
            audioManager.playEnemy(audioSource, "HobGoblin_Attack");
        }
    }

    /// <summary>
    /// Returns true if the current attack animation is playing.
    /// </summary>
    public bool isAttackAnimationPlaying()
    {
        return enemyAnimatorControl.GetCurrentAnimationClipInfo() == currentAttack.attackName;
    }

    /// <summary>
    /// Picks the current attack based on the attack count.
    /// </summary>
    private void attackPick()
    {
        // Always use "attackHand" since there is no other attack in this class.
        if (attackCount <= 0)
        {
            currentAttack = hobgoblinAttacks.Find(attack => attack.attackName == "attackHand");
        }
    }

    /// <summary>Gets the name of the current attack.</summary>
    public string getAttackName()
    {
        return currentAttack.attackName;
    }

    /// <summary>Gets the range of the current attack.</summary>
    public float getAttackRange()
    {
        return currentAttack.attackRange;
    }

    /// <summary>Gets the attack duration/time.</summary>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the damage of the current attack.</summary>
    public float getAttackDamage()
    {
        return currentAttack.attackDamage;
    }

    /// <summary>Starts the Hobgoblin's attack animation.</summary>
    public void startAttackHobgoblin()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Hobgoblin's attack animation.</summary>
    public void endAttackHobgoblin()
    {
        isHitting = false;
        isAttacking = false;
    }

    // enable this to see the attack range in the editor
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
}
