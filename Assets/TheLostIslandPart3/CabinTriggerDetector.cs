using UnityEngine;

public class CabinTriggerDetector : MonoBehaviour
{
    private GameObject hint;
    private QuestManager questManager;
    private string gameManagerTag = "GameManager";

    void Start()
    {
        hint = transform.Find("Hint").gameObject;
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        this.enabled = !questManager.checkingCompletedStoryQuest(typeof(FindMapPartInCabin));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<StartPlayer>().getPlayer();
            if (player.getCurrentMainQuest() is FindMapPartInCabin)
            {
                GameObject door = gameObject.transform.Find("Door").gameObject;
                door.GetComponent<Animator>().SetBool("IsClosed", true);
                Debug.Log(hint);
                (player.getCurrentMainQuest() as FindMapPartInCabin).setHint(hint);
                Debug.Log((player.getCurrentMainQuest() as FindMapPartInCabin).getHint());
                BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
                boxCollider.enabled = false;
            }
        }
    }
}
