using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Manages the spawning and lifecycle of the Final Boss enemy,
/// including spawn effects, material transitions, and quest completion.
/// Handles boss respawning when the player dies and quest progression.
/// </summary>
public class FinalBossSpawn : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>Prefab for the final boss enemy.</summary>
    [SerializeField]
    private GameObject finalBoss;

    /// <summary>Prefab for the boss with material change effect (dissolve).</summary>
    [SerializeField]
    private GameObject finalBossPrefabMatChange;

    /// <summary>Prefab for the boss spawn visual effect.</summary>
    [SerializeField]
    private GameObject finalBossSpawnEffect;

    /// <summary>GameObject marking the spawn point for boss minions.</summary>
    [SerializeField]
    private GameObject EnemySpawnPoint;
    #endregion

    #region Spawned Objects
    /// <summary>Clone of the boss with dissolve effect for spawn animation.</summary>
    private GameObject cloneEnemyDissolve;

    /// <summary>Clone of the spawn visual effect.</summary>
    private GameObject cloneEffect;

    /// <summary>Clone of the actual final boss enemy.</summary>
    private GameObject cloneFinalBoss;
    #endregion

    #region Component References
    /// <summary>Reference to the PlayableDirector for cutscene control.</summary>
    [SerializeField]
    private PlayableDirector playableDirector;

    /// <summary>Reference to the AudioSource component for playing spawn sounds.</summary>
    private AudioSource audioSource;

    /// <summary>Reference to the AudioManager script for playing spawn sounds.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the UIManager for boss health bar control.</summary>
    private UIManager uiManager;
    #endregion

    #region Target References
    /// <summary>Reference to the player GameObject for quest and death checking.</summary>
    private GameObject player;
    #endregion

    #region Configuration
    /// <summary>Tag for the GameManager object.</summary>
    private string gameManegerTag = "GameManager";

    /// <summary>Tag for the Player object.</summary>
    private string playerTag = "Player";
    #endregion

    #region State Variables
    /// <summary>Flag indicating if the boss clone has been reset to prevent multiple resets.</summary>
    private bool isResetCloneFinalBoss = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components and sets up references on start.
    /// </summary>
    private void Start()
    {
        // Get required components
        audioSource = GetComponent<AudioSource>();

        // Find and store audio manager
        audioManager = GameObject
            .FindGameObjectWithTag(gameManegerTag)
            .GetComponentInChildren<AudioManager>();

        // Find and store player reference
        player = GameObject.FindGameObjectWithTag(playerTag);

        // Find and store UI manager
        uiManager = GameObject
            .FindGameObjectWithTag(gameManegerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }

    /// <summary>
    /// Called every frame to check boss and player status.
    /// </summary>
    private void Update()
    {
        // Check player death if boss exists
        if (cloneFinalBoss != null)
        {
            checkIfThePlayerIsDead();
        }

        // Check if boss is dead
        checkIfTheBossIsDead();
    }
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Public method to start the final boss spawning process.
    /// </summary>
    public void StartSpawnFinalBoss()
    {
        SpawnFinalBoss();
    }

    /// <summary>
    /// Pauses the playable director (cutscene).
    /// </summary>
    public void PausePlayableDirector()
    {
        playableDirector.Pause();
    }
    #endregion

    #region Boss Spawning
    /// <summary>
    /// Initiates the final boss spawning sequence with effects and animations.
    /// </summary>
    private void SpawnFinalBoss()
    {
        // Create boss with dissolve effect
        cloneEnemyDissolve = Instantiate(
            finalBossPrefabMatChange,
            transform.position,
            Quaternion.identity
        );

        // Play spawn sound and set up dissolve effect
        audioManager.playSFX(audioSource, "SpawnFinalBoss");
        cloneEnemyDissolve.GetComponent<DissolvingController>().setDissolveAmount();
        cloneEnemyDissolve.transform.SetParent(transform, worldPositionStays: true);
        cloneEnemyDissolve.transform.rotation = Quaternion.Euler(0, 180, 0);

        // Create spawn visual effect
        cloneEffect = Instantiate(finalBossSpawnEffect, transform.position, Quaternion.identity);
        cloneEffect.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        cloneEffect.transform.SetParent(transform, worldPositionStays: true);

        // Start spawn sequence coroutine
        StartCoroutine(startSpawn());
    }

    /// <summary>
    /// Coroutine that manages the spawn sequence timing and effects.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator startSpawn()
    {
        // Wait for initial dissolve effect
        yield return new WaitForSeconds(2.5f);

        // Start de-dissolve effect
        cloneEnemyDissolve.GetComponent<DissolvingController>().StartDeDissolve();
        yield return new WaitForSeconds(2.5f);

        // Resume cutscene and show boss health bar
        playableDirector.Resume();
        yield return new WaitForSeconds(2f);
        uiManager.showBossHealthBar();

        // Spawn actual boss and set up minion spawn point
        cloneFinalBoss = Instantiate(finalBoss, transform.position, Quaternion.identity);
        cloneFinalBoss.transform.SetParent(transform, worldPositionStays: true);
        cloneFinalBoss
            .GetComponent<Spawn_Drakonit_Handler>()
            .setEnemiesPlaceHolder(EnemySpawnPoint);

        // Clean up spawn effects
        Destroy(cloneEnemyDissolve);
        Destroy(cloneEffect);
    }
    #endregion

    #region Player Death Handling
    /// <summary>
    /// Checks if the player is dead and handles boss reset logic.
    /// </summary>
    private void checkIfThePlayerIsDead()
    {
        if (
            player.GetComponent<StartPlayer>().getPlayer().isDead()
            && cloneFinalBoss != null
            && !(cloneFinalBoss.GetComponent<StartNpc>().GetNpcsInstance() as Entity).isDead()
        )
        {
            // Start reset process if not already resetting
            if (!isResetCloneFinalBoss)
            {
                StartCoroutine(resetCloneFinalBoss());
                isResetCloneFinalBoss = true;
            }
        }
    }

    /// <summary>
    /// Coroutine that resets the boss when player dies.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator resetCloneFinalBoss()
    {
        // Wait before cleanup
        yield return new WaitForSeconds(3f);

        // Kill all spawned enemies and destroy boss
        cloneFinalBoss.GetComponent<Spawn_Drakonit_Handler>().killAllEnemies();
        Destroy(cloneFinalBoss);
        cloneFinalBoss = null;

        // Wait before respawning
        yield return new WaitForSeconds(13f);

        // Respawn boss and hide health bar
        SpawnFinalBoss();
        isResetCloneFinalBoss = false;
        uiManager.hideBossHealthBar();
    }
    #endregion

    #region Boss Death Handling
    /// <summary>
    /// Checks if the boss is dead and handles quest completion.
    /// </summary>
    private void checkIfTheBossIsDead()
    {
        if (
            cloneFinalBoss != null
            && (cloneFinalBoss.GetComponent<StartNpc>().GetNpcsInstance() as Entity).isDead()
        )
        {
            // Check if player has the kill final boss quest
            if (
                player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                is KillTheFinalBoss
            )
            {
                // Complete the quest and hide boss health bar
                (
                    player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest()
                    as KillTheFinalBoss
                ).CompleteQuest();
                uiManager.hideBossHealthBar();
            }
        }
    }
    #endregion
}
