using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject skillTreeMenu;

    [SerializeField]
    private GameObject inventoryMenu;

    [SerializeField]
    private GameObject blackScreen;

    //cooldown for all toggles (flag for each menu)
    private bool cooldownPauseOpen = false;
    private bool cooldownInventoryOpen = false;
    private bool cooldownSkillTreeOpen = false;

    private bool menuIsOpen = false;

    private void Awake()
    {
        blackScreen.SetActive(true);
        blackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
        skillTreeMenu.SetActive(false);
        inventoryMenu.SetActive(false);
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
            skillTreeMenu.SetActive(false);
            StartCoroutine(activateCooldownInventoryOpen(1.5f));
            StartCoroutine(FadeInBlackScreen(1f));
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        }
    }

    public void toggleSkillTreeMenu()
    {
        if (!cooldownSkillTreeOpen)
        {
            inventoryMenu.SetActive(false);
            StartCoroutine(activateCooldownSkillTreeOpen(1.5f));
            StartCoroutine(FadeInBlackScreen(1f));
            skillTreeMenu.SetActive(!skillTreeMenu.activeSelf);
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
}
