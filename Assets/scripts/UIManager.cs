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

    private InputListener inputListener;

    private string GameManagerTag = "GameManager";

    //cooldown for all toggles (flag for each menu)
    private bool cooldownPauseOpen = false;
    private bool cooldownInventoryOpen = false;
    private bool cooldownSkillTreeOpen = false;
    private bool cooldownFullMapOpen = false;
    private bool menuIsOpen = false;

    private void Awake()
    {
        inputListener = GameObject.FindGameObjectWithTag(GameManagerTag)
            .GetComponentInChildren<InputListener>();
        blackScreen.SetActive(false);
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
        skillTreeMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        fullMapMenu.SetActive(false);
    }

    private void Update()
    {
        if (inputListener.isPressingInventory())
        {
            toggleInventory();
        }

        if (inputListener.isPressingSkillTree())
        {
            toggleSkillTreeMenu();
        }

        if (inputListener.isPressingFullMap())
        {
            toggleFullMapMenu();
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
            fullMapMenu.SetActive(false);
            skillTreeMenu.SetActive(false);

            if (inventoryMenu.activeSelf)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
            if (skillTreeMenu.activeSelf)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            StartCoroutine(activateCooldownSkillTreeOpen(1.5f));
            skillTreeMenu.SetActive(!skillTreeMenu.activeSelf);
        }
    }

    public void toggleFullMapMenu()
    {
        if (!cooldownFullMapOpen)
        {
            //similar logic to the other menus
            inventoryMenu.SetActive(false);
            skillTreeMenu.SetActive(false);

            if (fullMapMenu.activeSelf)
            {
                StartCoroutine(FadeOutBlackScreen(0f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                StartCoroutine(FadeInBlackScreen(0.5f));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            StartCoroutine(activateCooldownFullMapOpen(1.5f));
            fullMapMenu.SetActive(!fullMapMenu.activeSelf);
        }
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
        skillTreeMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        blackScreen.SetActive(false);
    }
}
