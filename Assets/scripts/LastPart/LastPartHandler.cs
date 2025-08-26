using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Manages the final part of the game including cutscenes, quest progression, and gate control.
/// Handles multiple quest states and coordinates cutscene playback with quest completion.
/// Controls player visibility, UI state, and gate animations during final story sequences.
/// </summary>
public class LastPartHandler : MonoBehaviour
{
    #region Cutscene Objects
    /// <summary>
    /// Cutscene GameObject for the entrance sequence.
    /// Plays when the player enters the final area.
    /// </summary>
    [SerializeField]
    private GameObject enterCutScene;

    /// <summary>
    /// Cutscene GameObject for the hit sequence.
    /// Plays when the player gets hit in the final area.
    /// </summary>
    [SerializeField]
    private GameObject getHitCutScene;
    #endregion

    #region Cutscene Triggers
    /// <summary>
    /// Collider that triggers the entrance cutscene.
    /// Detects when player enters the final area.
    /// </summary>
    [SerializeField]
    private GameObject enterCutSceneCollider;

    /// <summary>
    /// Collider that triggers the hit cutscene.
    /// Detects when player gets hit in the final area.
    /// </summary>
    [SerializeField]
    private GameObject getHitCutSceneCollider;
    #endregion

    #region Environment Elements
    /// <summary>
    /// Gate GameObject that can be opened during quest progression.
    /// Controlled by quest completion states.
    /// </summary>
    [SerializeField]
    private GameObject gate;
    #endregion

    #region Component References
    /// <summary>
    /// Reference to the quest manager for checking quest completion status.
    /// Used to coordinate quest progression and completion.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Reference to the current active quest from the player.
    /// Tracks which quest is currently being processed.
    /// </summary>
    private Quest currentQuest;
    #endregion

    #region Cutscene State Variables
    /// <summary>
    /// Indicates if the entrance cutscene should be triggered.
    /// Set to true when "Go To The Center" quest becomes active.
    /// </summary>
    private bool isEnterCutScene = false;

    /// <summary>
    /// Indicates if the entrance cutscene has been completed.
    /// Prevents multiple cutscene triggers for the same sequence.
    /// </summary>
    private bool isEnterCutSceneCompleted = false;

    /// <summary>
    /// Indicates if the hit cutscene should be triggered.
    /// Set to true when "Time To Get The Item" quest becomes active.
    /// </summary>
    private bool isGetHitCutScene = false;

    /// <summary>
    /// Indicates if the hit cutscene has been completed.
    /// Prevents multiple cutscene triggers for the same sequence.
    /// </summary>
    private bool isGetHitCutSceneCompleted = false;
    #endregion

    #region GameObject References
    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Reference to the player GameObject for quest checking and state management.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Reference to the GameManager for accessing various system components.
    /// </summary>
    private GameObject gameManager;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by setting up references and subscribing to cutscene triggers.
    /// Finds required GameObjects and sets up event subscriptions for cutscene colliders.
    /// </summary>
    private void Start()
    {
        // Find player and game manager references
        player = GameObject.FindGameObjectWithTag(playerTag);
        gameManager = GameObject.FindGameObjectWithTag(gameManagerTag);

        // Get quest manager for quest coordination
        questManager = gameManager.GetComponentInChildren<QuestManager>();

        // Subscribe to cutscene trigger events
        enterCutSceneCollider
            .GetComponent<ColiderCutScene>()
            .subscribeToOnTriggerEnter(EnterCutScene);
        getHitCutSceneCollider
            .GetComponent<ColiderCutScene>()
            .subscribeToOnTriggerEnter(GetHitCutScene);
    }

