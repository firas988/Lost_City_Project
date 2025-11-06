using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages background music playback based on the current scene.
/// Automatically plays appropriate music tracks when scenes are loaded.
/// </summary>
public class MusicManager : MonoBehaviour
{
    [Header("Audio Management")]
    private AudioManager audioManager; // Reference to the main audio manager for music playback

    [Header("Music Source")]
    private AudioSource musicSource; // AudioSource component for playing music

    /// <summary>
    /// Initializes the MusicManager by finding the AudioManager and setting up music playback
    /// for the current scene. Called once when the MonoBehaviour is created.
    /// </summary>
    void Start()
    {
        // Find the GameManager object and get its AudioManager component
        audioManager = GetComponent<AudioManager>();

        // Get the AudioSource component attached to this GameObject
        musicSource = GetComponent<AudioSource>();

        // Play the appropriate music for the current scene
        playMusic();
    }

    /// <summary>
    /// Plays the appropriate background music based on the scene index.
    /// Maps scene build indices to specific music tracks.
    /// </summary>
    /// <param name="sceneIndex">The build index of the current scene</param>
    public void playMusic()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Play the appropriate music for the current scene
        switch (sceneIndex)
        {
            case 0: // Main Menu scene
                audioManager.playMusic(musicSource, "mainmenu", true);
                break;
            case 2: // Game Part 1 scene
                audioManager.playMusic(musicSource, "part2", true);
                break;
            case 3: // Game Part 3 scene
                audioManager.playMusic(musicSource, "part3", true);
                break;
            case 4: // Game Part 4 scene
                audioManager.playMusic(musicSource, "part4", true);
                break;
            case 5: // Game Part 5 scene
                audioManager.playMusic(musicSource, "part5", true);
                break;
            // Note: Scene index 1 appears to be skipped in the music mapping
        }
    }

    /// <summary>
    /// Stops the current music playback.
    /// </summary>
    public void stopMusic()
    {
        musicSource.Stop();
    }
}
