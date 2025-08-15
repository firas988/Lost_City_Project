using System.Collections;
using UnityEngine;

public class PlayerDeadHandler : MonoBehaviour
{
    private Player player;
    private StatisticsHandler statisticsHandler;
    private InputListener inputListener;
    private AnimateAttackController animateAttackController;
    private UIManager uiManager;
    private bool isSpawned = false;

    private string gameManagerTag = "GameManager";

    /// <summary>Tracks whether the player is dead.</summary>
    private bool isDead = false;

    private GameObject playerSpawnPoint;
    private string playerSpawnPointTag = "Respawn";

    void Start()
    {
        player = GetComponent<StartPlayer>().getPlayer();
        statisticsHandler = GetComponentInChildren<StatisticsHandler>();
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();
        animateAttackController = GetComponent<AnimateAttackController>();
        uiManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
        playerSpawnPoint = GameObject.FindGameObjectWithTag(playerSpawnPointTag);
    }

    void Update()
    {
        if (playerSpawnPoint == null)
        {
            playerSpawnPoint = GameObject.FindGameObjectWithTag(playerSpawnPointTag);
        }
        checkDeath();
        if (player.isDead() && !isSpawned && isDead)
        {
            isSpawned = true;
            StartCoroutine(spawnPlayer());
        }
    }

    private IEnumerator spawnPlayer()
    {
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(3f);
        uiManager.startFadeInBlackScreen(1f);
        yield return new WaitForSeconds(3f);
        player.resetHealth();
        animateAttackController.spawnAnimation();
        transform.position = playerSpawnPoint.transform.position;
        yield return new WaitForSeconds(3f);
        uiManager.startFadeOutBlackScreen(0f);
        yield return new WaitForSeconds(1f);
        inputListener.setCanAttack(true);
        inputListener.setCanMove(true);
        isSpawned = false;
        GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Disables input and plays death animation if the player is dead.
    /// </summary>
    public void checkDeath()
    {
        if (player.isDead() && !isDead)
        {
            inputListener.setCanAttack(false);
            inputListener.setCanMove(false);
            animateAttackController.DeathAnimation();
            statisticsHandler.Death();
            isDead = true;
        }
        else if (!player.isDead() && isDead)
        {
            isDead = false;
        }
    }
}
