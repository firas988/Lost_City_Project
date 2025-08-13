using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeybindList", menuName = "Keybinds/KeybindList")]
public class KeybindList : ScriptableObject
{
    [SerializeField]
    private List<Keybind> keybinds;

    public List<Keybind> Keybinds
    {
        get { return keybinds; }
    }
}
