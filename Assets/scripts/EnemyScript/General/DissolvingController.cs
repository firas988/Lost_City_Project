using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the dissolve effect for GameObjects, typically used for enemy death animations.
/// Manages material shader properties to create smooth dissolve and de-dissolve transitions.
/// Requires AudioSource component for dissolve sound effects.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DissolvingController : MonoBehaviour
{
    #region Material Management
    /// <summary>Array of materials from all renderers that will be affected by dissolve effects.</summary>
    private Material[] skinnesMaterials;
    #endregion

    #region Dissolve Configuration
    /// <summary>Rate at which the dissolve effect progresses (0.0 to 1.0).</summary>
    [SerializeField]
    private float dissolveRate = 0.0125f;

    /// <summary>Time interval between dissolve effect updates for smooth animation.</summary>
    [SerializeField]
    private float refreshRate = 0.025f;
    #endregion

    #region Audio Components
    /// <summary>Reference to the AudioSource component for playing dissolve sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing dissolve sound effects.</summary>
    private AudioManager audioManager;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes materials and audio components on awake.
    /// </summary>
    void Awake()
    {
        // Get all renderers from this object and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Collect all materials from all renderers
        var mats = new System.Collections.Generic.List<Material>();
        foreach (Renderer rend in renderers)
        {
            mats.AddRange(rend.materials);
        }

        // Store materials for dissolve effect manipulation
        skinnesMaterials = mats.ToArray();

        // Get audio components
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
    }
    #endregion

    #region Dissolve Effect Control
    /// <summary>
    /// Starts the dissolve effect coroutine that gradually dissolves the object.
    /// </summary>
    public void StartDissolve()
    {
        StartCoroutine(Dissolve());
    }

    /// <summary>
    /// Coroutine that gradually dissolves the object and destroys it when complete.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    IEnumerator Dissolve()
    {
        if (skinnesMaterials.Length > 0)
        {
            // Play dissolve sound effect
            audioManager.playSFX(audioSource, "Dissolving");

            // Gradually increase dissolve amount from 0 to 1
            float counter = 0f;
            while (counter < 1f)
            {
                counter += dissolveRate;

                // Apply dissolve amount to all materials
                for (int i = 0; i < skinnesMaterials.Length; i++)
                {
                    skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }

        // Destroy the object when dissolve is complete
        Destroy(gameObject);
    }

    /// <summary>
    /// Starts the de-dissolve effect coroutine that gradually reveals the object.
    /// </summary>
    public void StartDeDissolve()
    {
        StartCoroutine(DeDissolve());
    }

    /// <summary>
    /// Sets a specific dissolve amount for immediate effect without animation.
    /// </summary>
    /// <param name="dissolveAmount">Dissolve amount from 0.0 (fully visible) to 1.0 (fully dissolved).</param>
    public void setDissolveAmount(float dissolveAmount = 1f)
    {
        if (skinnesMaterials == null)
        {
            return;
        }

        // Apply dissolve amount to all materials immediately
        for (int i = 0; i < skinnesMaterials.Length; i++)
        {
            if (skinnesMaterials[i] != null)
            {
                skinnesMaterials[i].SetFloat("_DissolveAmount", dissolveAmount);
            }
        }
    }

    /// <summary>
    /// Coroutine that gradually de-dissolves the object from fully dissolved to fully visible.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    IEnumerator DeDissolve()
    {
        if (skinnesMaterials.Length > 0)
        {
            // Start from fully dissolved state
            float counter = 1f;
            for (int i = 0; i < skinnesMaterials.Length; i++)
            {
                skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
            }

            // Gradually decrease dissolve amount from 1 to 0
            while (counter > 0f)
            {
                counter -= dissolveRate;

                // Apply dissolve amount to all materials
                for (int i = 0; i < skinnesMaterials.Length; i++)
                {
                    skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
    #endregion
}
