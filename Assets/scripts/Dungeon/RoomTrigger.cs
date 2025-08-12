using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private DungeonManager dungeonManager;

    void Start()
    {
        dungeonManager = GameObject.Find("dungeon").GetComponent<DungeonManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            dungeonManager.StartDungeon();
        }
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
