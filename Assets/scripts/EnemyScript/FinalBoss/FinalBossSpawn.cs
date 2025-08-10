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


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject.FindGameObjectWithTag(gameManegerTag).GetComponentInChildren<AudioManager>();
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

        cloneFinalBoss = Instantiate(finalBoss, transform.position, Quaternion.identity);
        cloneFinalBoss.transform.SetParent(transform, worldPositionStays: true);
        cloneFinalBoss.GetComponent<Spawn_Drakonit_Handler>().setEnemiesPlaceHolder(EnemySpawnPoint);
        Destroy(cloneEnemyDissolve);
        Destroy(cloneEffect);

    }

}
