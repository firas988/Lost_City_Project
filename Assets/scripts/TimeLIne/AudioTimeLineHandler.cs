using UnityEngine;

/// <summary>
/// Handles audio volume control for timeline/cutscene audio sources.
/// Provides methods to set audio source volumes to match current SFX or music volume settings.
/// Integrates with AudioManager to maintain consistent audio levels across the game.
/// </summary>
public class AudioTimeLineHandler : MonoBehaviour
{
    #region Configuration
    /// <summary>
    /// Tag identifier for finding the GameManager GameObject in the scene.
    /// Used to locate the AudioManager component within the GameManager hierarchy.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Component References
    /// <summary>
    /// Reference to the AudioManager component for accessing current volume settings.
    /// Used to apply consistent SFX and music volume levels to timeline audio sources.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the MusicManager component for accessing current music volume settings.
    /// Used to apply consistent music volume levels to timeline audio sources.
    /// </summary>
    private MusicManager musicManager;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the audio handler by finding and storing reference to the AudioManager.
    /// Locates AudioManager through the GameManager tag for audio volume management.
    /// </summary>
    private void Awake()
    {
        // Find and store reference to AudioManager for volume control
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<AudioManager>();

        // Find and store reference to MusicManager for music volume control
        musicManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<MusicManager>();
    }
    #endregion

    #region Audio Volume Control
    /// <summary>
    /// Sets the specified audio source volume to match current SFX volume settings.
    /// Ensures timeline audio effects maintain consistent volume with other game SFX.
    /// </summary>
    /// <param name="audioSource">The AudioSource component to adjust volume for.</param>
    public void SFXVolume(AudioSource audioSource)
    {
        audioManager.setAudioSourceVolumeToSFXVolume(audioSource);
    }

    /// <summary>
    /// Sets the specified audio source volume to match current music volume settings.
    /// Ensures timeline background music maintains consistent volume with other game music.
    /// </summary>
    /// <param name="audioSource">The AudioSource component to adjust volume for.</param>
    public void MusicVolume(AudioSource audioSource)
    {
        audioManager.setAudioSourceVolumeToMusicVolume(audioSource);
    }

    /// <summary>
    /// Stops the current music playback.
    /// </summary>
    public void stopMusic()
    {
        musicManager.stopMusic();
    }

    /// <summary>
    /// Plays the specified music clip.
    /// </summary>
    public void playMusic()
    {
        musicManager.playMusic();
    }
    #endregion
}
