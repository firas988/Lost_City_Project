using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls attack logic for the Troll enemy,
/// including picking attacks, hit detection, and cooldown handling.
/// Implements the EnemyAttackBehavior interface for compatibility.
/// </summary>
public class TrollAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    /// <summary>Counter for how many times the troll has attacked.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum allowed attack count before reset.</summary>
    private int attackCountMax = 0;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>Currently selected attack from the list.</summary>
    private Attack currentAttack;

    /// <summary>GameObject representing the attack origin (e.g., hand, weapon).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Script that provides attack data for enemies.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to this enemy.</summary>
    private List<Attack> trollAttacks;

    /// <summary>Reference to the movement controller of the enemy.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the animation controller of the enemy.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    private Player player = null;

    /// <summary>
    /// Initializes references and loads attack data based on the enemy's tag.
    /// </summary>
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();

        enemyAttackesConvert = FindAnyObjectByType<EnemyAttackesConvert>();

        // Get the list of attacks based on this enemy's tag.
        trollAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        // Set a default attack (e.g., "attackHand").
        currentAttack = trollAttacks.Find(attack => attack.attackName == "attackHand");
    }

    /// <summary>
    /// Called every frame to manage attack logic and cooldowns.
    /// </summary>
    void Update()
    {
        attackPick(); // Selects appropriate attack if needed.
        attackPlacePick(); // Finds attack origin point (e.g., hand).

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
    /// Finds the GameObject (Transform) where the attack should originate from.
    /// </summary>
    private void attackPlacePick()
    {
        currentAttackPlace = FindDeepChild(transform, currentAttack.attackName).gameObject;
    }

    /// <summary>
    /// Recursively searches for a child transform by name.
    /// </summary>
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
    /// Checks if the current attack hits the player and handles cooldown logic.
    /// </summary>
    /// <returns>True if a hit is detected this frame.</returns>
    private bool hitCheck()
    {
        // Only detect hits if attacking and the animation is playing.
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            // Detect colliders in range of the attack origin.
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    // Register the hit and start cooldown.
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
        // If attack count is reset, choose default attack.
        if (attackCount <= 0)
        {
            currentAttack = trollAttacks.Find(attack => attack.attackName == "attackHand");
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

    /// <summary>Gets the duration (cooldown) of the current attack.</summary>
    public float getAttackTime()
    {
        return currentAttack.attackTime;
    }

    /// <summary>Gets the damage value of the current attack.</summary>
    public float getAttackDamage()
    {
        return currentAttack.attackDamage;
    }

    /// <summary>Starts the Troll's attack animation.</summary>
    public void startAttackTroll()
    {
        isAttacking = true;
    }

    /// <summary>Ends the Troll's attack animation.</summary>
    public void endAttackTroll()
    {
        isHitting = false;
        isAttacking = false;
    }

    ///enable this to see the attack range
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
}
