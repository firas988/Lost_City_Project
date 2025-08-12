using UnityEngine;

public class HandCollisionObserver : MonoBehaviour
{
    [SerializeField]
    private bool playerHit = false;

    private int countEntries = 0;
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
        if (wolfBossAttacking.getIsAttacking() )
        {
            if (other.gameObject.CompareTag("Player"))
            {
                
                other.GetComponent<StartPlayer>().getPlayer().takeDamage(wolfBossAttacking.getCurrentAttackDMG());
                Debug.Log("Player hit with " + wolfBossAttacking.getCurrentAttackDMG() + " damage");
            }
        }
        GetComponent<BoxCollider>().enabled = false;

    }

    private void OnTriggerExit(Collider other)
    {

    }

    public void enableHandCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void disableHandCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
}
