using UnityEngine;

public class TempleColliderObserver : MonoBehaviour
{
    private bool playerPassed = false;

   

    // Update is called once per frame
    void Update()
    {
        if (!playerPassed)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerPassed)
        {
            return;
        }

        Debug.Log(other.gameObject.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest());
        if (
            !(
                other.gameObject.tag == "Player"
                && other.gameObject.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                    is TempleKillAllGaurds
            )
        )
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = false;
            playerPassed = true;
        }
    }
}
