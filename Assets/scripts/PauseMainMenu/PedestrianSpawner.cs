using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject[] spawnPoints;

    [SerializeField]
    private string prefabPath;

    private GameObject [] prefab;
    private int spawnCount = 0;

    private int robertCount = 0;

    private bool waitForSpawn=false;



    void Start()
    {
       prefab = Resources.LoadAll<GameObject>(prefabPath);

    }

    // Update is called once per frame
    void Update()
    {
        if (waitForSpawn)
            return;

        if (spawnCount == 30)
            return;
            
      StartCoroutine(SpawnPedestrians());
    }


   private IEnumerator SpawnPedestrians()
    {
       
            GameObject spawnPoint = spawnPoints[getRandomNumber(0, spawnPoints.Length)];
            NavMeshHit hit;
            GameObject pedestrian = Instantiate(prefab[getRandomNumber(0, prefab.Length)], spawnPoint.transform.position, Quaternion.identity);
            if ( pedestrian.tag == "Robert" && robertCount >= 1)
            {
                Destroy(pedestrian);
                yield return null;
            }
            if (NavMesh.SamplePosition(pedestrian.transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                pedestrian.transform.position = hit.position;
                spawnCount++;
                if (pedestrian.name == "Robert")
                {
                    robertCount++;
                }
            }
        waitForSpawn = true;
            yield return new WaitForSeconds(1.0f); // Wait 1 second between spawns
        waitForSpawn = false;
        
    }
    private int getRandomNumber (int min, int max)
    {
        return Random.Range(min, max);
    }
}
