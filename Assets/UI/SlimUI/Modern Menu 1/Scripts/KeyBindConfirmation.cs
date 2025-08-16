using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindConfirmation : MonoBehaviour
{
    [SerializeField]
    private static string key; // key to be set
    private bool isConfirming = false;
    private static TextMeshPro clickedButtonText;
    void Update()
    {



        KeyCode keycode = GetCurrentKeyDown();
        if (keycode != KeyCode.None)
        {
            if (InputListener.setKeybind(key, keycode))
            {
                clickedButtonText.text = keycode.ToString();
            }
            this.gameObject.SetActive(false);
        }
    }

    public KeyCode GetCurrentKeyDown()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                return kcode;
            }
        }
        return KeyCode.None; // No key is currently pressed
    }

    public static void setKey(string key)
    {
        KeyBindConfirmation.key = key;
    }
    public static void setClickedButton(TextMeshPro button)
    {
        clickedButtonText = button;
        Debug.Log(clickedButtonText.text + " " + key);
    }
}
