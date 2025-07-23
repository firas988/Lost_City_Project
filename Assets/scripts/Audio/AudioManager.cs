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
    // ===================== Fields =====================

    private float GlobalVolume = 1f; // Master volume multiplier for all audio
    private float MusicVolume = 1f; // Volume multiplier for music
    private float SFXVolume = 1f; // Volume multiplier for sound effects
    private float EnemyVolume = 1f; // Volume multiplier for enemy sounds
    private float UIVolume = 1f; // Volume multiplier for UI sounds

    [SerializeField]
    private AudioEnteries audioEntries; // Reference to audio entries asset
    private Dictionary<string, AudioClip> audioEntriesDict; // Lookup dictionary for audio clips by name
    private Queue<string> audioQueue = new Queue<string>(); // Queue for UI audio playback
    private bool isAudioPlaying = false; // Tracks if a UI audio is currently playing

    // ===================== Unity Methods =====================

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

    /// <summary>
    /// Initializes the audio entries dictionary at startup.
    /// </summary>
    void Start()
    {
        audioEntriesDict = new Dictionary<string, AudioClip>();
        foreach (AudioEntry audioEntry in audioEntries.AudioEntries)
        {
            audioEntriesDict.Add(audioEntry.AudioName, audioEntry.AudioClip);
        }
    }

    // ===================== Audio Playback Methods =====================

    /// <summary>
    /// Plays a sound effect (SFX) by name on the given AudioSource.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the audio clip to play.</param>
    public void playSFX(AudioSource audioSource, string audioName)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = SFXVolume * GlobalVolume;
            audioSource.PlayOneShot(audioClip);
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
    public void playMusic(AudioSource audioSource, string audioName)
    {
        try
        {
            audioEntriesDict.TryGetValue(audioName, out AudioClip audioClip);
            audioSource.volume = MusicVolume * GlobalVolume;
            audioSource.PlayOneShot(audioClip);
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

    // ===================== UI Audio Queue =====================

    /// <summary>
    /// Queues a UI audio clip to be played in sequence. Only one UI audio will play at a time.
    /// </summary>
    /// <param name="audioSource">The AudioSource to play the clip on.</param>
    /// <param name="audioName">The name of the UI audio clip to queue.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator queueUI(AudioSource audioSource, string audioName)
    {
        audioQueue.Enqueue(audioName);
        while (audioQueue.Count > 0)
        {
            if (!isAudioPlaying)
            {
                isAudioPlaying = true;
                playUI(audioSource, audioQueue.Peek());
                // Wait for the audio clip to finish (plus extra time for animation sync)
                yield return new WaitForSeconds(audioEntriesDict[audioQueue.Peek()].length + 4f);
                isAudioPlaying = false;
                audioQueue.Dequeue();
            }
            else
            {
                yield return null;
            }
        }
    }

    // ===================== Volume Setters =====================

    /// <summary>
    /// Sets the global (master) volume multiplier.
    /// </summary>
    /// <param name="volume">New global volume value.</param>
    public void setGlobalVolume(float volume)
    {
        GlobalVolume = volume;
    }

    /// <summary>
    /// Sets the UI volume multiplier.
    /// </summary>
    /// <param name="volume">New UI volume value.</param>
    public void setUIVolume(float volume)
    {
        UIVolume = volume;
    }

    /// <summary>
    /// Sets the music volume multiplier.
    /// </summary>
    /// <param name="volume">New music volume value.</param>
    public void setMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

    /// <summary>
    /// Sets the SFX volume multiplier.
    /// </summary>
    /// <param name="volume">New SFX volume value.</param>
    public void setSFXVolume(float volume)
    {
        SFXVolume = volume;
    }

    /// <summary>
    /// Sets the enemy volume multiplier.
    /// </summary>
    /// <param name="volume">New enemy volume value.</param>
    public void setEnemyVolume(float volume)
    {
        EnemyVolume = volume;
    }
}
