// AudioManager.cs
// Manages all audio playback and volume control for the game, including SFX, music, enemy, and UI sounds.
// Handles audio queuing for UI sounds and provides runtime volume adjustment.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioManager is responsible for managing all audio playback in the game.
/// It supports global, music, SFX, enemy, and UI volume controls, and provides methods to play and queue audio clips.
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Private Fields

    /// <summary>
    /// Master volume multiplier for all audio.
    /// </summary>
    private float GlobalVolume = 1f;

    /// <summary>
    /// Volume multiplier for music.
    /// </summary>
    private float MusicVolume = 1f;

    /// <summary>
    /// Volume multiplier for sound effects.
    /// </summary>
    private float SFXVolume = 1f;

    /// <summary>
    /// Volume multiplier for enemy sounds.
    /// </summary>
    private float EnemyVolume = 1f;

    /// <summary>
    /// Volume multiplier for UI sounds.
    /// </summary>
    private float UIVolume = 1f;

    /// <summary>
    /// Reference to audio entries asset.
    /// </summary>
    [SerializeField]
    private AudioEnteries audioEntries;

    /// <summary>
    /// Lookup dictionary for audio clips by name.
    /// </summary>
    private Dictionary<string, AudioClip> audioEntriesDict;

    /// <summary>
    /// Queue for UI audio playback.
    /// </summary>
    private Queue<string> audioQueue = new Queue<string>();

    /// <summary>
    /// Tracks if a UI audio is currently playing.
    /// </summary>
    private bool isAudioPlaying = false;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the audio entries dictionary at startup.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(n) where n = number of audio entries
    void Awake()
    {
        // Initialize audio entries dictionary
        audioEntriesDict = new Dictionary<string, AudioClip>();
        foreach (AudioEntry audioEntry in audioEntries.AudioEntries)
        {
            audioEntriesDict.Add(audioEntry.AudioName, audioEntry.AudioClip);
        }

        // Load volume settings from player preferences
        GlobalVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f);
        UIVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        EnemyVolume = PlayerPrefs.GetFloat("EnemyVolume", 1f);
    }

    #endregion

    #region Audio Playback Methods

    /// <summary>
    /// Plays a sound effect (SFX) by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the audio clip to play.</param>
    /// <param name="loop">Whether to loop the audio clip.</param>
    // COMPLEXITY ANALYSIS: playSFX() - O(1)
    public void playSFX(AudioSource audioSource, string audioName, bool loop = false)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = SFXVolume * GlobalVolume;

            if (loop)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"AudioEntry not found: {audioName} - {e}");
        }
    }

    /// <summary>
    /// Plays a music track by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the music clip to play.</param>
    /// <param name="loop">Whether to loop the audio clip.</param>
    // COMPLEXITY ANALYSIS: playMusic() - O(1)
    public void playMusic(AudioSource audioSource, string audioName, bool loop = false)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = MusicVolume * GlobalVolume;

            if (loop)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"AudioEntry not found: {audioName} - {e}");
        }
    }

    /// <summary>
    /// Plays an enemy sound by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the enemy audio clip to play.</param>
    // COMPLEXITY ANALYSIS: playEnemy() - O(1)
    public void playEnemy(AudioSource audioSource, string audioName)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = EnemyVolume * GlobalVolume;
            audioSource.PlayOneShot(audioClip);
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"AudioEntry not found: {audioName} - {e}");
        }
    }

    /// <summary>
    /// Plays a UI sound by name on the given AudioSource using PlayOneShot.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the UI audio clip to play.</param>
    // COMPLEXITY ANALYSIS: playUI() - O(1)
    public void playUI(AudioSource audioSource, string audioName)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = UIVolume * GlobalVolume;
            audioSource.PlayOneShot(audioClip);
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"AudioEntry not found: {audioName} - {e}");
        }
    }

    #endregion

    #region Volume Control Methods

    /// <summary>
    /// Sets the global (master) volume multiplier.
    /// </summary>
    /// <param name="volume">New global volume value.</param>
    // COMPLEXITY ANALYSIS: setGlobalVolume() - O(1)
    public void setGlobalVolume(float volume)
    {
        GlobalVolume = volume;
        PlayerPrefs.SetFloat("GlobalVolume", GlobalVolume);
    }

    /// <summary>
    /// Sets the UI volume multiplier.
    /// </summary>
    /// <param name="volume">New UI volume value.</param>
    // COMPLEXITY ANALYSIS: setUIVolume() - O(1)
    public void setUIVolume(float volume)
    {
        UIVolume = volume;
        PlayerPrefs.SetFloat("UIVolume", UIVolume);
    }

    /// <summary>
    /// Sets the music volume multiplier.
    /// </summary>
    /// <param name="volume">New music volume value.</param>
    // COMPLEXITY ANALYSIS: setMusicVolume() - O(1)
    public void setMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
    }

    /// <summary>
    /// Sets the SFX volume multiplier.
    /// </summary>
    /// <param name="volume">New SFX volume value.</param>
    // COMPLEXITY ANALYSIS: setSFXVolume() - O(1)
    public void setSFXVolume(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }

    /// <summary>
    /// Sets the enemy volume multiplier.
    /// </summary>
    /// <param name="volume">New enemy volume value.</param>
    // COMPLEXITY ANALYSIS: setEnemyVolume() - O(1)
    public void setEnemyVolume(float volume)
    {
        EnemyVolume = volume;
        PlayerPrefs.SetFloat("EnemyVolume", EnemyVolume);
    }

    /// <summary>
    /// Sets the AudioSource volume to match the global volume.
    /// </summary>
    /// <param name="audioSource">The AudioSource to adjust.</param>
    // COMPLEXITY ANALYSIS: setAudioSourceVolumeToGlobalVolume() - O(1)
    public void setAudioSourceVolumeToGlobalVolume(AudioSource audioSource)
    {
        audioSource.volume = GlobalVolume;
    }

    /// <summary>
    /// Sets the AudioSource volume to match the music volume.
    /// </summary>
    /// <param name="audioSource">The AudioSource to adjust.</param>
    // COMPLEXITY ANALYSIS: setAudioSourceVolumeToMusicVolume() - O(1)
    public void setAudioSourceVolumeToMusicVolume(AudioSource audioSource)
    {
        audioSource.volume = MusicVolume * GlobalVolume;
    }

    /// <summary>
    /// Sets the AudioSource volume to match the SFX volume.
    /// </summary>
    /// <param name="audioSource">The AudioSource to adjust.</param>
    // COMPLEXITY ANALYSIS: setAudioSourceVolumeToSFXVolume() - O(1)
    public void setAudioSourceVolumeToSFXVolume(AudioSource audioSource)
    {
        audioSource.volume = SFXVolume * GlobalVolume;
    }

    /// <summary>
    /// Sets the AudioSource volume to match the enemy volume.
    /// </summary>
    /// <param name="audioSource">The AudioSource to adjust.</param>
    // COMPLEXITY ANALYSIS: setAudioSourceVolumeToEnemyVolume() - O(1)
    public void setAudioSourceVolumeToEnemyVolume(AudioSource audioSource)
    {
        audioSource.volume = EnemyVolume * GlobalVolume;
    }

    /// <summary>
    /// Sets the AudioSource volume to match the UI volume.
    /// </summary>
    /// <param name="audioSource">The AudioSource to adjust.</param>
    // COMPLEXITY ANALYSIS: setAudioSourceVolumeToUIVolume() - O(1)
    public void setAudioSourceVolumeToUIVolume(AudioSource audioSource)
    {
        audioSource.volume = UIVolume * GlobalVolume;
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the length of an audio clip by name.
    /// </summary>
    /// <param name="audioName">The name of the audio clip.</param>
    /// <returns>The length of the audio clip in seconds, or 0 if not found.</returns>
    // COMPLEXITY ANALYSIS: getAudioClipLength() - O(1)
    public float getAudioClipLength(string audioName)
    {
        if (audioName == null || !audioEntriesDict.ContainsKey(audioName))
        {
            return 0f;
        }

        return audioEntriesDict[audioName].length;
    }

    /// <summary>
    /// Gets the current global volume.
    /// </summary>
    /// <returns>The global volume value.</returns>
    // COMPLEXITY ANALYSIS: getGlobalVolume() - O(1)
    public float getGlobalVolume()
    {
        return GlobalVolume;
    }

    /// <summary>
    /// Gets the current UI volume.
    /// </summary>
    /// <returns>The UI volume value.</returns>
    // COMPLEXITY ANALYSIS: getUIVolume() - O(1)
    public float getUIVolume()
    {
        return UIVolume;
    }

    /// <summary>
    /// Gets the current music volume.
    /// </summary>
    /// <returns>The music volume value.</returns>
    // COMPLEXITY ANALYSIS: getMusicVolume() - O(1)
    public float getMusicVolume()
    {
        return MusicVolume;
    }

    /// <summary>
    /// Gets the current SFX volume.
    /// </summary>
    /// <returns>The SFX volume value.</returns>
    // COMPLEXITY ANALYSIS: getSFXVolume() - O(1)
    public float getSFXVolume()
    {
        return SFXVolume;
    }

    /// <summary>
    /// Gets the current enemy volume.
    /// </summary>
    /// <returns>The enemy volume value.</returns>
    // COMPLEXITY ANALYSIS: getEnemyVolume() - O(1)
    public float getEnemyVolume()
    {
        return EnemyVolume;
    }

    #endregion
}
