using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPrompt : MonoBehaviour
{
    [SerializeField]
    public GameObject confrimationPromptPanel;

    [Tooltip("The text that will be displayed in the confirmation prompt")]
    //<summary>
    //The text that will be displayed in the confirmation prompt
    //</summary>
    [SerializeField]
    private TMP_Text confirmationPromptText;

    //EVENT THAT IS TRIGGERED WHEN THE CONFIRMATION PROMPT IS ACTIVATED
    public event Action onConfirm;

    //<summary>
    //The button that will be clicked if the user confirms the action
    //</summary>

    [Tooltip("The button that will be clicked if the user confirms the action")]
    [SerializeField]
    private Button confirmationPromptYesButton;

    //<summary>
    //The button that will be clicked if the user cancels the action
    //</summary>
    [SerializeField]
    [Tooltip("The button that will be clicked if the user cancels the action")]
    private Button confirmationPromptNoButton;

    //<summary>
    //Sets the text that will be displayed in the confirmation prompt
    //</summary>
    public void setConfirmationPromptText(string text)
    {
        confirmationPromptText.text = text;
    }

    //<summary>
    //Sets the action that will be performed if the user confirms the action
    //</summary>
    public void setConfirmationPromptYes(Action yes)
    {
        confirmationPromptYesButton.onClick.AddListener(() => yes());
    }

    //<summary>
    //Sets the action that will be performed if the user cancels the action
    //</summary>
    public void setConfirmationPromptNo(Action no)
    {
        confirmationPromptNoButton.onClick.AddListener(() => no());
    }

    public void subscribeToConfirmationPrompt(Action action)
    {
        onConfirm += action;
    }

    public void submitConfirmation()
    {
        onConfirm.Invoke();
        confrimationPromptPanel.SetActive(false);
    }

    
}
