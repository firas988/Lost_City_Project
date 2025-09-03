using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that contains a collection of AudioEntry objects.
/// Used by the AudioManager to access all available audio entries.
/// </summary>
[CreateAssetMenu(fileName = "AudioEntries", menuName = "AudioObjects/AudioEntries")]
public class AudioEnteries : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// List of all audio entries available in the game.
    /// </summary>
    [SerializeField]
    private List<AudioEntry> audioEntries;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the list of all audio entries.
    /// </summary>
    public List<AudioEntry> AudioEntries => audioEntries;

    #endregion
}
