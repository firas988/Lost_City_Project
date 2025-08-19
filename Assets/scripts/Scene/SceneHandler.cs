using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField]
    private Slider loadingBar;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private UIMenuManager uiMenuManager;

    public void LoadScene(int index)
    {
        StartCoroutine(LoadAsynchronously(index));
    }

    IEnumerator LoadAsynchronously(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;
        uiManager.hideAllMenus();
        uiManager.toggleLoadingScreen();
        uiMenuManager.DisablePanels();
        uiMenuManager.toggleLoadingScreen();

        float displayedProgress = 0f;
        float fakeProgressSpeed = 0.5f;

        while (!operation.isDone)
        {
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                0.9f,
                fakeProgressSpeed * Time.deltaTime
            );
            loadingBar.value = displayedProgress;

            if (operation.progress >= 0.9f)
            {
                while (displayedProgress < 1f)
                {
                    displayedProgress = Mathf.MoveTowards(
                        displayedProgress,
                        1f,
                        fakeProgressSpeed * Time.deltaTime
                    );
                    loadingBar.value = displayedProgress;
                    yield return null;
                }

                // yield return new WaitForSeconds(2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
