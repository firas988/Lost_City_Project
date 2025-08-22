using System.Collections;
using TMPro;
using UnityEngine;

public class PotionUIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject potionHealth;

    [SerializeField]
    private GameObject potionSpeed;

    [SerializeField]
    private GameObject potionStrength;

    private Coroutine healthRegenCoroutine;
    private Coroutine speedRegenCoroutine;
    private Coroutine strengthRegenCoroutine;

    private void Awake()
    {
        potionHealth.SetActive(false);
        potionSpeed.SetActive(false);
        potionStrength.SetActive(false);
    }

    public void StartHealthRegen(float time)
    {
        if (healthRegenCoroutine != null)
        {
            StopCoroutine(healthRegenCoroutine);
        }
        healthRegenCoroutine = StartCoroutine(HealthRegen(time));
    }

    public void StartSpeedRegen(float time)
    {
        if (speedRegenCoroutine != null)
        {
            StopCoroutine(speedRegenCoroutine);
        }
        speedRegenCoroutine = StartCoroutine(SpeedRegen(time));
    }

    public void StartStrengthRegen(float time)
    {
        if (strengthRegenCoroutine != null)
        {
            StopCoroutine(strengthRegenCoroutine);
        }
        strengthRegenCoroutine = StartCoroutine(StrengthRegen(time));
    }

    private IEnumerator HealthRegen(float time)
    {
        TextMeshProUGUI text = potionHealth.GetComponentInChildren<TextMeshProUGUI>();
        potionHealth.SetActive(true);
        while (time > 0)
        {
            time -= 1;
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        potionHealth.SetActive(false);
    }

    private IEnumerator SpeedRegen(float time)
    {
        TextMeshProUGUI text = potionSpeed.GetComponentInChildren<TextMeshProUGUI>();
        potionSpeed.SetActive(true);
        while (time > 0)
        {
            time -= 1;
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        potionSpeed.SetActive(false);
    }

    private IEnumerator StrengthRegen(float time)
    {
        TextMeshProUGUI text = potionStrength.GetComponentInChildren<TextMeshProUGUI>();
        potionStrength.SetActive(true);
        while (time > 0)
        {
            time -= 1;
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        potionStrength.SetActive(false);
    }

    public void StopHealthRegen()
    {
        if (healthRegenCoroutine != null)
        {
            StopCoroutine(healthRegenCoroutine);
            potionHealth.SetActive(false);
        }
    }

    public void StopSpeedRegen()
    {
        if (speedRegenCoroutine != null)
        {
            StopCoroutine(speedRegenCoroutine);
            potionSpeed.SetActive(false);
        }
    }

    public void StopStrengthRegen()
    {
        if (strengthRegenCoroutine != null)
        {
            StopCoroutine(strengthRegenCoroutine);
            potionStrength.SetActive(false);
        }
    }
}
