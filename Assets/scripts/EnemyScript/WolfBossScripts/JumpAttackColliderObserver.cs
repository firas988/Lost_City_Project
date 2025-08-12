using UnityEngine;

public class JumpAttackColliderObserver : MonoBehaviour
{
    private WolfBossAttacking wolfBossAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wolfBossAttacking = GetComponentInParent<WolfBossAttacking>();
    }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (wolfBossAttacking.getIsAttacking() && other.gameObject.CompareTag("Player"))
        {
            other
                .GetComponent<StartPlayer>()
                .getPlayer()
                .takeDamage(wolfBossAttacking.getCurrentAttackDMG());
        }
        GetComponent<SphereCollider>().enabled = false;
    }

    public void enableJumpAttackCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }
}
