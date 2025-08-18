using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
    private Animator CameraObject;

    // campaign button sub menu
    [Header("MENUS")]
    [Tooltip("The Menu for when the MAIN menu buttons")]
    [SerializeField]
    private GameObject mainMenu;

    [Tooltip("THe first list of buttons")]
    [SerializeField]
    private GameObject firstMenu;

    [Tooltip("The Menu for when the PLAY button is clicked")]
    [CanBeNull]
    [SerializeField]
    private GameObject playMenu;

    [Tooltip("The Menu for when the EXIT button is clicked")]
    [SerializeField]
    private GameObject exitMenu;

    public enum Theme
    {
        custom1,
        custom2,
        custom3,
    };

    [Header("THEME SETTINGS")]
    [SerializeField]
    private Theme theme;
    private int themeIndex;

    [SerializeField]
    private ThemedUIData themeController;

    [Header("PANELS")]
    [Tooltip("The UI Panel parenting all sub menus")]
    [SerializeField]
    private GameObject mainCanvas;

    [Tooltip("The UI Panel that holds the CONTROLS window tab")]
    [SerializeField]
    private GameObject PanelControls;

    [Tooltip("The UI Panel that holds the GAME window tab")]
    [SerializeField]
    private GameObject PanelGame;

    [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
    [SerializeField]
    private GameObject PanelKeyBindings;

    [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
    [SerializeField]
    private GameObject PanelMovement;

    [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
    [SerializeField]
    private GameObject PanelCombat;

    [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
    [SerializeField]
    private GameObject PanelGeneral;

    // highlights in settings screen
    [Header("SETTINGS SCREEN")]
    [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    [SerializeField]
    private GameObject lineGame;

    [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
    [SerializeField]
    private GameObject lineVideo;

    [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
    [SerializeField]
    private GameObject lineControls;

    [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
    [SerializeField]
    private GameObject lineKeyBindings;

    [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
    [SerializeField]
    private GameObject lineMovement;

    [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
    [SerializeField]
    private GameObject lineCombat;

    [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
    [SerializeField]
    private GameObject lineGeneral;

    [Header("LOADING SCREEN")]
    [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
    public bool waitForInput = true;

    [SerializeField]
    private GameObject loadingMenu;

    [Tooltip("The loading bar Slider UI element in the Loading Screen")]
    [SerializeField]
    private Slider loadingBar;

    [SerializeField]
    private TMP_Text loadPromptText;

    [SerializeField]
    private KeyCode userPromptKey;

    [Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    [SerializeField]
    private AudioSource hoverSound;

    [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
    [SerializeField]
    private AudioSource sliderSound;

    [Tooltip(
        "The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen"
    )]
    [SerializeField]
    private AudioSource swooshSound;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();
        audioManager.playMusic(GetComponent<AudioSource>(), "music", true);

        CameraObject = transform.GetComponent<Animator>();

        playMenu?.SetActive(false);
        exitMenu?.SetActive(false);
        firstMenu?.SetActive(true);
        mainMenu?.SetActive(true);

        SetThemeColors();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            CameraObject.Update(Time.unscaledDeltaTime);
        }
    }

    void SetThemeColors()
    {
        switch (theme)
        {
            case Theme.custom1:
                themeController.currentColor = themeController.custom1.graphic1;
                themeController.textColor = themeController.custom1.text1;
                themeIndex = 0;
                break;
            case Theme.custom2:
                themeController.currentColor = themeController.custom2.graphic2;
                themeController.textColor = themeController.custom2.text2;
                themeIndex = 1;
                break;
            case Theme.custom3:
                themeController.currentColor = themeController.custom3.graphic3;
                themeController.textColor = themeController.custom3.text3;
                themeIndex = 2;
                break;
            default:
                Debug.Log("Invalid theme selected.");
                break;
        }
    }

    public void PlayCampaign()
    {
        exitMenu?.SetActive(false);
        playMenu?.SetActive(true);
    }

    public void ReturnMenu()
    {
        playMenu?.SetActive(false);
        exitMenu?.SetActive(false);
        mainMenu?.SetActive(true);
    }

    public void LoadScene(string scene)
    {
        if (scene != "")
        {
            StartCoroutine(LoadAsynchronously(scene));
        }
    }

    public void DisablePlayCampaign()
    {
        playMenu?.SetActive(false);
    }

    public void Position2()
    {
        CameraObject.Update(Time.unscaledDeltaTime);
        DisablePlayCampaign();
        CameraObject.SetFloat("Animate", 1);
    }

    public void Position1()
    {
        CameraObject.Update(Time.unscaledDeltaTime);
        CameraObject.SetFloat("Animate", -2);
        CameraObject.SetBool("Stats", false);
    }

    public void Position3()
    {
        CameraObject.Update(Time.unscaledDeltaTime);
        CameraObject.SetFloat("Animate", -0.5f);
        CameraObject.SetBool("WorldList", false);
    }

    public void statsPanelPosition()
    {
        CameraObject.Update(Time.unscaledDeltaTime);
        CameraObject.SetBool("Stats", true);
    }

    public void worldListPanelPosition()
    {
        CameraObject.Update(Time.unscaledDeltaTime);
        CameraObject.SetBool("WorldList", true);
    }

    void DisablePanels()
    {
        PanelControls.SetActive(false);
        PanelGame.SetActive(false);
        PanelKeyBindings.SetActive(false);

        lineGame.SetActive(false);
        lineControls.SetActive(false);
        lineVideo.SetActive(false);
        lineKeyBindings.SetActive(false);

        PanelMovement.SetActive(false);
        lineMovement.SetActive(false);
        PanelCombat.SetActive(false);
        lineCombat.SetActive(false);
        PanelGeneral.SetActive(false);
        lineGeneral.SetActive(false);
    }

    public void GamePanel()
    {
        DisablePanels();
        PanelGame.SetActive(true);
        lineGame.SetActive(true);
    }

    public void VideoPanel()
    {
        DisablePanels();
        lineVideo.SetActive(true);
    }

    public void ControlsPanel()
    {
        DisablePanels();
        PanelControls.SetActive(true);
        lineControls.SetActive(true);
    }

    public void KeyBindingsPanel()
    {
        DisablePanels();
        MovementPanel();
        PanelKeyBindings.SetActive(true);
        lineKeyBindings.SetActive(true);
    }

    public void MovementPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelMovement.SetActive(true);
        lineMovement.SetActive(true);
    }

    public void CombatPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelCombat.SetActive(true);
        lineCombat.SetActive(true);
    }

    public void GeneralPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelGeneral.SetActive(true);
        lineGeneral.SetActive(true);
    }

    public void PlayHover()
    {
        hoverSound.Play();
    }

    public void PlaySFXHover()
    {
        sliderSound.Play();
    }

    public void PlaySwoosh()
    {
        swooshSound.Play();
    }

    // Are You Sure - Quit Panel Pop Up
    public void AreYouSure()
    {
        exitMenu.SetActive(true);
        DisablePlayCampaign();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Load Bar synching animation
    IEnumerator LoadAsynchronously(string sceneName)
    { // scene name is just the name of the current scene being loaded
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        mainCanvas?.SetActive(false);
        loadingMenu?.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .95f);
            loadingBar.value = progress;

            if (operation.progress >= 0.9f && waitForInput)
            {
                loadPromptText.text =
                    "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                loadingBar.value = 1;

                if (Input.GetKeyDown(userPromptKey))
                {
                    operation.allowSceneActivation = true;
                }
            }
            else if (operation.progress >= 0.9f && !waitForInput)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
