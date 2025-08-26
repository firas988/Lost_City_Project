using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the usage and effects of consumable potions in the game.
/// Handles different potion types including health regeneration, instant healing, strength, and speed potions.
/// Coordinates with BuffAndDebuffHandler, PotionUIHandler, and PlayerController for comprehensive potion management.
/// </summary>
public class PotionHandler : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Reference to the buff/debuff handler for applying potion effects.
    /// </summary>
    private BuffAndDebuffHandler buffAndDebuffHandler;

    /// <summary>
    /// Reference to the potion UI handler for updating visual feedback.
    /// </summary>
    private PotionUIHandler potionUIHandler;

    /// <summary>
    /// Reference to the player controller for updating player stats.
    /// </summary>
    private PlayerController playerController;
    #endregion

    #region Potion Management
    /// <summary>
    /// Dictionary tracking active potion coroutines by consumable type.
    /// Prevents multiple potions of the same type from running simultaneously.
    /// </summary>
    private Dictionary<ConsumableType, Coroutine> activePotionCoroutines =
        new Dictionary<ConsumableType, Coroutine>();
    #endregion

    #region Particle Effects
    /// <summary>
    /// Particle system for instant health potion effects.
    /// Provides visual feedback when using instant healing potions.
    /// </summary>
    [SerializeField]
    private ParticleSystem particleHealthInstant;

    /// <summary>
    /// Particle system for health regeneration potion effects.
    /// Provides visual feedback during health regeneration.
    /// </summary>
    [SerializeField]
    private ParticleSystem particleHealthRegeneration;

    /// <summary>
    /// Particle system for strength potion effects.
    /// Provides visual feedback during strength buffs.
    /// </summary>
    [SerializeField]
    private ParticleSystem particleStrengthRegeneration;

    /// <summary>
    /// Particle system for speed potion effects.
    /// Provides visual feedback during speed buffs.
    /// </summary>
    [SerializeField]
    private ParticleSystem particleSpeedRegeneration;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references to required components and managers.
    /// Finds GameManager and its children components for potion handling.
    /// </summary>
    void Start()
    {
        // Find BuffAndDebuffHandler in GameManager's children
        buffAndDebuffHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<BuffAndDebuffHandler>();

        // Find PotionUIHandler in GameManager's parent's children
        potionUIHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<PotionUIHandler>();

        // Get PlayerController from this GameObject
        playerController = GetComponent<PlayerController>();
    }
    #endregion

    #region Potion Usage
    /// <summary>
    /// Uses a potion by applying its effects and managing the duration.
    /// Stops any existing potion of the same type before applying the new one.
    /// </summary>
    /// <param name="potion">The consumable item to use.</param>
    public void UsePotion(ConsumableItem potion)
    {
        // Stop existing potion of the same type if active
        if (activePotionCoroutines.ContainsKey(potion.ConsumableType))
        {
            StopHandler(potion);
            StopCoroutine(activePotionCoroutines[potion.ConsumableType]);
            activePotionCoroutines.Remove(potion.ConsumableType);
        }

        // Start new potion effect and track the coroutine
        Coroutine newCoroutine = StartCoroutine(ApplyPotionEffect(potion));
        activePotionCoroutines.Add(potion.ConsumableType, newCoroutine);
    }
    #endregion

    #region Potion Effect Application
    /// <summary>
    /// Coroutine that applies potion effects and manages their duration.
    /// Starts appropriate effects based on potion type and waits for duration to expire.
    /// </summary>
    /// <param name="potion">The consumable item whose effects to apply.</param>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator ApplyPotionEffect(ConsumableItem potion)
    {
        // Apply effects based on potion type
        switch (potion.ConsumableType)
        {
            case ConsumableType.HealthRegenerationPotion:
                StartHealthRegen(potion);
                potionUIHandler.StartHealthRegen(potion.EffectDuration);
                particleHealthRegeneration.Play();
                break;

            case ConsumableType.StrengthPotion:
                IncreaseStrength(potion);
                potionUIHandler.StartStrengthRegen(potion.EffectDuration);
                particleStrengthRegeneration.Play();
                break;

            case ConsumableType.SpeedPotion:
                IncreaseSpeed(potion);
                potionUIHandler.StartSpeedRegen(potion.EffectDuration);
                particleSpeedRegeneration.Play();
                break;

            case ConsumableType.HealthInstantPotion:
                InstantHeal(potion);
                particleHealthInstant.Play();
                break;
        }

        // Wait for potion effect duration to expire
        yield return new WaitForSeconds(potion.EffectDuration);

        // Stop all effects when duration expires
        StopHandler(potion);

        // Remove coroutine from tracking dictionary
        activePotionCoroutines.Remove(potion.ConsumableType);
    }
    #endregion

    #region Effect Management
    /// <summary>
    /// Stops all active effects for a specific potion type.
    /// Cleans up buffs, UI updates, and particle effects.
    /// </summary>
    /// <param name="potion">The consumable item whose effects to stop.</param>
    private void StopHandler(ConsumableItem potion)
    {
        // Stop effects based on potion type
        switch (potion.ConsumableType)
        {
            case ConsumableType.HealthRegenerationPotion:
                StopHealthRegen();
                potionUIHandler.StopHealthRegen();
                particleHealthRegeneration.Stop();
                break;

            case ConsumableType.StrengthPotion:
                ResetStrength();
                potionUIHandler.StopStrengthRegen();
                particleStrengthRegeneration.Stop();
                break;

            case ConsumableType.SpeedPotion:
                ResetSpeed(potion);
                potionUIHandler.StopSpeedRegen();
                particleSpeedRegeneration.Stop();
                break;
        }
    }
    #endregion

    #region Health Potion Methods
    /// <summary>
    /// Starts health regeneration effect using the potion's regeneration amount.
    /// </summary>
    /// <param name="potion">The health regeneration potion to use.</param>
    void StartHealthRegen(ConsumableItem potion)
    {
        buffAndDebuffHandler.StartAddHealthBerSecond(potion.HealthRegenerationAmount);
    }

    /// <summary>
    /// Stops the active health regeneration effect.
    /// </summary>
    void StopHealthRegen()
    {
        buffAndDebuffHandler.StopAddHealthBerSecond();
    }

    /// <summary>
    /// Applies instant healing using the potion's health amount.
    /// </summary>
    /// <param name="potion">The instant health potion to use.</param>
    void InstantHeal(ConsumableItem potion)
    {
        buffAndDebuffHandler.StartAddHealthBerSecond(potion.HealthRegenerationAmount, false);
    }
    #endregion

    #region Strength Potion Methods
    /// <summary>
    /// Applies strength bonus using the potion's strength amount.
    /// </summary>
    /// <param name="potion">The strength potion to use.</param>
    void IncreaseStrength(ConsumableItem potion)
    {
        buffAndDebuffHandler.addStrength(potion.StrengthAmount);
    }

    /// <summary>
    /// Removes the active strength bonus effect.
    /// </summary>
    void ResetStrength()
    {
        buffAndDebuffHandler.resetStrength();
    }
    #endregion

    #region Speed Potion Methods
    /// <summary>
    /// Applies speed bonus using the potion's speed amount.
    /// Updates player controller speed to reflect the change.
    /// </summary>
    /// <param name="potion">The speed potion to use.</param>
    void IncreaseSpeed(ConsumableItem potion)
    {
        buffAndDebuffHandler.addSpeed(potion.SpeedAmount);
        playerController.updateSpeed();
    }

    /// <summary>
    /// Removes the active speed bonus effect.
    /// Updates player controller speed to reflect the change.
    /// </summary>
    /// <param name="potion">The speed potion whose effects to remove.</param>
    void ResetSpeed(ConsumableItem potion)
    {
        buffAndDebuffHandler.resetSpeed(potion.SpeedAmount);
        playerController.updateSpeed();
    }
    #endregion
}
