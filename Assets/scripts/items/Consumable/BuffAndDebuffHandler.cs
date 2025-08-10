using UnityEngine;
using System.Collections;

public class BuffAndDebuffHandler : MonoBehaviour
{

  private Coroutine HealthRegen;
  private Coroutine StrengthRegen;
  private Coroutine SpeedRegen;


  private string playerTag = "Player";
  private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<StartPlayer>().getPlayer();
    }












    public void StartAddHealthBerSecond(float health, bool isRegen = true)
    {
        if (isRegen && HealthRegen == null)
        {
            HealthRegen = StartCoroutine(AddHealthBerSecond(health));
        }
        else if (!isRegen)
        {
            player.addHealth(health);
        }
    }

    public void StopAddHealthBerSecond()
    {
        if (HealthRegen != null)
        {
            StopCoroutine(HealthRegen);
            HealthRegen = null;
        }
    }

    private IEnumerator AddHealthBerSecond(float health)
    {
        while (true)
        {
            player.addHealth(health);
            yield return new WaitForSeconds(1);
        }
    }


    public void addStrength(float strength)
    {
        player.addStrengthPotionBuff(strength);
    }

    public void resetStrength()
    {
        player.resetStrengthPotionBuff();
    }

    public void addSpeed(float speed)
    {
        player.addSpeedBonus(speed);
    }

    public void resetSpeed(float speed)
    {
        player.removeSpeedPotionBuff(speed);
    }
}