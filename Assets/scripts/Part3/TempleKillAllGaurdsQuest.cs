using System.Collections;
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

    private bool isCompleted;
    private string gameManagerTag = "GameManager";
    private QuestManager questManager;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();
        isCompleted = false;
        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    // Update is called once per frame
    void Update()
    {
        if (isCompleted)
        {
            return;
        }

        if (
            (
                player.getCurrentMainQuest() is TempleKillAllGaurds
                && gaurdZones.All(gaurdZone =>
                    gaurdZone.GetComponent<Enemyspawner>().getAllEnemiesDead()
                )
            )
        )
        {
            deactivateHolograms();
            player.getCurrentMainQuest().CompleteQuest();
            isCompleted = true;
        }
    }

    public void deactivateHolograms()
    {
        foreach (GameObject forceField in forceFields)
        {
            forceField.SetActive(false);
        }
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        if (questManager.checkingCompletedStoryQuest(typeof(TempleKillAllGaurds)))
        {
            foreach (GameObject gaurdZone in gaurdZones)
            {
                gaurdZone.GetComponent<Enemyspawner>().setCanMultipleRespawn(true);
            }
            isCompleted = true;
        }
        else if (
            (
                player.getCurrentMainQuest() is TempleKillAllGaurds
                && gaurdZones.All(gaurdZone =>
                    gaurdZone.GetComponent<Enemyspawner>().getAllEnemiesDead()
                )
            )
        )
        {
            deactivateHolograms();
            player.getCurrentMainQuest().CompleteQuest();
            isCompleted = true;
        }
    }
}
