using System.Collections;
using UnityEngine;

public class ItemToFindTopenTheMiddel_Hnadler : MonoBehaviour
{
    [SerializeField]
    private GameObject crystal;

    [SerializeField]
    private GameObject hologram;

    private QuestManager questManager;
    private ObjectInteraction objectInteraction;

    private bool isMovingUp = false;
    private Vector3 targetPosition;

    private string playerTag = "Player";
    private string gameManagerTag = "GameManager";
    private bool isQuestIsGoToActivateTheKey = false;
    private bool isQuestIsActivateTheKey = false;
    private bool isQuestIsCompleted = false;
    private Quest currentQuest;

    void Start()
    {
        crystal.SetActive(false);
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        objectInteraction = GetComponent<ObjectInteraction>();
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    void Update()
    {
        if (isQuestIsCompleted)
        {
            return;
        }
        moveUp();
        checkIfTheQuestIsGoToActivateTheKey();
        checkIfTheQuestIsActivateTheKey();
        if (currentQuest == null)
        {
            return;
        }
    }

    public void moveUp()
    {
        if (isMovingUp)
        {
            crystal.transform.position = Vector3.Lerp(
                crystal.transform.position,
                targetPosition,
                Time.deltaTime * 1
            );

            if (Vector3.Distance(crystal.transform.position, targetPosition) < 0.01f)
            {
                crystal.transform.position = targetPosition;
                isMovingUp = false;
                if (
                    currentQuest is ActivateTheKey
                    && isQuestIsActivateTheKey
                    && !currentQuest.isCompleted
                )
                {
                    (currentQuest as ActivateTheKey).CompleteQuest();
                    isQuestIsActivateTheKey = false;
                }
                isQuestIsCompleted = true;
            }
        }
    }

    public void foundIT()
    {
        if (!isQuestIsActivateTheKey || isMovingUp)
        {
            return;
        }
        crystal.SetActive(true);
        targetPosition = new Vector3(
            crystal.transform.position.x,
            crystal.transform.position.y + 15f,
            crystal.transform.position.z
        );
        isMovingUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && isQuestIsGoToActivateTheKey && !currentQuest.isCompleted)
        {
            if (currentQuest is GoToActivateTheKey)
            {
                (currentQuest as GoToActivateTheKey).CompleteQuest();
                isQuestIsGoToActivateTheKey = false;
            }
        }
    }

    public void checkIfTheQuestIsGoToActivateTheKey()
    {
        Player player = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();
        if (currentQuest is GoToActivateTheKey)
        {
            if (!isQuestIsGoToActivateTheKey)
            {
                isQuestIsGoToActivateTheKey = true;
                if (hologram != null)
                {
                    hologram.SetActive(false);
                    Destroy(hologram);
                }
            }
        }
    }

    public void checkIfTheQuestIsActivateTheKey()
    {
        Player player = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();
        if (currentQuest is ActivateTheKey)
        {
            if (!isQuestIsActivateTheKey)
            {
                isQuestIsActivateTheKey = true;
            }
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);
            }
        }
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);
        checkIfTheQuestIsGoToActivateTheKeyIsCompleted();
        checkIfTheQuestIsActivateTheKeyIsCompleted();
    }

    public bool checkIfTheQuestIsGoToActivateTheKeyIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToActivateTheKey)))
        {
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);
            }
            return true;
        }
        return false;
    }

    public bool checkIfTheQuestIsActivateTheKeyIsCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(ActivateTheKey)))
        {
            if (hologram != null)
            {
                hologram.SetActive(false);
                Destroy(hologram);
                isQuestIsActivateTheKey = true;
                foundIT();
                objectInteraction.setIsFinshed(true);
                objectInteraction.hideCanvas();
            }
            return true;
        }
        return false;
    }
}
