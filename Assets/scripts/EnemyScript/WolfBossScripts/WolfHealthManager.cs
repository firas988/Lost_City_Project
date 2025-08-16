using System.Collections;
using UnityEngine;

public class WolfHealthManager : MonoBehaviour
{
    [SerializeField]
    private Entity entity;

    [SerializeField]
    private float Curhealth;

    private BossBarHandler bossBar;

    [SerializeField]
    private string bossTag = "WolfBoss";

    void Start()
    {
        entity = (Entity)GetComponent<StartNpc>().GetNpcsInstance();
        bossBar = GameObject.FindObjectOfType<BossBarHandler>();
        Curhealth = entity.getHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.setHealth(0);
        }

        if (Curhealth != entity.getHealth())
        {
            Curhealth = entity.getHealth();

            if (Curhealth <= 0)
            {
                GetComponent<Animator>().SetBool("IsDead", true);
            }

            bossBar.TakeDamage(Curhealth / entity.getMaxHealth());
        }
    }

    public void WolfBossDead()
    {
        StartCoroutine(disAppearBossBar());
    }

    public IEnumerator disAppearBossBar()
    {
        GetComponent<DissolvingController>().StartDissolve();
        yield return new WaitForSeconds(2.5f);
        KillEnemyHandler.KilledEnemy(bossTag);
        Destroy(this.gameObject);
    }
}
