using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FinalBoss_AttackControl : MonoBehaviour, EnemyAttackBehavior
{
    /// <summary>Flag indicating if the enemy is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the enemy is currently hitting.</summary>
    private bool isHitting = false;

    /// <summary>Counter for the number of attacks.</summary>
    private int attackCount = 0;

    /// <summary>Maximum number of attacks.</summary>
    private int attackCountMax = 1;

    /// <summary>The current attack data being used.</summary>
    private Attack currentAttack;

    /// <summary>GameObject marking the origin point of the current attack (e.g., paw or mouth).</summary>
    private GameObject currentAttackPlace;

    /// <summary>Reference to the EnemyAttackesConvert script that provides attack data.</summary>
    private EnemyAttackesConvert enemyAttackesConvert;

    /// <summary>List of attacks available to the Final Boss.</summary>
    private List<Attack> FinalBossAttacks;

    /// <summary>Reference to the Player script for interacting with the player.</summary>
    private Player player = null;

    /// <summary>Reference to the AudioSource component for playing attack sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing attack sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";

    private FinalBossControl finalBossControl;
    private FinalBoss_AnimationControl finalBoss_AnimationControl;

    /// <summary>
    /// Initializes components and loads attack data on start.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        enemyAttackesConvert = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<EnemyAttackesConvert>();

        FinalBossAttacks = enemyAttackesConvert.getEnemyAttacks(gameObject.tag);

        currentAttack = FinalBossAttacks.Find(attack => attack.attackName == "Final_Boss_attack");

        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();

        finalBossControl = GetComponent<FinalBossControl>();
        finalBoss_AnimationControl = GetComponent<FinalBoss_AnimationControl>();
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
        if (finalBossControl.getIsAttacking() && isAttackAnimationPlaying())
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
        if (attackCount <= 1)
        {
            audioManager.playEnemy(audioSource, "FinalBoss_Attack");
        }
    }

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
            currentAttack = FinalBossAttacks.Find(attack =>
                attack.attackName == "Final_Boss_attack"
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

    /// <summary>Starts the Bear's attack animation.</summary>
    public void startAttackFinalBoss()
    {
        isAttacking = true;
        playAttackSound();
    }

    /// <summary>Ends the Bear's attack animation.</summary>
    public void endAttackFinalBoss()
    {
        isHitting = false;
        isAttacking = false;
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    // // enable this to see the attack range in the editor
    // /// <summary>
    // /// Draws Gizmos in the editor to visualize the attack radius.
    // /// Red indicates a hit detected this frame, green otherwise.
    // /// </summary>
    // void OnDrawGizmos()
    // {
    //     Debug.Log("currentAttackPlace: " + currentAttackPlace);
    //     Debug.Log("currentAttack: " + currentAttack);
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
}
