using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfBossAttacking : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField]
    private float swingAttackRange;

    [SerializeField]
    private float swingAttackCooldown = 2f;

    [SerializeField]
    private float swingAttackDamage = 40f;

    [SerializeField]
    private bool swingAttackOnCooldown ;

    [SerializeField]
    private float roarAttackRange;

    [SerializeField]
    private float roarAttackCooldown = 5f;

    [SerializeField]
    private float roarAttackDamage = 10f;

    [SerializeField]
    private bool roarAttackOnCooldown ;

    [SerializeField]
    private float jumpAttackRange;

    [SerializeField]
    private float jumpAttackCooldown = 10f;

    [SerializeField]
    private float jumpAttackDamage = 20f;

    [SerializeField]
    private bool jumpAttackOnCooldown ;

    [SerializeField]
    private GameObject sphereRange;

    [SerializeField]
    private GameObject sphereSwingAttack;

    [SerializeField]
    private bool isAttacking = false;

    [SerializeField]
    private bool isHitting = false;



    private int [] attackRangeForStun =  {3,6};
    private Vector2 stunTime;

    private int attacksTillStun;
    private int countAttacks;
    private List<Attack> attacks;

    private Attack currentAttack;
    private HandCollisionObserver handCollisionObserver;
    private JumpAttackColliderObserver jumpAttackColliderObserver;
    private RoarCollideObserver roarColliderObserver;
    //lamba expressions for the sphere checks
  
    private List<string> possibleAttacks = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        handCollisionObserver = GetComponentInChildren<HandCollisionObserver>();
        jumpAttackColliderObserver = GetComponentInChildren<JumpAttackColliderObserver>();
        roarColliderObserver = GetComponentInChildren<RoarCollideObserver>();
        attacks = GameObject.FindObjectOfType<EnemyAttackesConvert>().getEnemyAttacks(gameObject.tag);
        player = GameObject.FindObjectOfType<StartPlayer>().gameObject;


        swingAttackRange = attacks.Find(attack => attack.attackName == "Swing").attackRange;
        swingAttackDamage = attacks.Find(attack => attack.attackName == "Swing").attackDamage;
        swingAttackOnCooldown = false;
        roarAttackRange = attacks.Find(attack => attack.attackName == "Roar").attackRange;
        roarAttackDamage = attacks.Find(attack => attack.attackName == "Roar").attackDamage;
        roarAttackOnCooldown = false;
        jumpAttackRange = attacks.Find(attack => attack.attackName == "JumpAttack").attackRange;
        jumpAttackDamage = attacks.Find(attack => attack.attackName == "JumpAttack").attackDamage;
        jumpAttackOnCooldown = false;
      

        stunTime = new Vector2(3,5);
        attacksTillStun = Random.Range(attackRangeForStun[0], attackRangeForStun[1]);
        countAttacks = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking || animator.GetBool("IsStun"))
        {
            return;
        }

        
        if (countAttacks >= attacksTillStun)
        {
                   
            StartCoroutine(StunTime());
            return;
                   
        }
       

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

       
        Debug.Log("playerInJumpAttackRange: " + playerInJumpAttackRange);

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

        if (possibleAttacks.Count > 0)
        {
            PerformRandomAttack();
        }
        else
        {
            activateNavMeshAgent();
        }
    }

    private void PerformRandomAttack()
    {
        if (possibleAttacks.Count > 0)
        {
            string randomAttack = possibleAttacks[Random.Range(0, possibleAttacks.Count)];

            if (randomAttack == "Swing" && !swingAttackOnCooldown)
            {
                //double check if player is in range
                if (
                    Physics.CheckSphere(
                        sphereSwingAttack.transform.position,
                        swingAttackRange,
                        LayerMask.GetMask("Player")
                    )
                )
                {
                    //attack player
                    animator.SetBool("Swing", true);
                    swingAttackOnCooldown = true;
                    navMeshAgent.enabled = false;
                    isAttacking = true;
                    currentAttack = attacks.Find(attack => attack.attackName == "Swing");
                    countAttacks++;
                }
            }
            else if (randomAttack == "Roar" && !roarAttackOnCooldown)
            {
                //double check if player is in range
                if (
                    Physics.CheckSphere(
                        sphereRange.transform.position,
                        roarAttackRange,
                        LayerMask.GetMask("Player")
                    )
                )
                {
                    //attack player
                    animator.SetBool("Roar", true);
                    roarAttackOnCooldown = true;
                    navMeshAgent.enabled = false;
                    isAttacking = true;
                    currentAttack = attacks.Find(attack => attack.attackName == "Roar");
                    countAttacks++;
                }
            }
            else if (randomAttack == "JumpAttack" && !jumpAttackOnCooldown)
            {

                Debug.Log("JumpAttack: " + Physics.CheckSphere(
                        sphereRange.transform.position,
                        jumpAttackRange,
                        LayerMask.GetMask("Player"))
                    + " \n"+ 
                    
                    "Roar Attack: " + Physics.CheckSphere(
                        sphereRange.transform.position,
                        roarAttackRange,
                        LayerMask.GetMask("Player")
                    ));
                //double check if player is in range and has no options other than jump attack
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
                   
                    //attack player
                    animator.SetBool("JumpAttack", true);
                    jumpAttackOnCooldown = true;
                    navMeshAgent.enabled = false;
                    isAttacking = true;
                    currentAttack = attacks.Find(attack => attack.attackName == "JumpAttack");
                    countAttacks++;
                }

            }
            else
            {
                isAttacking = false;
            }
        }

        possibleAttacks.Clear();
    }

    public void activateNavMeshAgent()
    {
        navMeshAgent.enabled = true;
    }

    public IEnumerator RoarAttackCooldown()
    {
        yield return new WaitForSeconds(roarAttackCooldown);
        roarAttackOnCooldown = false;
    }

    public IEnumerator JumpAttackCooldown()
    {
        yield return new WaitForSeconds(jumpAttackCooldown);
        jumpAttackOnCooldown = false;
    }

    public IEnumerator SwingAttackCooldown()
    {
        yield return new WaitForSeconds(swingAttackCooldown);
        swingAttackOnCooldown = false;
    }

    public IEnumerator StunTime()
    {
        player.GetComponent<PlayerAttackController>().SetCanDealDamage(false);
        animator.SetBool("IsStun", true);
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(Random.Range(stunTime.x, stunTime.y));
        navMeshAgent.enabled = true;
        animator.SetBool("IsStun", false);
        attacksTillStun = (int)Random.Range(attackRangeForStun[0], attackRangeForStun[1]);
        countAttacks = 0;
        player.GetComponent<PlayerAttackController>().SetCanDealDamage(true);
    }

    public void enableHandCollider()
    {
        handCollisionObserver.enableHandCollider();
    }

    public void enableJumpAttackCollider()
    {
        jumpAttackColliderObserver.enableJumpAttackCollider();
    }

    public void enableRoarCollider()
    {
        roarColliderObserver.enableCollider();
    }

    // Draw Gizmos to visualize attack ranges in Scene view
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

    public void setIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    public void setIsHitting(bool isHitting)
    {
        this.isHitting = isHitting;
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    public bool getIsHitting()
    {
        return isHitting;
    }

    public float getCurrentAttackDMG()
    {
        return currentAttack.attackDamage;
    }
}
