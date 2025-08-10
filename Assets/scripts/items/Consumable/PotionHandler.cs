using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionHandler : MonoBehaviour
{
    private string gameManagerTag = "GameManager";
    private BuffAndDebuffHandler buffAndDebuffHandler;

    private Dictionary<ConsumableType, Coroutine> activePotionCoroutines =
        new Dictionary<ConsumableType, Coroutine>();

    void Start()
    {
        buffAndDebuffHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<BuffAndDebuffHandler>();
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
                break;

            case ConsumableType.StrengthPotion:
                IncreaseStrength(potion);
                break;

            case ConsumableType.SpeedPotion:
                IncreaseSpeed(potion);
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
                break;

            case ConsumableType.StrengthPotion:
                ResetStrength();
                break;

            case ConsumableType.SpeedPotion:
                ResetSpeed(potion);
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
    }

    void ResetSpeed(ConsumableItem potion)
    {
        buffAndDebuffHandler.resetSpeed(potion.SpeedAmount);
    }

    void InstantHeal(ConsumableItem potion)
    {
        buffAndDebuffHandler.StartAddHealthBerSecond(potion.HealthRegenerationAmount, false);
    }
}
