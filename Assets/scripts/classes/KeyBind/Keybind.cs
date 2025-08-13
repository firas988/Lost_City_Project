using UnityEngine;

[CreateAssetMenu(fileName = "Keybind", menuName = "Keybinds/Keybind")]
[System.Serializable]
public class Keybind : ScriptableObject
{
    [SerializeField]
    private string key;

    [SerializeField]
    private KeyCode keycode;

    public string Key
    {
        get { return key; }
    }

    public KeyCode Keycode
    {
        get { return keycode; }
    }

    public void SetKeycode(KeyCode keycode)
    {
        this.keycode = keycode;
    }

    public void SetKey(string key)
    {
        this.key = key;
    }
}
