using UnityEngine;

public class BridgeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject Hologram;

    void Update()
    {
        checkIfTheQuestIsGoToBridge();
    }

    private void checkIfTheQuestIsGoToBridge()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        Quest quest = player.getCurrentMainQuest();
        if (quest is GoToBridge)
        {
            Destroy(Hologram);
        }
    }
}
