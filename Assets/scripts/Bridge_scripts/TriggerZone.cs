using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TriggerZone : MonoBehaviour
{
    [SerializeField]
    private Transform PointToMoveTo;
    private InputListener inputListener;

    private Animator animator;

    private NavMeshAgent navMeshAgent;

    private AudioManager audioManager;

    private string gameManagerTag = "GameManager";
    private string playerTag = "Player";

    private bool isMoving = false;

    private GameObject player;

    [SerializeField]
    private GameObject Portal; //10

    [SerializeField]
    private GameObject magicCircle; //30

    [SerializeField]
    private GameObject star; //20

    [SerializeField]
    private AudioSource audioSourcePortal;

    [SerializeField]
    private AudioSource audioSourceMagicCircle;

    [SerializeField]
    private AudioSource audioSourceStar;

    private SceneHandler sceneHandler;

    private bool isLoading = false;

    private void Awake()
    {
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();
        player = GameObject.FindGameObjectWithTag(playerTag);
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        sceneHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<SceneHandler>();
    }

    void Update()
    {
        if (isMoving && !navMeshAgent.pathPending)
        {
            animator.SetBool("isWalking", true);
            checkTheProgressToPlayTheEffect();
            followThePlayer();
        }
        if (
            isMoving
            && !navMeshAgent.pathPending
            && navMeshAgent.remainingDistance <= 0.1f
            && !isLoading
        )
        {
            animator.SetBool("isWalking", false);
            audioSourceStar.Stop();
            audioSourceMagicCircle.Stop();
            audioSourcePortal.Stop();
            BridgeHandler bridgeHandler = gameObject.transform.parent.GetComponent<BridgeHandler>();
            bridgeHandler.completeQuest();
            LoadScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isMoving)
        {
            animator = other.GetComponent<Animator>();
            if (inputListener != null)
            {
                other.gameObject.GetComponent<CharacterController>().enabled = false;

                inputListener.setCanMove(false);
                other.gameObject.AddComponent<NavMeshAgent>();
                navMeshAgent = other.gameObject.GetComponent<NavMeshAgent>();
                navMeshAgent.agentTypeID = -1372625422;
                navMeshAgent.speed = 1.7f;
                StartCoroutine(StartMovingNextFrame());
            }
        }
    }

    private IEnumerator StartMovingNextFrame()
    {
        yield return new WaitForSeconds(0.1f);
        MoveThePlayerToTheBridge();
    }

    private void MoveThePlayerToTheBridge()
    {
        navMeshAgent.SetDestination(PointToMoveTo.position);
        isMoving = true;
    }

    private void checkTheProgressToPlayTheEffect()
    {
        if (navMeshAgent.remainingDistance <= 8f && !Portal.activeSelf)
        {
            Portal.SetActive(true);
            audioManager.playSFX(audioSourcePortal, "Portal");
        }

        if (navMeshAgent.remainingDistance <= 20f && !star.activeSelf)
        {
            star.SetActive(true);
            audioManager.playSFX(audioSourceStar, "Star", true);
        }
        if (navMeshAgent.remainingDistance <= 25f && !magicCircle.activeSelf)
        {
            magicCircle.SetActive(true);
            audioManager.playSFX(audioSourceMagicCircle, "MagicCircle");
        }
    }

    private void followThePlayer()
    {
        if (magicCircle.activeSelf)
        {
            Vector3 pos = player.transform.position;
            pos.y += 0.5f;
            magicCircle.transform.position = pos;
        }

        if (star.activeSelf)
        {
            Vector3 pos = player.transform.position;
            pos.y += 0.5f;
            star.transform.position = pos;
        }
    }

    private void LoadScene()
    {
        if (!isLoading)
        {
            animator.SetBool("isWalking", false);
            isLoading = true;
            if (SceneManager.GetActiveScene().buildIndex == 2)
                sceneHandler.LoadScene(3);
            else
                sceneHandler.LoadScene(2);
        }
    }
}
