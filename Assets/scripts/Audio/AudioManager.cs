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
    void Awake()
    {
        audioEntriesDict = new Dictionary<string, AudioClip>();
        foreach (AudioEntry audioEntry in audioEntries.AudioEntries)
        {
            audioEntriesDict.Add(audioEntry.AudioName, audioEntry.AudioClip);
        }

        //load the volume from the player prefs
        GlobalVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f);
        UIVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        EnemyVolume = PlayerPrefs.GetFloat("EnemyVolume", 1f);
    }

    /// <summary>
    /// Handles runtime input for adjusting global and UI volume using keyboard shortcuts.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            setGlobalVolume(GlobalVolume - 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            setGlobalVolume(GlobalVolume + 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            setUIVolume(UIVolume - 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            setUIVolume(UIVolume + 0.1f);
        }
    }

    #endregion

    #region Audio Playback Methods

    /// <summary>
    /// Plays a sound effect (SFX) by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the audio clip to play.</param>
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
            Debug.LogError("AudioEntry not found: " + audioName);
            Debug.LogError("AudioEntry not found: " + e);
        }
    }

    /// <summary>
    /// Plays a music track by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the music clip to play.</param>
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
            Debug.LogError("AudioEntry not found: " + audioName);
            Debug.LogError("AudioEntry not found: " + e);
        }
    }

    /// <summary>
    /// Plays an enemy sound by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the enemy audio clip to play.</param>
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
            Debug.LogError("AudioEntry not found: " + audioName);
            Debug.LogError("AudioEntry not found: " + e);
        }
    }

    /// <summary>
    /// Plays a UI sound by name on the given AudioSource using PlayOneShot.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the UI audio clip to play.</param>
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
            Debug.LogError("AudioEntry not found: " + audioName);
            Debug.LogError("AudioEntry not found: " + e);
        }
    }

    #endregion


    #region Volume Control Methods

    /// <summary>
    /// Sets the global (master) volume multiplier.
    /// </summary>
    /// <param name="volume">New global volume value.</param>
    public void setGlobalVolume(float volume)
    {
        GlobalVolume = volume;
        PlayerPrefs.SetFloat("GlobalVolume", GlobalVolume);
    }

    /// <summary>
    /// Sets the UI volume multiplier.
    /// </summary>
    /// <param name="volume">New UI volume value.</param>
    public void setUIVolume(float volume)
    {
        UIVolume = volume;
        PlayerPrefs.SetFloat("UIVolume", UIVolume);
    }

    /// <summary>
    /// Sets the music volume multiplier.
    /// </summary>
    /// <param name="volume">New music volume value.</param>
    public void setMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
    }

    /// <summary>
    /// Sets the SFX volume multiplier.
    /// </summary>
    /// <param name="volume">New SFX volume value.</param>
    public void setSFXVolume(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }

    /// <summary>
    /// Sets the enemy volume multiplier.
    /// </summary>
    /// <param name="volume">New enemy volume value.</param>
    public void setEnemyVolume(float volume)
    {
        EnemyVolume = volume;
        PlayerPrefs.SetFloat("EnemyVolume", EnemyVolume);
    }

    public void setAudioSourceVolumeToGlobalVolume(AudioSource audioSource)
    {
        audioSource.volume = GlobalVolume;
    }

    public void setAudioSourceVolumeToMusicVolume(AudioSource audioSource)
    {
        audioSource.volume = MusicVolume * GlobalVolume;
    }

    public void setAudioSourceVolumeToSFXVolume(AudioSource audioSource)
    {
        audioSource.volume = SFXVolume * GlobalVolume;
    }

    public void setAudioSourceVolumeToEnemyVolume(AudioSource audioSource)
    {
        audioSource.volume = EnemyVolume * GlobalVolume;
    }

    public void setAudioSourceVolumeToUIVolume(AudioSource audioSource)
    {
        audioSource.volume = UIVolume * GlobalVolume;
    }

    #endregion

    #region Getters

    public float getAudioClipLength(string audioName)
    {
        return audioEntriesDict[audioName].length;
    }

    public float getGlobalVolume()
    {
        return GlobalVolume;
    }

    public float getUIVolume()
    {
        return UIVolume;
    }

    public float getMusicVolume()
    {
        return MusicVolume;
    }

    public float getSFXVolume()
    {
        return SFXVolume;
    }

    public float getEnemyVolume()
    {
        return EnemyVolume;
    }

    #endregion
}
