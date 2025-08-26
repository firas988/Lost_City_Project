using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the UI display for active potion effects and their remaining duration.
/// Controls the visibility and countdown display of health, speed, and strength potion effects.
/// Provides visual feedback to players about active buffs and their remaining time.
/// </summary>
public class PotionUIHandler : MonoBehaviour
{
    #region UI Elements
    /// <summary>
    /// UI GameObject for displaying health regeneration potion status.
    /// Shows countdown timer and effect status.
    /// </summary>
    [SerializeField]
    private GameObject potionHealth;

    /// <summary>
    /// UI GameObject for displaying speed potion status.
    /// Shows countdown timer and effect status.
    /// </summary>
    [SerializeField]
    private GameObject potionSpeed;

    /// <summary>
    /// UI GameObject for displaying strength potion status.
    /// Shows countdown timer and effect status.
    /// </summary>
    [SerializeField]
    private GameObject potionStrength;
    #endregion

    #region Coroutine Management
    /// <summary>
    /// Coroutine reference for managing health regeneration UI countdown.
    /// Controls the countdown display and UI updates.
    /// </summary>
    private Coroutine healthRegenCoroutine;

    /// <summary>
    /// Coroutine reference for managing speed potion UI countdown.
    /// Controls the countdown display and UI updates.
    /// </summary>
    private Coroutine speedRegenCoroutine;

    /// <summary>
    /// Coroutine reference for managing strength potion UI countdown.
    /// Controls the countdown display and UI updates.
    /// </summary>
    private Coroutine strengthRegenCoroutine;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes all potion UI elements to be hidden by default.
    /// Ensures clean UI state when the game starts.
    /// </summary>
    private void Awake()
    {
        // Hide all potion UI elements initially
        potionHealth.SetActive(false);
        potionSpeed.SetActive(false);
        potionStrength.SetActive(false);
    }
    #endregion

    #region Potion Effect Start Methods
    /// <summary>
    /// Starts the health regeneration UI countdown display.
    /// Stops any existing countdown before starting a new one.
    /// </summary>
    /// <param name="time">The duration of the health regeneration effect in seconds.</param>
    public void StartHealthRegen(float time)
    {
        // Stop existing countdown if running
        if (healthRegenCoroutine != null)
        {
            StopCoroutine(healthRegenCoroutine);
        }
        // Start new countdown coroutine
        healthRegenCoroutine = StartCoroutine(HealthRegen(time));
    }

    /// <summary>
    /// Starts the speed potion UI countdown display.
    /// Stops any existing countdown before starting a new one.
    /// </summary>
    /// <param name="time">The duration of the speed potion effect in seconds.</param>
    public void StartSpeedRegen(float time)
    {
        // Stop existing countdown if running
        if (speedRegenCoroutine != null)
        {
            StopCoroutine(speedRegenCoroutine);
        }
        // Start new countdown coroutine
        speedRegenCoroutine = StartCoroutine(SpeedRegen(time));
    }

    /// <summary>
    /// Starts the strength potion UI countdown display.
    /// Stops any existing countdown before starting a new one.
    /// </summary>
    /// <param name="time">The duration of the strength potion effect in seconds.</param>
    public void StartStrengthRegen(float time)
    {
        // Stop existing countdown if running
        if (strengthRegenCoroutine != null)
        {
            StopCoroutine(strengthRegenCoroutine);
        }
        // Start new countdown coroutine
        strengthRegenCoroutine = StartCoroutine(StrengthRegen(time));
    }
    #endregion

    #region Countdown Coroutines
    /// <summary>
    /// Coroutine that manages the health regeneration countdown display.
    /// Updates the UI every second and formats time as MM:SS.
    /// </summary>
    /// <param name="time">The total duration of the effect in seconds.</param>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator HealthRegen(float time)
    {
        // Get text component and show health potion UI
        TextMeshProUGUI text = potionHealth.GetComponentInChildren<TextMeshProUGUI>();
        potionHealth.SetActive(true);

        // Countdown loop
        while (time > 0)
        {
            time -= 1;
            // Calculate minutes and seconds for display
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            // Format time as MM:SS
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        // Hide UI when countdown completes
        potionHealth.SetActive(false);
    }

    /// <summary>
    /// Coroutine that manages the speed potion countdown display.
    /// Updates the UI every second and formats time as MM:SS.
    /// </summary>
    /// <param name="time">The total duration of the effect in seconds.</param>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator SpeedRegen(float time)
    {
        // Get text component and show speed potion UI
        TextMeshProUGUI text = potionSpeed.GetComponentInChildren<TextMeshProUGUI>();
        potionSpeed.SetActive(true);

        // Countdown loop
        while (time > 0)
        {
            time -= 1;
            // Calculate minutes and seconds for display
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            // Format time as MM:SS
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        // Hide UI when countdown completes
        potionSpeed.SetActive(false);
    }

    /// <summary>
    /// Coroutine that manages the strength potion countdown display.
    /// Updates the UI every second and formats time as MM:SS.
    /// </summary>
    /// <param name="time">The total duration of the effect in seconds.</param>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator StrengthRegen(float time)
    {
        // Get text component and show strength potion UI
        TextMeshProUGUI text = potionStrength.GetComponentInChildren<TextMeshProUGUI>();
        potionStrength.SetActive(true);

        // Countdown loop
        while (time > 0)
        {
            time -= 1;
            // Calculate minutes and seconds for display
            int seconds = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt(time / 60);
            // Format time as MM:SS
            text.text = $"{minutes:D2}:{seconds:D2}";
            yield return new WaitForSeconds(1);
        }
        // Hide UI when countdown completes
        potionStrength.SetActive(false);
    }
    #endregion

    #region Potion Effect Stop Methods
    /// <summary>
    /// Stops the health regeneration UI countdown and hides the UI.
    /// Cleans up the coroutine and resets the display.
    /// </summary>
    public void StopHealthRegen()
    {
        if (healthRegenCoroutine != null)
        {
            // Stop countdown and hide UI
            StopCoroutine(healthRegenCoroutine);
            potionHealth.SetActive(false);
        }
    }

    /// <summary>
    /// Stops the speed potion UI countdown and hides the UI.
    /// Cleans up the coroutine and resets the display.
    /// </summary>
    public void StopSpeedRegen()
    {
        if (speedRegenCoroutine != null)
        {
            // Stop countdown and hide UI
            StopCoroutine(speedRegenCoroutine);
            potionSpeed.SetActive(false);
        }
    }

    /// <summary>
    /// Stops the strength potion UI countdown and hides the UI.
    /// Cleans up the coroutine and resets the display.
    /// </summary>
    public void StopStrengthRegen()
    {
        if (strengthRegenCoroutine != null)
        {
            // Stop countdown and hide UI
            StopCoroutine(strengthRegenCoroutine);
            potionStrength.SetActive(false);
        }
    }
    #endregion
}
