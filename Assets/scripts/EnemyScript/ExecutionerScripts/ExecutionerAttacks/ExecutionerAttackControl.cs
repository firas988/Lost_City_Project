using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the attack behavior of the Executioner enemy,
/// managing attack selection, hit detection, and cooldowns.
/// Implements the EnemyAttackBehavior interface.
/// </summary>
public class ExecutionerAttackControl : MonoBehaviour, EnemyAttackBehavior
{
    /// <summary>Current number of attacks performed.</summary>
    [SerializeField]
    private int attackCount = 0;

    /// <summary>Maximum attack count before reset.</summary>
    private int attackCountMax = 3;

    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject marking the origin point of the current attack (e.g., sword).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the EnemyAttackesConvert script that provides attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to the Executioner.</summary>
    private List<Attack> executionerAttacks;

    /// <summary>Reference to the EnemyMovement script for movement control.</summary>
    private EnemyMovement enemyMovement;

    /// <summary>Reference to the EnemyAnimatorControl script for animation control.</summary>
    private EnemyAnimatorControl enemyAnimatorControl;

    /// <summary>Reference to the Player script controlling player data.</summary>
    private Player player = null;

    /// <summary>Reference to the AudioSource component for playing attack sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing attack sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Flag indicating if the attack rotation animation is playing.</summary>
    private bool isAttackRotationPlaying = false;

    /// <summary>
    /// Initializes components and loads attack data on start.
    /// </summary>
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimatorControl = GetComponent<EnemyAnimatorControl>();
        audioSource = GetComponent<AudioSource>();
        audioManager = FindAnyObjectByType<AudioManager>();
        enemyAttackesConvert = FindAnyObjectByType<EnemyAttackesConvert>();

        executionerAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        currentAttack = executionerAttacks.Find(attack =>
            attack.attackName == "AttackOneHandSword"
        );
    }

    /// <summary>
    /// Called every frame to update attack choice, attack origin, and hit detection.
    /// </summary>
    void Update()
    {
        attackPick();
        attackPlacePick();

        if (hitCheck())
        {
            dealDamage();
        }
    }

    private void playAttackSound()
    {
        if (attackCount <= 2)
        {
            audioManager.playEnemy(audioSource, "Executioner_AttackOneHandSword");
        }
        else if (attackCount == 3)
        {
            audioManager.playEnemy(audioSource, "Executioner_AttackOneHandSwordRotation");
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

    /// <summary>
    /// Checks for hit detection on the player within attack radius and handles cooldown.
    /// </summary>
    /// <returns>True if a hit was detected this frame; otherwise false.</returns>
    private bool hitCheck()
    {
        if (enemyMovement.getIsAttacking() && isAttackAnimationPlaying())
        {
            Collider[] hitColliders = Physics.OverlapSphere(
                currentAttackPlace.transform.position,
                currentAttack.attackRadius
            );

            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player") && isAttacking && !isHitting)
                {
                    isHitting = true;
                    if (player == null)
                    {
                        player = col.GetComponent<StartPlayer>().getPlayer();
                    }
                    // Increment attack count and reset if exceeded max.
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
        if (attackCount <= 2)
        {
            currentAttack = executionerAttacks.Find(attack =>
                attack.attackName == "AttackOneHandSword"
            );
        }
        else if (attackCount == 3)
        {
            currentAttack = executionerAttacks.Find(attack =>
                attack.attackName == "AttackOneHandSwordRotation"
            );
        }
    }

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

    /// <summary>Starts the Executioner's attack animation.</summary>
    public void startAttackExecutioner()
    {
        isAttacking = true;
        if (!isAttackRotationPlaying)
        {
            playAttackSound();
        }
        if (attackCount == 3)
        {
            isAttackRotationPlaying = true;
        }
        else
        {
            isAttackRotationPlaying = false;
        }
    }

    /// <summary>Ends the Executioner's attack animation.</summary>
    public void endAttackExecutioner()
    {
        isHitting = false;
        isAttacking = false;
    }

    // enable this to see the attack range in the editor
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
}
