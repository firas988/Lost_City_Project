using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public abstract class UIcontroller
{

    public static void ToggleUI(UIBehaviour ui)
    {
        ui.enabled = !ui.enabled;
    }

    public static void SetText(UIBehaviour ui, string text)
    {
        try
        {
            ((TextMeshProUGUI) ui).text = text;
        }
        catch  
        {
            Debug.Log(ui);
            Debug.LogError("The UI element you tried setting it's content is not a text UI element");
        }
    }

}
