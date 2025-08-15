using UnityEngine;

public class AudioTimeLineHandler : MonoBehaviour
{
    private string gameManagerTag = "GameManager";
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<AudioManager>();
    }

    public void SFXVolume(AudioSource audioSource)
    {
        audioManager.setAudioSourceVolumeToSFXVolume(audioSource);
    }

    public void MusicVolume(AudioSource audioSource)
    {
        audioManager.setAudioSourceVolumeToMusicVolume(audioSource);
    }
}
