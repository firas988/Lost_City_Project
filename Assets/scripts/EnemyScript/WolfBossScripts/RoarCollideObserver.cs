using System.Collections;
using UnityEngine;

public class RoarCollideObserver : MonoBehaviour
{
    WolfBossAttacking wolfBossAttacking;

    private float growSpeed = 1f;

    private float currentRadius = 0;

    private float maxRadius = 10f;

    private SphereCollider sphereCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wolfBossAttacking = GetComponentInParent<WolfBossAttacking>();
        sphereCollider = GetComponent<SphereCollider>();
        currentRadius = sphereCollider.radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wolfBossAttacking.getIsAttacking() && other.CompareTag("Player"))
        {
            Debug.Log("Player entered the Roar Collide Observer");
            Player player = other.GetComponent<StartPlayer>().getPlayer();
            player.takeDamage(wolfBossAttacking.getCurrentAttackDMG());
            StartCoroutine(StrengthDebuff(player));
        }
        sphereCollider.enabled = false;
    }

    public void enableCollider()
    {
        sphereCollider.enabled = true;
        StartCoroutine(SmoothGrowCollider());
    }

    public IEnumerator SmoothGrowCollider()
    {
        while (sphereCollider.radius < maxRadius)
        {
            sphereCollider.radius = Mathf.MoveTowards(
                sphereCollider.radius,
                maxRadius,
                growSpeed * Time.deltaTime
            );
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        sphereCollider.radius = currentRadius;
    }

    public IEnumerator StrengthDebuff(Player player)
    {
        float previousStrengthBonusSkill = player.getCurrentStrengthBonusSkill();
        player.addStrengthBonusSkill(-previousStrengthBonusSkill);
        yield return new WaitForSeconds(30f);
        player.addStrengthBonusSkill(previousStrengthBonusSkill);
    }
}
