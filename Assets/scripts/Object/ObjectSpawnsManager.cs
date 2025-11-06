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
        objectsToSpawn = new List<GameObject>();
        spawnCounts = new Dictionary<GameObject, int>();
        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(false);
            objectsToSpawn.Add(child.gameObject);
            spawnCounts.Add(child.gameObject, 0);
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
        spawnCounts[obj] = spawnCounts[obj] - 1;
        obj.SetActive(spawnCounts[obj] >= 0);
        spawnCounts[obj] = Mathf.Max(0, spawnCounts[obj]);
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
