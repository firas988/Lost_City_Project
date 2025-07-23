using System.Collections;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckDistance = 1f;
    private int currentWaypointIndex = 0;
    private PlayerController playerController;

    private Animator animator;


    private bool isMoving = false;
    private float threshold = 0.2f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            playerController = other.GetComponent<PlayerController>();
            animator = other.GetComponent<Animator>();
            if (playerController != null)
            {
                animator.SetBool("isWalking", true); // Set the animation trigger
                playerController.enabled = false; 
                StartCoroutine(MoveAlongPath(other.transform));
            }
        }
    }

    private IEnumerator MoveAlongPath(Transform player)
    {
        isMoving = true;

        while (currentWaypointIndex < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            while (Vector3.Distance(player.position, targetPosition) > threshold)
            {
                Vector3 moveDirection = (targetPosition - player.position).normalized;
                Vector3 newPosition = player.position + moveDirection * moveSpeed * Time.deltaTime;

                float groundY = newPosition.y;
                if (Mathf.Abs(newPosition.y - groundY) < groundCheckDistance)
                {
                    newPosition.y = groundY;
                }

                player.position = newPosition;

                RotateTowardsTarget(player, targetPosition);

                yield return null;
            }

            currentWaypointIndex++;
        }

        playerController.enabled = true;
        isMoving = false;
        animator.SetBool("isWalking", false); // Set the animation trigger

        currentWaypointIndex = 0;
    }

    private void RotateTowardsTarget(Transform player, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - player.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private float GetGroundY(Vector3 position)
    {
        //RaycastHit hit;
        //if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit, groundCheckDistance, groundLayer))
        //{
        //    Debug.Log(hit.point.y);
        //    return hit.point.y;
        //}

        return position.y;
    }

    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
