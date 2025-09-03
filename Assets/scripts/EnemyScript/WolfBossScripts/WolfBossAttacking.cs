using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the attack behavior of the Wolf Boss enemy, managing multiple attack types
/// including swing, roar, and jump attacks with cooldowns and stun mechanics.
/// Integrates with collision observers and animation system for comprehensive boss combat.
/// </summary>
public class WolfBossAttacking : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>Reference to the Animator component for controlling attack animations.</summary>
    [SerializeField]
    private Animator animator;

    /// <summary>Reference to the player GameObject for targeting.</summary>
    [SerializeField]
    private GameObject player;

    /// <summary>Reference to the AudioManager script for playing boss sounds.</summary>
    [SerializeField]
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component for playing audio.</summary>
    [SerializeField]
    private AudioSource audioSource;

    /// <summary>Reference to the NavMeshAgent for movement control during attacks.</summary>
    [SerializeField]
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    #endregion

    #region Swing Attack Configuration
    /// <summary>Range within which the swing attack can hit the player.</summary>
    [SerializeField]
    private float swingAttackRange;

    /// <summary>Cooldown time for the swing attack in seconds.</summary>
    [SerializeField]
    private float swingAttackCooldown = 2f;

    /// <summary>Damage dealt by the swing attack.</summary>
    [SerializeField]
    private float swingAttackDamage = 40f;

    /// <summary>Flag indicating if the swing attack is currently on cooldown.</summary>
    [SerializeField]
    private bool swingAttackOnCooldown;
    #endregion

    #region Roar Attack Configuration
    /// <summary>Range within which the roar attack can affect the player.</summary>
    [SerializeField]
    private float roarAttackRange;

    /// <summary>Cooldown time for the roar attack in seconds.</summary>
    [SerializeField]
    private float roarAttackCooldown = 5f;

    /// <summary>Damage dealt by the roar attack.</summary>
    [SerializeField]
    private float roarAttackDamage = 10f;

    /// <summary>Flag indicating if the roar attack is currently on cooldown.</summary>
    [SerializeField]
    private bool roarAttackOnCooldown;
    #endregion

    #region Jump Attack Configuration
    /// <summary>Range within which the jump attack can hit the player.</summary>
    [SerializeField]
    private float jumpAttackRange;

    /// <summary>Cooldown time for the jump attack in seconds.</summary>
    [SerializeField]
    private float jumpAttackCooldown = 10f;

    /// <summary>Damage dealt by the jump attack.</summary>
    [SerializeField]
    private float jumpAttackDamage = 20f;

    /// <summary>Flag indicating if the jump attack is currently on cooldown.</summary>
    [SerializeField]
    private bool jumpAttackOnCooldown;
    #endregion

    #region Attack Range Spheres
    /// <summary>GameObject representing the general attack range sphere.</summary>
    [SerializeField]
    private GameObject sphereRange;

    /// <summary>GameObject representing the swing attack range sphere.</summary>
    [SerializeField]
    private GameObject sphereSwingAttack;
    #endregion

    #region Attack State Variables
    /// <summary>Flag indicating if the boss is currently performing an attack.</summary>
    [SerializeField]
    private bool isAttacking = false;

    /// <summary>Flag indicating if the boss is currently hitting the player.</summary>
    [SerializeField]
    private bool isHitting = false;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";

    /// <summary>Range of attacks before stun occurs [min, max].</summary>
    private int[] attackRangeForStun = { 3, 6 };

    /// <summary>Range of stun duration in seconds [min, max].</summary>
    private Vector2 stunTime;
    #endregion

    #region Attack Management
    /// <summary>Number of attacks required before stun occurs.</summary>
    private int attacksTillStun;

    /// <summary>Current count of attacks performed.</summary>
    private int countAttacks;

    /// <summary>List of available attacks from the attack database.</summary>
    private List<Attack> attacks;

    /// <summary>The currently selected attack being used.</summary>
    private Attack currentAttack;

    /// <summary>List of possible attacks that can be performed this frame.</summary>
    private List<string> possibleAttacks = new List<string>();
    #endregion

    #region Collision Observers
    /// <summary>Observer for hand collision during swing attacks.</summary>
    private HandCollisionObserver handCollisionObserver;

    /// <summary>Observer for jump attack collision detection.</summary>
    private JumpAttackColliderObserver jumpAttackColliderObserver;

    /// <summary>Observer for roar attack collision detection.</summary>
    private RoarCollideObserver roarColliderObserver;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components, attack data, and sets up initial configuration.
    /// </summary>
    void Start()
    {
        // Get required components
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        handCollisionObserver = GetComponentInChildren<HandCollisionObserver>();
        jumpAttackColliderObserver = GetComponentInChildren<JumpAttackColliderObserver>();
        roarColliderObserver = GetComponentInChildren<RoarCollideObserver>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Load attack data from database
        attacks = GameObject
            .FindAnyObjectByType<EnemyAttackesConvert>()
            .getEnemyAttacks(gameObject.tag);

        // Get audio components
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        // Initialize swing attack properties
        swingAttackRange = attacks.Find(attack => attack.attackName == "Swing").attackRange;
        swingAttackDamage = attacks.Find(attack => attack.attackName == "Swing").attackDamage;
        swingAttackOnCooldown = false;

        // Initialize roar attack properties
        roarAttackRange = attacks.Find(attack => attack.attackName == "Roar").attackRange;
        roarAttackDamage = attacks.Find(attack => attack.attackName == "Roar").attackDamage;
        roarAttackOnCooldown = false;

        // Initialize jump attack properties
        jumpAttackRange = attacks.Find(attack => attack.attackName == "JumpAttack").attackRange;
        jumpAttackDamage = attacks.Find(attack => attack.attackName == "JumpAttack").attackDamage;
        jumpAttackOnCooldown = false;

        // Initialize stun system
        stunTime = new Vector2(3, 5);
        attacksTillStun = Random.Range(attackRangeForStun[0], attackRangeForStun[1]);
        countAttacks = 0;
    }

    /// <summary>
    /// Called every frame to manage attack logic, range checking, and stun system.
    /// </summary>
    void Update()
    {
        // Exit if currently attacking or stunned
        if (isAttacking || animator.GetBool("IsStun"))
        {
            return;
        }

        // Check if stun should occur
        if (countAttacks >= attacksTillStun)
        {
            StartCoroutine(StunTime());
            return;
        }

        // Check player position relative to different attack ranges
        bool playerInSwingAttackRange = Physics.CheckSphere(
            sphereSwingAttack.transform.position,
            swingAttackRange,
            LayerMask.GetMask("Player")
        );
        bool playerInRoarAttackRange = Physics.CheckSphere(
            sphereRange.transform.position,
            roarAttackRange,
            LayerMask.GetMask("Player")
        );
        bool playerInJumpAttackRange = Physics.CheckSphere(
            sphereRange.transform.position,
            jumpAttackRange,
            LayerMask.GetMask("Player")
        );

        // Build list of possible attacks based on player position and cooldowns
        if (playerInSwingAttackRange && !swingAttackOnCooldown)
        {
            possibleAttacks.Add("Swing");
        }
        if (playerInRoarAttackRange && !roarAttackOnCooldown)
        {
            possibleAttacks.Add("Roar");
        }
        if (playerInJumpAttackRange)
        {
            possibleAttacks.Add("JumpAttack");
        }

        // Perform attack if any are available, otherwise move towards player
        if (possibleAttacks.Count > 0)
        {
            PerformRandomAttack();
        }
        else
        {
            activateNavMeshAgent();
        }
    }
    #endregion

    #region Attack Selection and Execution
    /// <summary>
    /// Randomly selects and executes one of the available attacks.
    /// </summary>
    private void PerformRandomAttack()
    {
        if (possibleAttacks.Count > 0)
        {
            // Select random attack from available options
            string randomAttack = possibleAttacks[Random.Range(0, possibleAttacks.Count)];

            // Execute selected attack with range verification
            if (randomAttack == "Swing" && !swingAttackOnCooldown)
            {
                doubleCheckSwingRange();
            }
            else if (randomAttack == "Roar" && !roarAttackOnCooldown)
            {
                doubleCheckRoarRange();
            }
            else if (randomAttack == "JumpAttack" && !jumpAttackOnCooldown)
            {
                doubleCheckBoltRange();
            }
            else
            {
                isAttacking = false;
            }
        }

        // Clear possible attacks for next frame
        possibleAttacks.Clear();
    }
    #endregion

    #region Attack Range Verification
    /// <summary>
    /// Double-checks if player is in swing attack range before executing.
    /// </summary>
    public void doubleCheckSwingRange()
    {
        // Double check if player is in range
        if (
            Physics.CheckSphere(
                sphereSwingAttack.transform.position,
                swingAttackRange,
                LayerMask.GetMask("Player")
            )
        )
        {
            // Execute swing attack
            animator.SetBool("Swing", true);
            swingAttackOnCooldown = true;
            navMeshAgent.enabled = false;
            isAttacking = true;
            currentAttack = attacks.Find(attack => attack.attackName == "Swing");
            countAttacks++;
        }
    }

    /// <summary>
    /// Double-checks if player is in roar attack range before executing.
    /// </summary>
    public void doubleCheckRoarRange()
    {
        // Double check if player is in range
        if (
            Physics.CheckSphere(
                sphereRange.transform.position,
                roarAttackRange,
                LayerMask.GetMask("Player")
            )
        )
        {
            // Execute roar attack
            animator.SetBool("Roar", true);
            roarAttackOnCooldown = true;
            navMeshAgent.enabled = false;
            isAttacking = true;
            currentAttack = attacks.Find(attack => attack.attackName == "Roar");
            countAttacks++;
        }
    }

    /// <summary>
    /// Double-checks if player is in jump attack range before executing.
    /// Only executes if no other attack options are available.
    /// </summary>
    public void doubleCheckBoltRange()
    {
        // Double check if player is in range and has no options other than jump attack
        if (
            Physics.CheckSphere(
                sphereRange.transform.position,
                jumpAttackRange,
                LayerMask.GetMask("Player")
            )
            && !Physics.CheckSphere(
                sphereRange.transform.position,
                roarAttackRange,
                LayerMask.GetMask("Player")
            )
        )
        {
            // Execute jump attack
            animator.SetBool("JumpAttack", true);
            jumpAttackOnCooldown = true;
            navMeshAgent.enabled = false;
            isAttacking = true;
            currentAttack = attacks.Find(attack => attack.attackName == "JumpAttack");
            countAttacks++;
        }
    }
    #endregion

    #region Movement Control
    /// <summary>
    /// Re-enables the NavMeshAgent for movement.
    /// </summary>
    public void activateNavMeshAgent()
    {
        navMeshAgent.enabled = true;
    }
    #endregion

    #region Cooldown Management
    /// <summary>
    /// Coroutine that manages the roar attack cooldown.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator RoarAttackCooldown()
    {
        yield return new WaitForSeconds(roarAttackCooldown);
        roarAttackOnCooldown = false;
    }

    /// <summary>
    /// Coroutine that manages the jump attack cooldown.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator JumpAttackCooldown()
    {
        yield return new WaitForSeconds(jumpAttackCooldown);
        jumpAttackOnCooldown = false;
    }

    /// <summary>
    /// Coroutine that manages the swing attack cooldown.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator SwingAttackCooldown()
    {
        yield return new WaitForSeconds(swingAttackCooldown);
        swingAttackOnCooldown = false;
    }
    #endregion

    #region Stun System
    /// <summary>
    /// Coroutine that handles the stun period after reaching attack limit.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator StunTime()
    {
        // Enable player damage dealing during stun
        player.GetComponent<PlayerAttackController>().SetCanDealDamage(true);

        // Set stun animation and disable movement
        animator.SetBool("IsStun", true);
        navMeshAgent.enabled = false;

        // Wait for random stun duration
        yield return new WaitForSeconds(Random.Range(stunTime.x, stunTime.y));

        // End stun and reset attack counter
        navMeshAgent.enabled = true;
        animator.SetBool("IsStun", false);
        attacksTillStun = (int)Random.Range(attackRangeForStun[0], attackRangeForStun[1]);
        countAttacks = 0;

        // Disable player damage dealing
        player.GetComponent<PlayerAttackController>().SetCanDealDamage(false);
    }
    #endregion

    #region Collider Management
    /// <summary>
    /// Enables the hand collider for swing attack hit detection.
    /// </summary>
    public void enableHandCollider()
    {
        handCollisionObserver.enableHandCollider();
    }

    /// <summary>
    /// Enables the jump attack collider for hit detection.
    /// </summary>
    public void enableJumpAttackCollider()
    {
        jumpAttackColliderObserver.enableJumpAttackCollider();
    }

    /// <summary>
    /// Enables the roar collider for area effect detection.
    /// </summary>
    public void enableRoarCollider()
    {
        roarColliderObserver.enableCollider();
    }
    #endregion

    #region Debug Visualization
    /// <summary>
    /// Draws Gizmos to visualize attack ranges in Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (sphereRange == null)
            return;

        // Draw Swing Attack Range (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sphereSwingAttack.transform.position, swingAttackRange);

        // Draw Roar Attack Range (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sphereRange.transform.position, roarAttackRange);

        // Draw Jump Attack Range (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(sphereRange.transform.position, jumpAttackRange);
    }
    #endregion

    #region Public Interface Methods
    /// <summary>Sets the attacking state.</summary>
    /// <param name="isAttacking">Whether the boss should be attacking.</param>
    public void setIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    /// <summary>Sets the hitting state.</summary>
    /// <param name="isHitting">Whether the boss should be hitting.</param>
    public void setIsHitting(bool isHitting)
    {
        this.isHitting = isHitting;
    }

    /// <summary>Gets the current attacking state.</summary>
    /// <returns>True if attacking; otherwise false.</returns>
    public bool getIsAttacking()
    {
        return isAttacking;
    }

    /// <summary>Gets the current hitting state.</summary>
    /// <returns>True if hitting; otherwise false.</returns>
    public bool getIsHitting()
    {
        return isHitting;
    }

    /// <summary>Gets the damage value of the current attack.</summary>
    /// <returns>The damage value of the current attack.</returns>
    public float getCurrentAttackDMG()
    {
        return currentAttack.attackDamage;
    }
    #endregion

    #region Audio Management
    /// <summary>
    /// Plays the roar attack sound effect.
    /// </summary>
    public void playRoarAttackSound()
    {
        audioManager.playEnemy(audioSource, "WolfBossRoar");
    }

    /// <summary>
    /// Plays the jump attack sound effect.
    /// </summary>
    public void playJumpAttackSound()
    {
        audioManager.playEnemy(audioSource, "WolfBossBolt");
    }

    /// <summary>
    /// Plays the swing attack sound effect.
    /// </summary>
    public void playSwingAttackSound()
    {
        audioManager.playEnemy(audioSource, "WolfBossSwing");
    }
    #endregion
}
