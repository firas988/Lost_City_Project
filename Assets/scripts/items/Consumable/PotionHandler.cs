using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionHandler : MonoBehaviour
{
    private string gameManagerTag = "GameManager";
    private BuffAndDebuffHandler buffAndDebuffHandler;
    private PotionUIHandler potionUIHandler;
    private PlayerController playerController;
    private Dictionary<ConsumableType, Coroutine> activePotionCoroutines =
        new Dictionary<ConsumableType, Coroutine>();

    [SerializeField]
    private ParticleSystem particleHealthInstant;

    [SerializeField]
    private ParticleSystem particleHealthRegeneration;

    [SerializeField]
    private ParticleSystem particleStrengthRegeneration;

    [SerializeField]
    private ParticleSystem particleSpeedRegeneration;

    void Start()
    {
        buffAndDebuffHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<BuffAndDebuffHandler>();
        potionUIHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<PotionUIHandler>();
        playerController = GetComponent<PlayerController>();
    }

    public void UsePotion(ConsumableItem potion)
    {
        if (activePotionCoroutines.ContainsKey(potion.ConsumableType))
        {
            StopHandler(potion);
            StopCoroutine(activePotionCoroutines[potion.ConsumableType]);
            activePotionCoroutines.Remove(potion.ConsumableType);
        }

        Coroutine newCoroutine = StartCoroutine(ApplyPotionEffect(potion));
        activePotionCoroutines.Add(potion.ConsumableType, newCoroutine);
    }

    private IEnumerator ApplyPotionEffect(ConsumableItem potion)
    {
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

        yield return new WaitForSeconds(potion.EffectDuration);

        StopHandler(potion);

        activePotionCoroutines.Remove(potion.ConsumableType);
    }

    private void StopHandler(ConsumableItem potion)
    {
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

    void StartHealthRegen(ConsumableItem potion)
    {
        buffAndDebuffHandler.StartAddHealthBerSecond(potion.HealthRegenerationAmount);
    }

    void StopHealthRegen()
    {
        buffAndDebuffHandler.StopAddHealthBerSecond();
    }

    void IncreaseStrength(ConsumableItem potion)
    {
        buffAndDebuffHandler.addStrength(potion.StrengthAmount);
    }

    void ResetStrength()
    {
        buffAndDebuffHandler.resetStrength();
    }

    void IncreaseSpeed(ConsumableItem potion)
    {
        buffAndDebuffHandler.addSpeed(potion.SpeedAmount);
        playerController.updateSpeed();
    }

    void ResetSpeed(ConsumableItem potion)
    {
        buffAndDebuffHandler.resetSpeed(potion.SpeedAmount);
        playerController.updateSpeed();
    }

    void InstantHeal(ConsumableItem potion)
    {
        buffAndDebuffHandler.StartAddHealthBerSecond(potion.HealthRegenerationAmount, false);
    }
}
