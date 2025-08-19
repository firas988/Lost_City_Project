using UnityEngine;

public class DungeonDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<StartPlayer>().getPlayer();

            if (player.getCurrentMainQuest() is DungeonLevel1)
            {
                GameObject
                    .FindWithTag("GameManager")
                    .GetComponentInChildren<SceneHandler>()
                    .LoadScene(4);
            }
        }
    }
}
