using UnityEngine;

[CreateAssetMenu(fileName = "AudioEntry", menuName = "AudioObjects/AudioEntry")]
public class AudioEntry : ScriptableObject
{
    
    [SerializeField] private string audioName;
    [SerializeField] private AudioClip audioClip;




    //getters
    public string AudioName => audioName;
    public AudioClip AudioClip => audioClip;
   
    
}
