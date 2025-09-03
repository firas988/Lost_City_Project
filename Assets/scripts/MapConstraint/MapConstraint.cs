using System.Collections;
using UnityEngine;

/// <summary>
/// Manages map boundary constraints and player movement restrictions.
/// Plays audio feedback and temporarily restricts player movement when entering constraint zones.
/// Provides a cinematic experience by controlling player actions during boundary interactions.
/// </summary>
public class MapConstraint : MonoBehaviour
{
    #region Serialized Fields
    [Header("Audio Configuration")]
    /// <summary>
    /// The name of the audio clip to play when the player enters the constraint zone.
    /// Specifies which audio file should be played during the constraint interaction.
    /// </summary>
    [SerializeField]
    private string audioToPlay;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Reference to the audio manager for playing constraint-related sounds.
    /// Coordinates audio playback for map constraint interactions.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the input listener for controlling player movement.
    /// Used to temporarily disable player movement during constraint interactions.
    /// </summary>
    private InputListener inputListener;

    [Header("Configuration")]
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    [Header("State Management")]
    /// <summary>
    /// Flag indicating whether the constraint audio has already been played.
    /// Prevents repeated audio playback and ensures proper constraint behavior.
    /// </summary>
    private bool hasPlayed = false;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the map constraint by finding required system components.
    /// Sets up references to audio manager and input listener for constraint functionality.
    /// </summary>
    private void Awake()
    {
        // Find and store references to required system managers
        audioManager = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        inputListener = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();
    }
    #endregion

    #region Trigger Event Methods
    /// <summary>
    /// Handles player entry into the map constraint zone.
    /// Activates constraint behavior including audio playback and movement restriction.
    /// </summary>
    /// <param name="other">The collider that entered the constraint trigger area.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player and audio hasn't been played yet
        if (other.gameObject.tag == "Player" && !hasPlayed)
        {
            // Disable the trigger to prevent repeated activation
            GetComponent<BoxCollider>().isTrigger = false;

            // Start the constraint sequence with audio playback
            StartCoroutine(PlayAudio(other.gameObject.GetComponent<AudioSource>()));

            // Restrict player movement during the constraint interaction
            inputListener.setCanMove(false);

            // Stop player animation to create cinematic effect
            other.gameObject.GetComponent<AnimateController>().stopPlayerAnimation();
        }
    }
    #endregion

    #region Constraint Management Methods
    /// <summary>
    /// Plays the constraint audio and manages the constraint sequence.
    /// Controls the timing of constraint activation and deactivation.
    /// </summary>
    /// <param name="audioSource">The audio source component to play the constraint sound.</param>
    /// <returns>Coroutine for managing the constraint timing sequence.</returns>
    public IEnumerator PlayAudio(AudioSource audioSource)
    {
        if (audioToPlay == "" || audioToPlay == null || audioToPlay == null)
        {
            yield return null;
        }
        // Play the specified constraint audio clip
        audioManager.playUI(audioSource, audioToPlay);
        hasPlayed = true;

        // Wait for the audio clip to finish playing
        yield return new WaitForSeconds(audioManager.getAudioClipLength(audioToPlay));

        // Reset constraint state and re-enable player movement
        hasPlayed = false;
        GetComponent<BoxCollider>().isTrigger = true;
        inputListener.setCanMove(true);
    }
    #endregion
}
