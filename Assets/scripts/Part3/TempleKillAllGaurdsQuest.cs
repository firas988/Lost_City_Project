using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TempleKillAllGaurdsQuest : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gaurdZones;

    [SerializeField]
    private List<GameObject> forceFields;

    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (
            player.getCurrentMainQuest() is TempleKillAllGaurds
            && gaurdZones.All(gaurdZone =>
                gaurdZone.GetComponent<Enemyspawner>().getAllEnemiesDead()
            )
        )
        {
            foreach (GameObject forceField in forceFields)
            {
                forceField.SetActive(false);
            }
            player.getCurrentMainQuest().CompleteQuest();
        }
    }
}
