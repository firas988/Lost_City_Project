using System.Collections;
using UnityEngine;

public class UiInputHandlerTimeLine : MonoBehaviour
{
    private InputListener inputListener;

    private UIManager uiManager;
    private string gameManagerTag = "GameManager";

    void Awake()
    {
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();
        uiManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }

    public void disableInput()
    {
        inputListener.setCanOpenMenu(false);
    }

    public void disableUi()
    {
        uiManager.hideAllMenus();
    }

    public void enableUi()
    {
        uiManager.showPlayerUI();
    }

    public void enableInput()
    {
        inputListener.setCanOpenMenu(true);
    }
}
