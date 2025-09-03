using UnityEngine;

/// <summary>
/// ScriptableObject that represents a keybinding configuration.
/// Stores both the key name and Unity KeyCode for input handling.
/// </summary>
[CreateAssetMenu(fileName = "Keybind", menuName = "Keybinds/Keybind")]
[System.Serializable]
public class Keybind : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// The display name of the key.
    /// </summary>
    [SerializeField]
    private string key;

    /// <summary>
    /// The Unity KeyCode associated with this keybinding.
    /// </summary>
    [SerializeField]
    private KeyCode keycode;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the display name of the key.
    /// </summary>
    public string Key
    {
        get { return key; }
    }

    /// <summary>
    /// Gets the Unity KeyCode associated with this keybinding.
    /// </summary>
    public KeyCode Keycode
    {
        get { return keycode; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the Unity KeyCode for this keybinding.
    /// </summary>
    /// <param name="keycode">The new KeyCode to assign.</param>
    public void SetKeycode(KeyCode keycode)
    {
        this.keycode = keycode;
    }

    /// <summary>
    /// Sets the display name for this keybinding.
    /// </summary>
    /// <param name="key">The new key name to assign.</param>
    public void SetKey(string key)
    {
        this.key = key;
    }

    #endregion
}
