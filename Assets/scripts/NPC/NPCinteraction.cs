using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCinteraction : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject sphere;
    private Transform playerTransform;
    private GameObject player;
    private Animator animator;
    private NPCnavigation NPCnavigation;
    private NavMeshAgent agent;
    private bool isOccupied;
  

    private playerScript ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        NPCnavigation = GetComponent<NPCnavigation>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float radius = 2f;
        isOccupied = Physics.CheckSphere(sphere.transform.position, radius, playerLayer);

        if (isOccupied)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, playerLayer);


            foreach (Collider hit in hits)
            {
                Debug.Log("Detected: " + hit.gameObject.name);

                playerTransform = hit.gameObject.transform;

                player = hit.gameObject;
                Debug.Log(player.gameObject.name);
                // Optional: do something only if not already handled

                ps = hit.GetComponent<playerScript>();

                if (ps != null && ps.getInteractingWith() == null)
                {
                    // Interact with player!
                    ps.setInteractingWith(this.gameObject);
                }

            }
            if (ps != null && ps.getInteractingWith() != null &&  ps.getInteractingWith().tag == this.gameObject.tag && ps.getInteractingWith().GetComponent<StartNpc>().GetNpcsInstance() == this.gameObject.GetComponent<StartNpc>().GetNpcsInstance())
            {
                toggleOffNavigation();

                Vector3 direction = playerTransform.position - transform.position;


                direction.y = 0;


                Quaternion rotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2);
            }
            
        }
        else
        {
            HandleExit();
        }






    }
    public void toggleOffNavigation()
    {
        NPCnavigation.setIsWandering(false);
        playerTransform = player.transform;
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool("isWalking", false);
        isOccupied = true;
        

    }

    private void HandleExit()
    {
        Debug.Log("left trigger");
     
        
         NPCnavigation.setIsWandering(true);
         if (player != null)
         {
            player = null;
         }
         if (playerTransform != null)
         {
            playerTransform = null;
         }
        agent.isStopped = false;
        isOccupied = false;

        
    }

    public GameObject getPlayer()
        { return player; }
}
