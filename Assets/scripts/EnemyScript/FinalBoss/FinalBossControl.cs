using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StartNpc))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FinalBoss_AnimationControl))]
[RequireComponent(typeof(Spawn_Drakonit_Handler))]
[RequireComponent(typeof(NPCnavigation))]
public class FinalBossControl : MonoBehaviour
{
    private StartNpc startNpc;
    private NavMeshAgent navMeshAgent;
    private Entity entity;
    private FinalBoss_AnimationControl finalBoss_AnimationControl;
    private FinalBoss_AttackControl finalBoss_AttackControl;
    private GameObject player;
    private string playerTag = "Player";
    private Spawn_Drakonit_Handler spawn_Drakonit_Handler;
    private NPCnavigation npcNavigation;

    private bool MoveToPlayerToAttack = false;
    private bool LookAtPlayer = false;

    private bool isAttacking = false;

    private bool canMove = true;
    private bool inCoroutine = false;
    private bool getHit = false;
    private Coroutine ControlBossStateCoroutine;

    private void Start()
    {
        startNpc = GetComponent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        player = GameObject.FindGameObjectWithTag(playerTag);
        navMeshAgent = GetComponent<NavMeshAgent>();
        finalBoss_AnimationControl = GetComponent<FinalBoss_AnimationControl>();
        finalBoss_AttackControl = GetComponent<FinalBoss_AttackControl>();
        spawn_Drakonit_Handler = GetComponent<Spawn_Drakonit_Handler>();
        npcNavigation = GetComponent<NPCnavigation>();
    }

    private void Update()
    {
        controlBossState();

        if (LookAtPlayer)
        {
            lookAtPlayer();
        }
        if (!canMove)
        {
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

        if (MoveToPlayerToAttack)
        {
            npcNavigation.setIsWandering(false);
            moveToPlayerToAttack();
        }
        else
        {
            npcNavigation.itWalk();
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     entity.setHealth(0);
        // }
    }

    private void controlBossState()
    {
        if (ControlBossStateCoroutine != null && getHit)
        {
            inCoroutine = false;
            MoveToPlayerToAttack = false;
            StopCoroutine(ControlBossStateCoroutine);
            finalBoss_AnimationControl.setAllBooleanParamToFalse("getHit");
            finalBoss_AnimationControl.startGetHitAnimation();
            ControlBossStateCoroutine = null;
        }
        if (!inCoroutine)
        {
            inCoroutine = true;
            ControlBossStateCoroutine = StartCoroutine(controlBossStateCoroutine());
        }
    }

    public void setGetHit(bool getHit)
    {
        this.getHit = getHit;
    }

    private IEnumerator controlBossStateCoroutine()
    {
        if (!getHit)
        {
            npcNavigation.setIsWandering(true);
            yield return new WaitForSeconds(10f);
        }
        else
        {
            getHit = false;
            canMove = false;
            yield return new WaitForSeconds(4f);
            canMove = true;
            npcNavigation.setIsWandering(true);
            yield return new WaitForSeconds(8f);
        }
        MoveToPlayerToAttack = true;
        yield return new WaitForSeconds(Random.Range(30f, 60f));
        MoveToPlayerToAttack = false;
        yield return new WaitForSeconds(0.8f);

        if (!spawn_Drakonit_Handler.getIsEnemiesSpawned())
        {
            spawnEnemy(10);
            canMove = false;
            yield return new WaitForSeconds(5f);
            canMove = true;
        }
        inCoroutine = false;
    }

    private void lookAtPlayer()
    {
        transform.LookAt(player.transform);
    }

    private void moveToPlayerToAttack()
    {
        if (getHit)
        {
            return;
        }
        npcNavigation.itRun();
        navMeshAgent.SetDestination(player.transform.position);

        // Measure distance to player.
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Get direction and angle between enemy and player.
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        float visionAngle = 100f; // Field of view angle.

        // If within attack range and FOV and not returning, start attacking.
        if (distance <= finalBoss_AttackControl.getAttackRange() && angle <= visionAngle)
        {
            isAttacking = true;

            // Stop movement while attacking.
            navMeshAgent.SetDestination(transform.position);
        }
        else if (isAttacking)
        {
            // Stop attacking if player moved out of range or view.
            isAttacking = false;
        }

        // Lock position if animation is still playing (e.g., bite animation).
        if (finalBoss_AttackControl.isAttackAnimationPlaying())
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }

    private void spawnEnemy(int numberOfEnemiesToSpawn)
    {
        finalBoss_AnimationControl.startSummoningAnimation();
        spawn_Drakonit_Handler.startSpawnEnemies(numberOfEnemiesToSpawn, 10f);
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public bool getMoveToPlayer()
    {
        return MoveToPlayerToAttack;
    }

    public NPC getNpcsInstance()
    {
        return startNpc.GetNpcsInstance();
    }
}
