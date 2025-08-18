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

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .95f);
            loadingBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                loadingBar.value = 1;
                yield return new WaitForSeconds(2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
