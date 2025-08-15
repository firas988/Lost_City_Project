using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class LastPartHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject enterCutScene;

    [SerializeField]
    private GameObject getHitCutScene;

    [SerializeField]
    private GameObject enterCutSceneCollider;

    [SerializeField]
    private GameObject getHitCutSceneCollider;

    [SerializeField]
    private GameObject gate;

    private Quest currentQuest;

    private bool isEnterCutScene = false;
    private bool isEnterCutSceneCompleted = false;
    private bool isGetHitCutScene = false;
    private bool isGetHitCutSceneCompleted = false;

    private string playerTag = "Player";
    private GameObject player;

    private string gameManagerTag = "GameManager";
    private GameObject gameManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);
        enterCutSceneCollider
            .GetComponent<ColiderCutScene>()
            .subscribeToOnTriggerEnter(EnterCutScene);
        getHitCutSceneCollider
            .GetComponent<ColiderCutScene>()
            .subscribeToOnTriggerEnter(GetHitCutScene);
    }

    private void Update()
    {
        checkIfTheQuestIsGoToTheCenter();
        checkIfTheQuestIsTimeToGetTheItem();
    }

    private void completeTheQuest(PlayableDirector director)
    {
        gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(true);
        gameManager.transform.parent.GetComponentInChildren<UIManager>().showPlayerUI();
        StartCoroutine(completeTheQuestCoroutine());
    }

    private IEnumerator completeTheQuestCoroutine()
    {
        player.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if (currentQuest is GoToTheCenter)
        {
            (currentQuest as GoToTheCenter).CompleteQuest();
        }
        else if (currentQuest is TimeToGetTheItem)
        {
            (currentQuest as TimeToGetTheItem).CompleteQuest();
        }
    }

    private void EnterCutScene()
    {
        if (isEnterCutScene && !isEnterCutSceneCompleted)
        {
            player.SetActive(false);
            enterCutScene.SetActive(true);
            enterCutScene.GetComponent<PlayableDirector>().stopped += completeTheQuest;
            isEnterCutSceneCompleted = true;
            gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(false);
            gameManager.transform.parent.GetComponentInChildren<UIManager>().hideAllMenus();
        }
    }

    private void GetHitCutScene()
    {
        if (isGetHitCutScene && !isGetHitCutSceneCompleted)
        {
            player.SetActive(false);
            getHitCutScene.SetActive(true);
            getHitCutScene.GetComponent<PlayableDirector>().stopped += completeTheQuest;
            gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(false);
            gameManager.transform.parent.GetComponentInChildren<UIManager>().hideAllMenus();
            isGetHitCutSceneCompleted = true;
        }
    }

    private void openGate()
    {
        gate.GetComponent<Animator>().SetTrigger("Open");
    }

    private void checkIfTheQuestIsGoToTheCenter()
    {
        Quest quest = player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest();
        if (quest is GoToTheCenter)
        {
            currentQuest = quest;
            isEnterCutScene = true;
        }
    }

    private void checkIfTheQuestIsTimeToGetTheItem()
    {
        Quest quest = player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest();
        if (quest is TimeToGetTheItem)
        {
            currentQuest = quest;
            openGate();
            isGetHitCutScene = true;
        }
    }
}
