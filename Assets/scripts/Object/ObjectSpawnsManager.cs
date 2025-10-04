using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawnsManager : MonoBehaviour
{
    private List<GameObject> objectsToSpawn;
    private Dictionary<GameObject, int> spawnCounts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject child in this.gameObject.transform)
        {
            child.SetActive(false);
            objectsToSpawn.Add(child);
            spawnCounts.Add(child, 0);
        }
    }

    public void SpawnAccordingToQuest(Quest quest)
    {
        List<GameObject> spawns = new List<GameObject>();
        foreach (GameObject child in objectsToSpawn)
        {
            if (quest.QuestTarget.Contains(child.tag))
            {
                spawns.Add(child.gameObject);
            }
        }

        if (spawns.Count > 0)
        {
            GameObject objToSpawn = spawns[UnityEngine.Random.Range(0, spawns.Count)];
            //enable a random spawn
            if (!TryActivateObject(objToSpawn))
            {
                spawnCounts[objToSpawn] = spawnCounts[objToSpawn] + 1;
            }
        }
    }

    public void DeSpawnOjbect(GameObject obj)
    {
        obj.SetActive(spawnCounts[obj] > 0);
    }

    public bool TryActivateObject(GameObject obj)
    {
        if (obj.activeSelf == true)
        {
            return false; //failed to activate object due to it being already active
        }

        obj.SetActive(true);
        return true;
    }
}
