using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioEntries", menuName = "AudioObjects/AudioEntries")]
public class AudioEnteries : ScriptableObject
{
    [SerializeField] private List<AudioEntry> audioEntries;


    //getters
    public List<AudioEntry> AudioEntries => audioEntries;
}
