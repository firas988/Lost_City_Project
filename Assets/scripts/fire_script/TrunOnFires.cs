using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Controls the sequential activation of fire effects in braziers, managing particle systems
/// and coordinating with a PlayableDirector for cinematic timing. Automatically discovers
/// numbered braziers and activates them in pairs with visual delays.
/// </summary>
public class TrunOnFires : MonoBehaviour
{
    #region Fire Management
    /// <summary>List of all discovered fire braziers as Transform components.</summary>
    private List<Transform> fires;

    /// <summary>Sorted list of fires ordered by their numeric names for sequential activation.</summary>
    private List<Transform> sortedFires;
    #endregion

    #region Cinematic Control
    /// <summary>Reference to the PlayableDirector for pausing and resuming cinematic sequences.</summary>
    [SerializeField]
    private PlayableDirector playableDirector;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the fire system by discovering numbered braziers and sorting them.
    /// </summary>
    void Start()
    {
        // Initialize fire lists
        fires = new List<Transform>();

        // Get all child transforms (potential braziers)
        Transform[] braziers = transform.GetComponentsInChildren<Transform>();

        // Filter for braziers with numeric names
        foreach (Transform brazier in braziers)
        {
            if (int.TryParse(brazier.name, out int result))
            {
                fires.Add(brazier);
            }
        }

        // Sort fires by numeric name for sequential activation
        sortedFires = fires.OrderBy(fire => int.Parse(fire.name)).ToList();
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Initiates the fire activation sequence by pausing the cinematic and starting the coroutine.
    /// </summary>
    public void TurnOnFires()
    {
        // Pause cinematic playback during fire sequence
        playableDirector.Pause();

        // Start the sequential fire activation
        StartCoroutine(playFires(sortedFires));
    }
    #endregion

    #region Fire Activation Sequence
    /// <summary>
    /// Coroutine that sequentially activates fires in pairs with visual delays.
    /// </summary>
    /// <param name="sortedFires">List of fires to activate in order.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator playFires(List<Transform> sortedFires)
    {
        // Check if fires list is valid
        if (sortedFires != null)
        {
            // Activate fires in pairs (every 2nd fire)
            for (int i = 0; i < sortedFires.Count; i += 2)
            {
                // Get particle systems for current fire pair
                ParticleSystem fireParticleSystem1 = sortedFires[i]
                    .GetComponentInChildren<ParticleSystem>();
                ParticleSystem fireParticleSystem2 = sortedFires[i + 1]
                    .GetComponentInChildren<ParticleSystem>();

                // Activate both particle systems if they exist
                if (fireParticleSystem1 != null && fireParticleSystem2 != null)
                {
                    fireParticleSystem1.Play();
                    fireParticleSystem2.Play();
                }

                // Wait before activating next pair
                yield return new WaitForSeconds(0.4f);
            }
        }

        // Resume cinematic playback after fire sequence completes
        playableDirector.Resume();
    }
    #endregion
}
