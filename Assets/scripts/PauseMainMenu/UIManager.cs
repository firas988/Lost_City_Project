using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject fullMapMenu;

    [SerializeField]
    private GameObject skillTreeMenu;

    [SerializeField]
    private GameObject inventoryMenu;

    [SerializeField]
    private GameObject blackScreen;

    [SerializeField]
    private Camera pauseMenu;

    [SerializeField]
    private GameObject SideQuestPanel;

    [SerializeField]
    private GameObject storyQuestPanel;

    private Camera mainCamera;

    private PlayerController playerController;

    private playerScript playerScript;

    [SerializeField]
    private GameObject playerUI;

    [SerializeField]
    private GameObject bossHealthBar;

    private InputListener inputListener;

    private string GameManagerTag = "GameManager";
    private string PlayerTag = "Player";

    //cooldown for all toggles (flag for each menu)
    private bool cooldownPauseOpen = false;
    private bool cooldownInventoryOpen = false;
    private bool cooldownSkillTreeOpen = false;
    private bool cooldownFullMapOpen = false;
    private bool menuIsOpen = false;

    private void Awake()
    {
        inputListener = GameObject
            .FindGameObjectWithTag(GameManagerTag)
            .GetComponentInChildren<InputListener>();
        blackScreen.SetActive(false);
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
        skillTreeMenu.GetComponent<Canvas>().enabled = false;
        inventoryMenu.SetActive(false);
        fullMapMenu.SetActive(false);
        bossHealthBar.SetActive(false);
        mainCamera = GameObject.FindGameObjectWithTag(PlayerTag).GetComponentInChildren<Camera>();
        playerScript = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<playerScript>();
        playerController = GameObject
            .FindGameObjectWithTag(PlayerTag)
            .GetComponent<PlayerController>();
        playerUI?.SetActive(true);
    }

    private void Update()
    {
        if (inputListener.isPressingInventory() && !menuIsOpen)
        {
            toggleInventory();
        }

        if (inputListener.isPressingSkillTree() && !menuIsOpen)
        {
            toggleSkillTreeMenu();
        }

        if (inputListener.isPressingFullMap() && !menuIsOpen)
        {
            toggleFullMapMenu();
        }

        if (inputListener.isPressingPause())
        {
            togglePauseMenu();
        }
    }

    public IEnumerator FadeInBlackScreen(float fadeInAmount)
    {
        float alpha = 0;
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
        blackScreen.SetActive(true);
        while (alpha < fadeInAmount)
        {
            alpha += Time.deltaTime / fadeInAmount;
            blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    public IEnumerator FadeOutBlackScreen(float fadeOutAmount)
    {
        float alpha = 1;
        while (alpha > fadeOutAmount)
        {
            alpha -= Time.deltaTime / fadeOutAmount;
            blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        blackScreen.SetActive(false);
        yield return null;
    }

    public void toggleInventory()
    {
        if (!cooldownInventoryOpen)
        {
            CharacterPrevController characterPrevController = GameObject
                .FindGameObjectWithTag(PlayerTag)
                .GetComponentInChildren<CharacterPrevController>();
            fullMapMenu.SetActive(false);
            skillTreeMenu.GetComponent<Canvas>().enabled = false;

            if (inventoryMenu.activeSelf)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                characterPrevController.hideCharacterPreview();
                playerController.startCameraRotation();
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterPrevController.showCharacterPreview();
                playerController.stopCameraRotation();
            }
            StartCoroutine(activateCooldownInventoryOpen(1.5f));
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        }
    }

    public void toggleSkillTreeMenu()
    {
        if (!cooldownSkillTreeOpen)
        {
            fullMapMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            if (skillTreeMenu.GetComponent<Canvas>().enabled)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.startCameraRotation();
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.stopCameraRotation();
            }
            StartCoroutine(activateCooldownSkillTreeOpen(1.5f));
            skillTreeMenu.GetComponent<Canvas>().enabled = !skillTreeMenu
                .GetComponent<Canvas>()
                .enabled;
        }
    }

    public void toggleFullMapMenu()
    {
        if (!cooldownFullMapOpen)
        {
            //similar logic to the other menus
            inventoryMenu.SetActive(false);
            skillTreeMenu.GetComponent<Canvas>().enabled = false;

            if (fullMapMenu.activeSelf)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.startCameraRotation();
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.stopCameraRotation();
            }
            StartCoroutine(activateCooldownFullMapOpen(1.5f));
            fullMapMenu.SetActive(!fullMapMenu.activeSelf);
        }
    }

    public void togglePauseMenu()
    {
        if (!cooldownPauseOpen)
        {
            if (pauseMenu.enabled)
            {
                menuIsOpen = false;
                mainCamera.enabled = true;
                pauseMenu.enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                showPlayerUI();
                playerController.startCameraRotation();
            }
            else
            {
                menuIsOpen = true;
                mainCamera.enabled = false;
                pauseMenu.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                playerUI.SetActive(false);
                playerController.stopCameraRotation();
            }
            // StartCoroutine(activateCooldownPauseOpen(1.5f));
        }
    }

    public void toggleLoadingScreen()
    {
        mainCamera.enabled = false;
        pauseMenu.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerUI.SetActive(false);
        playerController.stopCameraRotation();
    }

    //IEnumerator for cooldown for ea ch menu
    public IEnumerator activateCooldownInventoryOpen(float cooldownTime)
    {
        cooldownInventoryOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownInventoryOpen = false;
    }

    public IEnumerator activateCooldownSkillTreeOpen(float cooldownTime)
    {
        cooldownSkillTreeOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownSkillTreeOpen = false;
    }

    public IEnumerator activateCooldownPauseOpen(float cooldownTime)
    {
        cooldownPauseOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownPauseOpen = false;
    }

    public IEnumerator activateCooldownFullMapOpen(float cooldownTime)
    {
        cooldownFullMapOpen = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldownFullMapOpen = false;
    }

    public void hideAllMenus()
    {
        fullMapMenu.SetActive(false);
        skillTreeMenu.GetComponent<Canvas>().enabled = false;
        inventoryMenu.SetActive(false);
        blackScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    public void showPlayerUI()
    {
        if (!playerScript.getIsInCutscene())
        {
            playerUI.SetActive(true);
        }
    }

    public void startFadeInBlackScreen(float fadeInAmount)
    {
        StartCoroutine(FadeInBlackScreen(fadeInAmount));
    }

    public void startFadeOutBlackScreen(float fadeOutAmount)
    {
        StartCoroutine(FadeOutBlackScreen(fadeOutAmount));
    }

    public void showBossHealthBar()
    {
        bossHealthBar.SetActive(true);
    }

    public void hideBossHealthBar()
    {
        bossHealthBar.SetActive(false);
    }

    public bool isMenuOpen()
    {
        return menuIsOpen;
    }

    public void addQuest(int questId, Quest questToAdd)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().addQuest(questId, questToAdd);
    }

    public void removeQuest(int questId)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().removeQuest(questId);
    }

    public void updateQuestProgress(int questId, string progress)
    {
        SideQuestPanel.GetComponent<QuestListDisplay>().updateQuestProgress(questId, progress);
    }

    public void updateStoryQuestPanel(Quest questToAdd)
    {
        if (storyQuestPanel.GetComponent<QuestListing>() != null)
        {
            storyQuestPanel.GetComponent<QuestListing>().SetName(questToAdd.GetQuestName());
            storyQuestPanel
                .GetComponent<QuestListing>()
                .SetDescription(questToAdd.GetDescription());

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                storyQuestPanel.GetComponent<RectTransform>()
            );
        }
    }

    public GameObject getQuestById(int questId)
    {
        return SideQuestPanel.transform.Find(questId.ToString()).gameObject;
    }
}
