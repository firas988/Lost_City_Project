using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that contains a collection of Keybind objects.
/// Used to manage multiple keybinding configurations in the game.
/// </summary>
[CreateAssetMenu(fileName = "KeybindList", menuName = "Keybinds/KeybindList")]
public class KeybindList : ScriptableObject
{
    #region Serialized Fields

    /// <summary>
    /// List of all keybinding configurations.
    /// </summary>
    [SerializeField]
    private List<Keybind> keybinds;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the list of all keybinding configurations.
    /// </summary>
    public List<Keybind> Keybinds => keybinds;

    #endregion
}
