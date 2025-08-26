using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the enemy health bar UI by syncing it with the Entity's health values.
/// Includes smooth easing for the delayed slider effect and automatic camera-facing rotation.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    #region UI Components
    /// <summary>Immediate health slider that updates instantly with health changes.</summary>
    [SerializeField]
    private Slider healthSlider;

    /// <summary>Eased health slider that updates with delay for visual effect.</summary>
    [SerializeField]
    private Slider easeHealthSlider;

    /// <summary>UI text displaying current health in "current / max" format.</summary>
    public TextMeshProUGUI healthText;
    #endregion

    #region Component References
    /// <summary>Reference to the parent StartNpc component for accessing the entity.</summary>
    private StartNpc startNpc;

    /// <summary>Reference to the actual NPC/entity holding health data.</summary>
    private Entity entity;
    #endregion

    #region Health Values
    /// <summary>Maximum health value for the enemy.</summary>
    [SerializeField]
    private float maxHealth;

    /// <summary>Current health value for the enemy.</summary>
    [SerializeField]
    private float currentHealth;
    #endregion

    #region Easing Configuration
    /// <summary>Speed of the easing animation for smooth health bar transitions.</summary>
    private float lerpSpeed = 0.05f;

    /// <summary>Flag to prevent multiple easing coroutines from running simultaneously.</summary>
    [SerializeField]
    private bool isEasing = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references and sets up the sliders and health text at the start.
    /// </summary>
    void Start()
    {
        // Get required components
        startNpc = GetComponentInParent<StartNpc>();
        entity = (Entity)startNpc.GetNpcsInstance();

        // Initialize health values and slider setup
        maxHealth = currentHealth = entity.getHealth();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = currentHealth;

        // Set initial health text display
        healthText.text = maxHealth.ToString() + " / " + currentHealth.ToString();
    }

    /// <summary>
    /// Updates health bar rotation, current health value, and handles slider easing.
    /// </summary>
    void Update()
    {
        // Update health bar orientation and health values
        rotateHealthBar();
        updateCurrentHealth();

        // Update immediate health slider and text if health changed
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
    #endregion

    #region Health Management
    /// <summary>
    /// Updates the currentHealth variable with the actual value from the entity.
    /// </summary>
    private void updateCurrentHealth()
    {
        currentHealth = entity.getHealth();
    }
    #endregion

    #region Easing Animation
    /// <summary>
    /// Smoothly eases the delayed health slider toward the current health value.
    /// Waits 1.5 seconds before starting the easing effect.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator waitAndEaseHealth()
    {
        // Prevent multiple easing coroutines
        if (isEasing)
            yield break;
        isEasing = true;

        // Wait before starting ease effect
        yield return new WaitForSeconds(1.5f);

        // Smoothly ease the slider toward current health
        while (Mathf.Abs(easeHealthSlider.value - currentHealth) > 1.4f)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
            yield return null;
        }

        // Ensure final value is exact
        easeHealthSlider.value = currentHealth;
        isEasing = false;
    }
    #endregion

    #region UI Orientation
    /// <summary>
    /// Rotates the health bar canvas to always face the main camera.
    /// </summary>
    private void rotateHealthBar()
    {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Shows the health bar UI elements (sliders and text).
    /// </summary>
    public void showHealthBar()
    {
        healthSlider.gameObject.SetActive(true);
        easeHealthSlider.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the health bar UI elements (sliders and text).
    /// </summary>
    public void hideHealthBar()
    {
        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
    }
    #endregion
}