    /// <summary>
    /// Updates quest states and checks for quest completion each frame.
    /// Monitors multiple quest types and manages progression accordingly.
    /// </summary>
    private void Update()
    {
        // Check various quest states and update accordingly
        checkIfTheQuestIsGoToTheCenter();
        checkIfTheQuestIsTimeToGetTheItem();
        checkIfTheQuestIsKillTheFinalBossCompleted();
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the current quest and restores player control and UI.
    /// Called when cutscenes finish playing.
    /// </summary>
    /// <param name="director">The PlayableDirector that finished playing.</param>
    private void completeTheQuest(PlayableDirector director)
    {
        // Re-enable menu access and show player UI
        gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(true);
        gameManager.transform.parent.GetComponentInChildren<UIManager>().showPlayerUI();

        // Start quest completion sequence
        StartCoroutine(completeTheQuestCoroutine());
    }

    /// <summary>
    /// Coroutine that completes the appropriate quest based on current quest type.
    /// Waits for player to be active before completing the quest.
    /// </summary>
    /// <returns>Coroutine yield instructions.</returns>
    private IEnumerator completeTheQuestCoroutine()
    {
        // Re-enable player and wait for activation
        player.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        // Complete quest based on current quest type
        if (currentQuest is GoToTheCenter)
        {
            (currentQuest as GoToTheCenter).CompleteQuest();
        }
        else if (currentQuest is TimeToGetTheItem)
        {
            (currentQuest as TimeToGetTheItem).CompleteQuest();
        }
    }
    #endregion

    #region Cutscene Management
    /// <summary>
    /// Handles the entrance cutscene trigger and playback.
    /// Only plays if conditions are met and cutscene hasn't been completed.
    /// </summary>
    private void EnterCutScene()
    {
        // Check if cutscene should play and hasn't been completed
        if (
            isEnterCutScene
            && !isEnterCutSceneCompleted
            && !checkIfTheQuestIsGoToTheCenterCompleted()
        )
        {
            // Hide player and start cutscene
            player.SetActive(false);
            enterCutScene.SetActive(true);

            // Subscribe to cutscene completion event
            enterCutScene.GetComponent<PlayableDirector>().stopped += completeTheQuest;

            // Mark cutscene as completed and disable menu access
            isEnterCutSceneCompleted = true;
            gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(false);
            gameManager.transform.parent.GetComponentInChildren<UIManager>().hideAllMenus();
        }
    }

    /// <summary>
    /// Handles the hit cutscene trigger and playback.
    /// Only plays if conditions are met and cutscene hasn't been completed.
    /// </summary>
    private void GetHitCutScene()
    {
        // Check if cutscene should play and hasn't been completed
        if (isGetHitCutScene && !isGetHitCutSceneCompleted)
        {
            // Hide player and start cutscene
            player.SetActive(false);
            getHitCutScene.SetActive(true);

            // Subscribe to cutscene completion event
            getHitCutScene.GetComponent<PlayableDirector>().stopped += completeTheQuest;

            // Disable menu access and hide all menus
            gameManager.GetComponentInChildren<InputListener>().setCanOpenMenu(false);
            gameManager.transform.parent.GetComponentInChildren<UIManager>().hideAllMenus();

            // Mark cutscene as completed
            isGetHitCutSceneCompleted = true;
        }
    }
    #endregion

    #region Gate Control
    /// <summary>
    /// Opens the gate by triggering its animation.
    /// Called when specific quests are completed.
    /// </summary>
    private void openGate()
    {
        // Trigger the "Open" animation on the gate
        gate.GetComponent<Animator>().SetTrigger("Open");
    }
    #endregion

    #region Quest State Checking
    /// <summary>
    /// Checks if the "Go To The Center" quest has become active.
    /// Enables entrance cutscene trigger when quest becomes active.
    /// </summary>
    private void checkIfTheQuestIsGoToTheCenter()
    {
        // Get current quest from player
        Quest quest = player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest();

        // Check if quest type matches and update state
        if (quest is GoToTheCenter)
        {
            currentQuest = quest;
            isEnterCutScene = true;
        }
    }

    /// <summary>
    /// Checks if the "Time To Get The Item" quest has become active.
    /// Opens gate and enables hit cutscene trigger when quest becomes active.
    /// </summary>
    private void checkIfTheQuestIsTimeToGetTheItem()
    {
        // Get current quest from player
        Quest quest = player.GetComponent<StartPlayer>().getPlayer().getCurrentMainQuest();

        // Check if quest type matches and update state
        if (quest is TimeToGetTheItem)
        {
            currentQuest = quest;

            // Open gate and enable hit cutscene
            openGate();
            isGetHitCutScene = true;
        }
    }

    /// <summary>
    /// Checks if the "Go To The Center" quest has been completed.
    /// Used to prevent entrance cutscene from playing after completion.
    /// </summary>
    /// <returns>True if quest is completed, false otherwise.</returns>
    private bool checkIfTheQuestIsGoToTheCenterCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(GoToTheCenter)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the "Kill The Final Boss" quest has been completed.
    /// Opens the gate when the final boss is defeated.
    /// </summary>
    private void checkIfTheQuestIsKillTheFinalBossCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(KillTheFinalBoss)))
        {
            // Open gate when final boss is defeated
            openGate();
        }
    }
    #endregion
}
