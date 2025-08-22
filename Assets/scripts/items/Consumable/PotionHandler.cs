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
                break;

            case ConsumableType.StrengthPotion:
                IncreaseStrength(potion);
                potionUIHandler.StartStrengthRegen(potion.EffectDuration);
                break;

            case ConsumableType.SpeedPotion:
                IncreaseSpeed(potion);
                potionUIHandler.StartSpeedRegen(potion.EffectDuration);
                break;

            case ConsumableType.HealthInstantPotion:
                InstantHeal(potion);
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
                break;

            case ConsumableType.StrengthPotion:
                ResetStrength();
                potionUIHandler.StopStrengthRegen();
                break;

            case ConsumableType.SpeedPotion:
                ResetSpeed(potion);
                potionUIHandler.StopSpeedRegen();
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
