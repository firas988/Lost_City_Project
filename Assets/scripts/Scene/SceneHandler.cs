using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField]
    private GameObject loadCreateScene;

    [SerializeField]
    private Slider loadingBar;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private UIMenuManager uiMenuManager;

    [SerializeField]
    private SaveHandler saveHandler;

    [SerializeField]
    private bool inGame = true;

    public void LoadScene(int index)
    {
        StartCoroutine(LoadAsynchronously(index));
    }

    IEnumerator LoadAsynchronously(int index)
    {
        float displayedProgress = 0f;
        loadingBar.value = 0f;
        float fakeProgressSpeed = 0.5f;
        if (inGame)
        {
            if (index != 0)
            {
                saveHandler.SaveGame();
            }
            uiManager.hideAllMenus();
            uiManager.toggleLoadingScreen();
            uiMenuManager.DisablePanels();
            uiMenuManager.toggleLoadingScreen();
            yield return new WaitForSecondsRealtime(1f);
        }
        else
        {
            loadCreateScene.SetActive(false);
            loadingScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                0.9f,
                fakeProgressSpeed * Time.unscaledDeltaTime
            );
            loadingBar.value = displayedProgress;

            if (operation.progress >= 0.9f)
            {
                while (displayedProgress < 1f)
                {
                    displayedProgress = Mathf.MoveTowards(
                        displayedProgress,
                        1f,
                        fakeProgressSpeed * Time.unscaledDeltaTime
                    );
                    loadingBar.value = displayedProgress;
                    yield return null;
                }

                yield return new WaitForSecondsRealtime(1.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
