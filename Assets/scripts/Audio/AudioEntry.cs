using UnityEngine;

/// <summary>
/// ScriptableObject that represents an audio entry with a name and audio clip.
/// Used by the AudioManager to organize and reference audio assets.
/// </summary>
[CreateAssetMenu(fileName = "AudioEntry", menuName = "AudioObjects/AudioEntry")]
public class AudioEntry : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// The name identifier for this audio entry.
    /// </summary>
    [SerializeField]
    private string audioName;

    /// <summary>
    /// The audio clip associated with this entry.
    /// </summary>
    [SerializeField]
    private AudioClip audioClip;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the name identifier for this audio entry.
    /// </summary>
    public string AudioName => audioName;

    /// <summary>
    /// Gets the audio clip associated with this entry.
    /// </summary>
    public AudioClip AudioClip => audioClip;

    #endregion
}
