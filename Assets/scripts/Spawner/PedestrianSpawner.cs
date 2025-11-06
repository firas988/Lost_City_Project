using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the spawning of pedestrian NPCs at designated spawn points.
/// Handles random spawn point selection, NavMesh validation, and spawn rate control.
/// Ensures only one Robert NPC is spawned and manages total spawn count limits.
/// </summary>
public class PedestrianSpawner : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Array of spawn point GameObjects where pedestrians can be spawned.
    /// </summary>
    [SerializeField]
    private GameObject[] spawnPoints;

    /// <summary>
    /// Path to the prefab resources folder containing pedestrian prefabs.
    /// </summary>
    [SerializeField]
    private string prefabPath;
    #endregion

    #region Private Fields
    /// <summary>
    /// Array of loaded pedestrian prefabs from the resources folder.
    /// </summary>
    private GameObject[] prefab;

    /// <summary>
    /// Current count of spawned pedestrians.
    /// </summary>
    private int spawnCount = 0;

    /// <summary>
    /// Current count of spawned Robert NPCs (limited to 1).
    /// </summary>
    private int robertCount = 0;

    /// <summary>
    /// Flag to prevent multiple spawn attempts while waiting for spawn timer.
    /// </summary>
    private bool waitForSpawn = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the spawner by loading pedestrian prefabs from resources.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(p) where p = number of prefabs in resources folder
    void Start()
    {
        // Load all pedestrian prefabs from the specified resources path
        prefab = Resources.LoadAll<GameObject>(prefabPath);
    }

    /// <summary>
    /// Updates spawn logic each frame, initiating pedestrian spawning when conditions are met.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Skip spawning if currently waiting or max spawn count reached
        if (waitForSpawn)
            return;

        if (spawnCount == 20)
            return;

        // Start spawning coroutine
        StartCoroutine(SpawnPedestrians());
    }
    #endregion

    #region Spawn Management
    /// <summary>
    /// Coroutine that handles the spawning of individual pedestrians.
    /// Manages spawn point selection, NavMesh validation, and spawn rate control.
    /// </summary>
    /// <returns>Coroutine yield instructions.</returns>
    // COMPLEXITY ANALYSIS: SpawnPedestrians() - O(1)
    private IEnumerator SpawnPedestrians()
    {
        bool error = false;

        try
        {
            // Select random spawn point and pedestrian prefab
            GameObject spawnPoint = spawnPoints[getRandomNumber(0, spawnPoints.Length)];
            NavMeshHit hit;
            GameObject pedestrian = Instantiate(
                prefab[getRandomNumber(0, prefab.Length)],
                spawnPoint.transform.position,
                Quaternion.identity
            );

            // Check if this is Robert and limit to one instance
            if (pedestrian.CompareTag("Robert") && robertCount >= 1)
            {
                Destroy(pedestrian);
                yield break;
            }

            // Validate spawn position on NavMesh and adjust if necessary
            if (
                !System.Object.ReferenceEquals(pedestrian, null)
                && NavMesh.SamplePosition(
                    pedestrian.transform.position,
                    out hit,
                    2.0f,
                    NavMesh.AllAreas
                )
            )
            {
                // Snap pedestrian to valid NavMesh position
                pedestrian.transform.position = hit.position;
                spawnCount++;

                // Track Robert count if this is a Robert NPC
                if (pedestrian.CompareTag("Robert"))
                {
                    robertCount++;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            Debug.Log("Error: " + ex.Message);
        }

        if (error)
        {
            yield break;
        }

        // Set spawn cooldown to prevent rapid spawning
        waitForSpawn = true;
        yield return new WaitForSeconds(1.0f); // Wait 1 second between spawns
        waitForSpawn = false;
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Generates a random number within the specified range.
    /// </summary>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <returns>Random integer within the specified range.</returns>
    // COMPLEXITY ANALYSIS: getRandomNumber() - O(1)
    private int getRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
    #endregion
}
