using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FinalBossSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject finalBoss;

    [SerializeField]
    private GameObject finalBossPrefabMatChange;

    [SerializeField]
    private GameObject finalBossSpawnEffect;

    [SerializeField]
    private GameObject EnemySpawnPoint;

    private GameObject cloneEnemyDissolve;
    private GameObject cloneEffect;

    private GameObject cloneFinalBoss;

    [SerializeField]
    private PlayableDirector playableDirector;

    private AudioSource audioSource;
    private AudioManager audioManager;
    private string gameManegerTag = "GameManager";
    private UIManager uiManager;

    private string playerTag = "Player";
    private GameObject player;

    private bool isResetCloneFinalBoss = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManegerTag)
            .GetComponentInChildren<AudioManager>();
        player = GameObject.FindGameObjectWithTag(playerTag);

        uiManager = GameObject
            .FindGameObjectWithTag(gameManegerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }

    private void Update()
    {
        if (cloneFinalBoss != null)
        {
            checkIfThePlayerIsDead();
        }

        checkIfTheBossIsDead();
    }

    public void StartSpawnFinalBoss()
    {
        SpawnFinalBoss();
    }

    public void PausePlayableDirector()
    {
        playableDirector.Pause();
    }

    private void SpawnFinalBoss()
    {
        cloneEnemyDissolve = Instantiate(
            finalBossPrefabMatChange,
            transform.position,
            Quaternion.identity
        );
        audioManager.playSFX(audioSource, "SpawnFinalBoss");
        cloneEnemyDissolve.GetComponent<DissolvingController>().setDissolveAmount();
        cloneEnemyDissolve.transform.SetParent(transform, worldPositionStays: true);
        cloneEnemyDissolve.transform.rotation = Quaternion.Euler(0, 180, 0);
        cloneEffect = Instantiate(finalBossSpawnEffect, transform.position, Quaternion.identity);
        cloneEffect.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        cloneEffect.transform.SetParent(transform, worldPositionStays: true);
        StartCoroutine(startSpawn());
    }

    private IEnumerator startSpawn()
    {
        yield return new WaitForSeconds(2.5f);
        cloneEnemyDissolve.GetComponent<DissolvingController>().StartDeDissolve();
        yield return new WaitForSeconds(2.5f);
        playableDirector.Resume();
        yield return new WaitForSeconds(2f);
        uiManager.showBossHealthBar();
        cloneFinalBoss = Instantiate(finalBoss, transform.position, Quaternion.identity);
        cloneFinalBoss.transform.SetParent(transform, worldPositionStays: true);
        cloneFinalBoss
            .GetComponent<Spawn_Drakonit_Handler>()
            .setEnemiesPlaceHolder(EnemySpawnPoint);
        Destroy(cloneEnemyDissolve);
        Destroy(cloneEffect);
    }

    private void checkIfThePlayerIsDead()
    {
        if (
            player.GetComponent<StartPlayer>().getPlayer().isDead()
            && cloneFinalBoss != null
            && !(cloneFinalBoss.GetComponent<StartNpc>().GetNpcsInstance() as Entity).isDead()
        )
        {
            if (!isResetCloneFinalBoss)
            {
                StartCoroutine(resetCloneFinalBoss());
                isResetCloneFinalBoss = true;
            }
        }
    }

    private IEnumerator resetCloneFinalBoss()
    {
        yield return new WaitForSeconds(3f);
        cloneFinalBoss.GetComponent<Spawn_Drakonit_Handler>().killAllEnemies();
        Destroy(cloneFinalBoss);
        cloneFinalBoss = null;
        yield return new WaitForSeconds(13f);
        SpawnFinalBoss();
        isResetCloneFinalBoss = false;
        uiManager.hideBossHealthBar();
    }

    private void checkIfTheBossIsDead()
    {
        if (
            cloneFinalBoss != null
            && (cloneFinalBoss.GetComponent<StartNpc>().GetNpcsInstance() as Entity).isDead()
        )
        {
            if (
                player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                is KillTheFinalBoss
            )
            {
                (
                    player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                    as KillTheFinalBoss
                ).CompleteQuest();
                uiManager.hideBossHealthBar();
            }
        }
    }
}
