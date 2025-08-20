using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the enemy health bar UI by syncing it with the Entity's health values.
/// Includes smooth easing for the delayed slider effect.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider; // Immediate health slider (updates instantly)

    [SerializeField]
    private Slider easeHealthSlider; // Eased health slider (updates with delay for visual effect)

    private StartNpc startNpc; // Reference to the parent StartNpc component

    private Entity entity; // Reference to the actual NPC/entity holding health data

    public TextMeshProUGUI healthText; // UI text displaying current health

    [SerializeField]
    private float maxHealth; // Maximum health value

    [SerializeField]
    private float currentHealth; // Current health value

    private float lerpSpeed = 0.05f; // Speed of easing animation

    [SerializeField]
    private bool isEasing = false; // Flag to prevent multiple easing coroutines

    /// <summary>
    /// Initializes references and sets up the sliders and health text at the start.
    /// </summary>
    void Start()
    {
        startNpc = GetComponentInParent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();
        maxHealth = currentHealth = entity.getHealth();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = currentHealth;
        healthText.text = maxHealth.ToString() + " / " + currentHealth.ToString();
    }

    /// <summary>
    /// Updates health bar rotation, current health value, and handles slider easing.
    /// </summary>
    void Update()
    {
        rotateHealthBar();
        updateCurrentHealth();

        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
            healthText.text = maxHealth.ToString() + " / " + currentHealth.ToString();
        }

        // Trigger easing effect only if the sliders are not in sync
        if (healthSlider.value != easeHealthSlider.value)
        {
            StartCoroutine(waitAndEaseHealth());
        }
    }

    /// <summary>
    /// Updates the currentHealth variable with the actual value from the entity.
    /// </summary>
    private void updateCurrentHealth()
    {
        currentHealth = entity.getHealth();
    }

    /// <summary>
    /// Smoothly eases the delayed health slider toward the current health value.
    /// Waits 2 seconds before starting the easing effect.
    /// </summary>
    private IEnumerator waitAndEaseHealth()
    {
        if (isEasing)
            yield break;
        isEasing = true;

        yield return new WaitForSeconds(1.5f);

        while (Mathf.Abs(easeHealthSlider.value - currentHealth) > 1.4f)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
            yield return null;
        }

        easeHealthSlider.value = currentHealth;
        isEasing = false;
    }

    /// <summary>
    /// Rotates the health bar canvas to always face the main camera.
    /// </summary>
    private void rotateHealthBar()
    {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }

    public void showHealthBar()
    {
        healthSlider.gameObject.SetActive(true);
        easeHealthSlider.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
    }

    public void hideHealthBar()
    {
        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
    }
}
