using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the application and removal of buff and debuff effects from consumable items.
/// Handles health regeneration, strength bonuses, and speed bonuses through coroutines.
/// Coordinates with the Player component to apply and remove temporary stat modifications.
/// </summary>
public class BuffAndDebuffHandler : MonoBehaviour
{
    #region Coroutine Management
    /// <summary>
    /// Coroutine reference for managing health regeneration over time.
    /// Controls the continuous health restoration effect.
    /// </summary>
    private Coroutine HealthRegen;

    /// <summary>
    /// Coroutine reference for managing strength buff duration.
    /// Controls the temporary strength bonus effect.
    /// </summary>
    private Coroutine StrengthRegen;

    /// <summary>
    /// Coroutine reference for managing speed buff duration.
    /// Controls the temporary speed bonus effect.
    /// </summary>
    private Coroutine SpeedRegen;
    #endregion

    #region Player Reference
    /// <summary>
    /// Tag used to find the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Reference to the Player component for applying buff/debuff effects.
    /// </summary>
    private Player player;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the player reference by finding the player GameObject and getting its Player component.
    /// </summary>
    void Start()
    {
        // Find player GameObject and get Player component reference
        player = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<StartPlayer>()
            .getPlayer();
    }
    #endregion

    #region Health Management
    /// <summary>
    /// Starts or applies health regeneration or instant health restoration.
    /// Can either start a continuous regeneration effect or instantly add health.
    /// </summary>
    /// <param name="health">The amount of health to add per second (regeneration) or instantly.</param>
    /// <param name="isRegen">Whether to start regeneration (true) or apply instant health (false).</param>
    public void StartAddHealthBerSecond(float health, bool isRegen = true)
    {
        if (isRegen && HealthRegen == null)
        {
            // Start health regeneration coroutine if not already running
            HealthRegen = StartCoroutine(AddHealthBerSecond(health));
        }
        else if (!isRegen)
        {
            // Apply instant health restoration
            player.addHealth(health);
        }
    }

    /// <summary>
    /// Stops the health regeneration effect if it's currently active.
    /// Cleans up the coroutine reference.
    /// </summary>
    public void StopAddHealthBerSecond()
    {
        if (HealthRegen != null)
        {
            // Stop health regeneration coroutine and clear reference
            StopCoroutine(HealthRegen);
            HealthRegen = null;
        }
    }

    /// <summary>
    /// Coroutine that continuously adds health to the player every second.
    /// Runs indefinitely until stopped by StopAddHealthBerSecond.
    /// </summary>
    /// <param name="health">The amount of health to add per second.</param>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator AddHealthBerSecond(float health)
    {
        while (true)
        {
            // Add health and wait for one second
            player.addHealth(health);
            yield return new WaitForSeconds(1);
        }
    }
    #endregion

    #region Strength Buff Management
    /// <summary>
    /// Applies a strength bonus to the player.
    /// </summary>
    /// <param name="strength">The strength bonus amount to apply.</param>
    public void addStrength(float strength)
    {
        player.addStrengthPotionBuff(strength);
    }

    /// <summary>
    /// Removes the strength bonus from the player.
    /// </summary>
    public void resetStrength()
    {
        player.resetStrengthPotionBuff();
    }
    #endregion

    #region Speed Buff Management
    /// <summary>
    /// Applies a speed bonus to the player.
    /// </summary>
    /// <param name="speed">The speed bonus amount to apply.</param>
    public void addSpeed(float speed)
    {
        player.addSpeedBonus(speed);
    }

    /// <summary>
    /// Removes the speed bonus from the player.
    /// </summary>
    /// <param name="speed">The speed bonus amount to remove.</param>
    public void resetSpeed(float speed)
    {
        player.removeSpeedPotionBuff(speed);
    }
    #endregion
}
