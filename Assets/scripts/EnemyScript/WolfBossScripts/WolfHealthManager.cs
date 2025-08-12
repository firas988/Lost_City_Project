using UnityEngine;

public class WolfHealthManager : MonoBehaviour
{
    [SerializeField]
    private Entity entity;

    [SerializeField]
    private float Curhealth;

    private BossBarHandler bossBar;

    void Start()
    {
        entity = (Entity)GetComponent<StartNpc>().GetNpcsInstance();
        bossBar = GetComponentInChildren<BossBarHandler>();
        Curhealth = entity.getHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (Curhealth != entity.getHealth())
        {
            Curhealth = entity.getHealth();
            bossBar.TakeDamage(Curhealth / entity.getMaxHealth());
        }
    }
}
