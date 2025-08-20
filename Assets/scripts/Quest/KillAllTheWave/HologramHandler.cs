using UnityEngine;

public class HologramHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject SphereHologramOut;

    private Quest currentQuest;

    private string playerTag = "Player";

    private QuestManager questManager;

    private string gameManagerTag = "GameManager";

    private bool isQuestIsGoToFinshAllTheWave = false;
    private bool isQuestCompleted = false;

    private void Awake()
    {
        questManager = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponentInChildren<QuestManager>();
    }

    private void Update()
    {
        if (isQuestCompleted || isQuestIsGoToFinshAllTheWave)
            return;
        checkThecurrentQuest();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isQuestCompleted)
        {
            if (isQuestIsGoToFinshAllTheWave)
            {
                (currentQuest as GoToFinshAllTheWave).CompleteQuest();
                isQuestCompleted = true;
                setSphereHologramOut(false);
                return;
            }
            setSphereHologramOut(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && !isQuestCompleted)
        {
            setSphereHologramOut(false);
        }
    }

    public void setSphereHologramOut(bool isActive)
    {
        SphereHologramOut.SetActive(isActive);
    }

    public void checkThecurrentQuest()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        currentQuest = player.getCurrentMainQuest();
        if (currentQuest is GoToFinshAllTheWave)
        {
            if (!isQuestIsGoToFinshAllTheWave)
            {
                setSphereHologramOut(false);
                isQuestIsGoToFinshAllTheWave = true;
            }
        }
    }
}
