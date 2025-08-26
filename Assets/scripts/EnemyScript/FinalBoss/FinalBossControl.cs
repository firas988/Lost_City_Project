using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the main behavior of the Final Boss enemy,
/// managing movement, state transitions, attack coordination, and enemy spawning.
/// Integrates with multiple components for comprehensive boss AI behavior.
/// </summary>
[RequireComponent(typeof(StartNpc))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FinalBoss_AnimationControl))]
[RequireComponent(typeof(Spawn_Drakonit_Handler))]
[RequireComponent(typeof(NPCnavigation))]
public class FinalBossControl : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the StartNpc script for NPC instance management.</summary>
    private StartNpc startNpc;

    /// <summary>Reference to the NavMeshAgent for AI navigation.</summary>
    private NavMeshAgent navMeshAgent;

    /// <summary>Reference to the Entity component for health and status management.</summary>
    private Entity entity;

    /// <summary>Reference to the FinalBoss_AnimationControl script for animation management.</summary>
    private FinalBoss_AnimationControl finalBoss_AnimationControl;

    /// <summary>Reference to the FinalBoss_AttackControl script for attack management.</summary>
    private FinalBoss_AttackControl finalBoss_AttackControl;

    /// <summary>Reference to the Spawn_Drakonit_Handler script for enemy spawning.</summary>
    private Spawn_Drakonit_Handler spawn_Drakonit_Handler;

    /// <summary>Reference to the NPCnavigation script for movement behavior.</summary>
    private NPCnavigation npcNavigation;

    /// <summary>Reference to the BossBarHandler for UI health bar management.</summary>
    private BossBarHandler bossBarHandler;

    /// <summary>Reference to the AudioSource component for playing boss sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing boss sounds.</summary>
    private AudioManager audioManager;
    #endregion

    #region Target References
    /// <summary>Reference to the player GameObject for targeting and interaction.</summary>
    private GameObject player;
    #endregion

    #region Configuration
    /// <summary>Tag for the Player object.</summary>
    private string playerTag = "Player";

    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";

    /// <summary>Damage amount for dash-to-target attacks.</summary>
    private float dashToTargetDamage = 100f;
    #endregion

    #region Movement State Variables
    /// <summary>Flag indicating if the boss should move to player for attack.</summary>
    private bool MoveToPlayerToAttack = false;

    /// <summary>Flag indicating if the boss should look at the player.</summary>
    private bool LookAtPlayer = false;

    /// <summary>Flag indicating if the boss can currently move.</summary>
    private bool canMove = true;
    #endregion

    #region Combat State Variables
    /// <summary>Flag indicating if the boss is currently attacking.</summary>
    private bool isAttacking = false;

    /// <summary>Flag indicating if the boss has been hit.</summary>
    private bool getHit = false;
    #endregion

    #region Coroutine Management
    /// <summary>Flag indicating if a coroutine is currently running.</summary>
    private bool inCoroutine = false;

    /// <summary>Reference to the main boss state control coroutine.</summary>
    private Coroutine ControlBossStateCoroutine;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components and sets up references on start.
    /// </summary>
    private void Start()
    {
        // Get required components
        startNpc = GetComponent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        player = GameObject.FindGameObjectWithTag(playerTag);
        navMeshAgent = GetComponent<NavMeshAgent>();
        finalBoss_AnimationControl = GetComponent<FinalBoss_AnimationControl>();
        finalBoss_AttackControl = GetComponent<FinalBoss_AttackControl>();
        spawn_Drakonit_Handler = GetComponent<Spawn_Drakonit_Handler>();
        npcNavigation = GetComponent<NPCnavigation>();

        // Find and store boss bar handler
        bossBarHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<BossBarHandler>();

        // Get audio components
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<AudioManager>();
    }

    /// <summary>
    /// Called every frame to update boss behavior and state management.
    /// </summary>
    private void Update()
    {
        // Ensure player reference is valid
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
        }

        // Ensure boss bar handler reference is valid
        if (bossBarHandler == null)
        {
            bossBarHandler = GameObject
                .FindGameObjectWithTag(gameManagerTag)
                .transform.parent.GetComponentInChildren<BossBarHandler>();
        }

        // Control boss state transitions
        controlBossState();

        // Handle looking at player when needed
        if (LookAtPlayer)
        {
            lookAtPlayer();
        }

        // Handle movement restrictions
        if (!canMove)
        {
            // Stop wandering and movement, look at player if alive
            npcNavigation.setIsWandering(false);
            navMeshAgent.SetDestination(transform.position);
            finalBoss_AnimationControl.setAllBooleanParamToFalse("summon");
            if (!entity.isDead())
            {
                LookAtPlayer = true;
            }
            else
            {
                LookAtPlayer = false;
            }
            return;
        }
        else
        {
            LookAtPlayer = false;
        }

        // Handle movement to player for attack
        if (MoveToPlayerToAttack)
        {
            npcNavigation.setIsWandering(false);
            moveToPlayerToAttack();
        }
        else
        {
            // Allow normal wandering behavior
            npcNavigation.itWalk();
        }

        // Debug key for testing (set health to 0)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.setHealth(0);
        }
    }
    #endregion

    #region Boss State Management
    /// <summary>
    /// Controls the main boss state transitions and coroutine management.
    /// </summary>
    private void controlBossState()
    {
        // Handle hit interruption
        if (ControlBossStateCoroutine != null && getHit)
        {
            inCoroutine = false;
            MoveToPlayerToAttack = false;
            StopCoroutine(ControlBossStateCoroutine);
            finalBoss_AnimationControl.setAllBooleanParamToFalse("getHit");
            finalBoss_AnimationControl.startGetHitAnimation();
            ControlBossStateCoroutine = null;
        }

        // Start new state cycle if not in coroutine
        if (!inCoroutine)
        {
            inCoroutine = true;
            ControlBossStateCoroutine = StartCoroutine(controlBossStateCoroutine());
        }
    }

    /// <summary>
    /// Sets the hit state and handles damage application.
    /// </summary>
    /// <param name="getHit">Whether the boss has been hit.</param>
    public void setGetHit(bool getHit)
    {
        this.getHit = getHit;
        if (getHit)
        {
            // Apply damage and update boss bar
            entity.takeDamage(dashToTargetDamage);
            bossBarHandler.TakeDamage(entity.getHealth() / entity.getMaxHealth());
        }
    }

    /// <summary>
    /// Main coroutine that controls the boss behavior cycle.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator controlBossStateCoroutine()
    {
        if (!getHit)
        {
            // Normal wandering phase
            npcNavigation.setIsWandering(true);
            yield return new WaitForSeconds(10f);
        }
        else
        {
            // Hit recovery phase
            getHit = false;
            canMove = false;
            yield return new WaitForSeconds(4f);
            canMove = true;
            npcNavigation.setIsWandering(true);
            yield return new WaitForSeconds(8f);
        }

        // Attack phase
        MoveToPlayerToAttack = true;
        yield return new WaitForSeconds(Random.Range(10f, 30f));
        MoveToPlayerToAttack = false;
        yield return new WaitForSeconds(0.8f);

        // Enemy spawning phase
        if (!spawn_Drakonit_Handler.getIsEnemiesSpawned() && !entity.isDead())
        {
            spawnEnemy(10);
            canMove = false;
            yield return new WaitForSeconds(5f);
            canMove = true;
        }
        else if (entity.isDead())
        {
            // Kill all spawned enemies if boss is dead
            spawn_Drakonit_Handler.killAllEnemies();
        }

        inCoroutine = false;
    }
    #endregion

    #region Movement and Targeting
    /// <summary>
    /// Makes the boss look at the player's position.</summary>
    private void lookAtPlayer()
    {
        transform.LookAt(player.transform);
    }

    /// <summary>
    /// Moves the boss toward the player for attack, with attack range detection.
    /// </summary>
    private void moveToPlayerToAttack()
    {
        if (getHit)
        {
            return;
        }

        // Set running animation and move to player
        npcNavigation.itRun();
        navMeshAgent.SetDestination(player.transform.position);

        // Measure distance to player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Get direction and angle between enemy and player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        float visionAngle = 100f; // Field of view angle

        // If within attack range and FOV and not returning, start attacking
        if (distance <= finalBoss_AttackControl.getAttackRange() && angle <= visionAngle)
        {
            isAttacking = true;

            // Stop movement while attacking
            navMeshAgent.SetDestination(transform.position);
        }
        else if (isAttacking)
        {
            // Stop attacking if player moved out of range or view
            isAttacking = false;
        }

        // Lock position if animation is still playing (e.g., bite animation)
        if (finalBoss_AttackControl.isAttackAnimationPlaying())
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
    #endregion

    #region Enemy Spawning
    /// <summary>
    /// Spawns enemies and plays summoning effects.
    /// </summary>
    /// <param name="numberOfEnemiesToSpawn">Number of enemies to spawn.</param>
    private void spawnEnemy(int numberOfEnemiesToSpawn)
    {
        // Play summoning sound and animation
        audioManager.playEnemy(audioSource, "Summon_Enemy_Final_Boss");
        finalBoss_AnimationControl.startSummoningAnimation();

        // Start enemy spawning process
        spawn_Drakonit_Handler.startSpawnEnemies(numberOfEnemiesToSpawn, 10f);
    }
    #endregion

    #region Public Interface Methods
    /// <summary>Gets the current attacking state.</summary>
    /// <returns>True if the boss is attacking; otherwise false.</returns>
    public bool getIsAttacking()
    {
        return isAttacking;
    }

    /// <summary>Sets whether the boss can move.</summary>
    /// <param name="canMove">Whether movement is allowed.</param>
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    /// <summary>Gets whether the boss is moving to player for attack.</summary>
    /// <returns>True if moving to attack; otherwise false.</returns>
    public bool getMoveToPlayer()
    {
        return MoveToPlayerToAttack;
    }

    /// <summary>Gets the NPC entity instance.</summary>
    /// <returns>Reference to the Entity component.</returns>
    public Entity getNpcsInstance()
    {
        return entity;
    }
    #endregion
}
