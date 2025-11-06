using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages dialogue interactions between the player and NPCs, handling conversation flow and quest distribution.
/// Controls dialogue UI display, input handling, and quest assignment upon conversation completion.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// UI button for continuing the dialogue conversation.
    /// </summary>
    [SerializeField]
    private GameObject continueButton;

    /// <summary>
    /// UI button for canceling or exiting the dialogue.
    /// </summary>
    [SerializeField]
    private GameObject cancelButton;

    /// <summary>
    /// UI text for displaying the NPC's name.
    /// </summary>
    [SerializeField]
    private GameObject npcName;

    /// <summary>
    /// Layer mask for identifying NPCs that can engage in dialogue.
    /// </summary>
    [SerializeField]
    private LayerMask talkativeLayers;

    /// <summary>
    /// Reference to the quest manager for quest assignment after dialogue completion.
    /// </summary>
    [SerializeField]
    private QuestManager questManager;

    /// <summary>
    /// Reference to the level manager for XP distribution.
    /// </summary>
    private LevelManager levelManager;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the animation controller for managing player animations during dialogue.
    /// </summary>
    private AnimateController animateController;

    /// <summary>
    /// Reference to the player controller for managing player animations during dialogue.
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// Reference to the input listener for detecting interaction input.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to the player's script for state management during dialogue.
    /// </summary>
    private playerScript playerStateManager;

    /// <summary>
    /// Canvas component for the dialogue UI system.
    /// </summary>
    private Canvas dialUI;

    /// <summary>
    /// Text component for displaying NPC dialogue responses.
    /// </summary>
    private TextMeshProUGUI textContainer;

    /// <summary>
    /// Reference to the NPC GameObject the player is currently talking to.
    /// </summary>
    private GameObject talkingTo;

    /// <summary>
    ///  NPC component of the NPC for quest distribution.
    /// </summary>
    private TalkativeNpc npc;

    /// <summary>
    /// The current dialogue option selected for continuing the conversation.
    /// </summary>
    private string continueSentence;

    /// <summary>
    /// Flag to prevent rapid dialogue interactions.
    /// </summary>
    private bool onCoolDown;

    /// <summary>
    /// Tag for the GameManager GameObject.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Tag for the Player GameObject.
    /// </summary>
    private string playerTag = "Player";

    #endregion

    #region Events

    /// <summary>
    /// Event triggered when dialogue ends, providing the quest to be assigned.
    /// </summary>
    public event Action<Quest> onDialogueExit;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the dialogue manager by finding required components and setting up initial NPC reference.
    /// </summary>
    void Awake()
    {
        // COMPLEXITY ANALYSIS: Awake() - O(n) where n = number of GameObjects in scene
        // Find the input listener component from the GameManager GameObject
        inputListener = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();

        // Find the player script component in the scene
        playerStateManager = GameObject.FindWithTag(playerTag).GetComponent<playerScript>();

        // Find the animation controller component in the scene
        animateController = GameObject.FindWithTag(playerTag).GetComponent<AnimateController>();

        // Find the player controller component in the scene
        playerController = GameObject.FindWithTag(playerTag).GetComponent<PlayerController>();

        // Get the Canvas component attached to this GameObject for UI management
        dialUI = GetComponent<Canvas>();

        // Get the NPC the player is currently interacting with
        talkingTo = playerStateManager.getInteractingWith();

        // Get the level manager component in the scene
        levelManager = GameObject
            .FindWithTag(gameManagerTag)
            .GetComponentInChildren<LevelManager>();

        // If there's an NPC to talk to, get its QuestGiver component
        if (talkingTo != null)
        {
            // Cast the NPC to QuestGiver type for quest distribution functionality
            npc = (TalkativeNpc)talkingTo.GetComponent<StartNpc>().GetNpcsInstance();
        }
        textContainer = this
            .gameObject.transform.Find("Content")
            .gameObject.transform.Find("dialogueText")
            .GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Continuously monitors for dialogue interaction input and manages conversation flow.
    /// Handles dialogue initiation and NPC response processing.
    /// </summary>
    void Update()
    {
        // COMPLEXITY ANALYSIS: Update() - O(1)
        // Early return if player is not near NPC, already in dialogue, or not pressing interaction key

        if (
            !playerStateManager.isNearNPC
            || playerStateManager.isInDialogue()
            || !inputListener.isInteracting()
        )
        {
            // Unlock cursor for UI interaction
            if (playerStateManager.isInDialogue())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return; // Exit early to prevent dialogue processing
        }

        // Get the current NPC the player is trying to interact with
        talkingTo = playerStateManager.getInteractingWith();

        // Early return if no NPC found or NPC is not on a talkative layer
        if (talkingTo == null || !IsInTalkativeLayers(talkingTo))
        {
            return; // Exit early if NPC validation fails
        }

        // Get the QuestGiver component from the NPC for quest functionality
        npc = (TalkativeNpc)talkingTo.GetComponent<StartNpc>().GetNpcsInstance();

        // Check if input is valid, interaction is happening, and quest is not already completed
        if (inputListener != null && inputListener.isInteracting())
        {
            startDialogue();
        }
    }

    #endregion

    #region Dialogue Management

    /// <summary>
    /// Processes the player's response to NPC dialogue and continues the conversation.
    /// Handles quest assignment when dialogue reaches completion.
    /// </summary>
    public void respondToNpc()
    {
        // COMPLEXITY ANALYSIS: respondToNpc() - O(1)
        try
        {
            // Get the NPC's response based on the player's selected dialogue option

            if (updateDialogueText())
            {
                // Dialogue has ended - no more options available
                if (npc is QuestGiver && ((QuestGiver)npc).GetQuestToGive() != null)
                {
                    // Dialogue ended
                    if (((QuestGiver)npc).GetQuestToGive() is StoryQuest)
                    {
                        if (((QuestGiver)npc).GetQuestToGive() != null)
                        {
                            ((StoryQuest)((QuestGiver)npc).GetQuestToGive()).CompleteQuest();
                            levelManager.addXP(200f);
                        }
                        // }

                        closeDialogue();
                        return;
                    }

                    // Get the quest to be assigned to the player
                    Quest questToGive = ((QuestGiver)npc).GetQuestToGive();

                    // Trigger the dialogue exit event with the quest
                    onDialogueExit?.Invoke(questToGive);
                    closeDialogue();
                    return;
                }

                if (npc.GetType() is TalkativeNpc)
                {
                    levelManager.addXP(100f);
                    closeDialogue();
                    return;
                }
                closeDialogue();

                return;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error in respondToNpc: " + npc);
            Debug.LogError("Error in respondToNpc: " + e.Message);
        }
    }

    public bool updateDialogueText()
    {
        string response = npc.respodToDialogue(
            continueSentence != null ? continueSentence : "start",
            out string[] options,
            out bool endDialogue
        );

        if (!endDialogue)
        {
            textContainer.text =
                talkingTo.layer == LayerMask.NameToLayer("QuestGiver")
                && response.Contains("TARGET")
                    ? response.Replace(
                        "TARGET",
                        string.Join(", ", ((QuestGiver)this.npc).GetQuestToGive().QuestTarget)
                    )
                    : response;

            // Set the continue button text to the first dialogue option
            continueButton.GetComponent<TextMeshProUGUI>().text = options[0];
            //set the goodbye button text to the next dialogue option
            cancelButton.GetComponent<TextMeshProUGUI>().text = options[1];
            // Store the selected dialogue option for the next response
            continueSentence = options[0];
        }
        //return an indicator if the dialogue has ended
        return endDialogue;
    }

    /// <summary>
    /// Initiates dialogue with the current NPC and sets up the initial conversation state.
    /// </summary>
    public void startDialogue()
    {
        // COMPLEXITY ANALYSIS: startDialogue() - O(1)
        if (npc.GetType() is QuestGiver)
        {
            if (
                ((QuestGiver)npc).GetQuestToGive() != null
                && ((QuestGiver)npc).GetQuestToGive().isCompleted
            )
            {
                return;
            }
        }

        // stop player animation
        continueSentence = null;
        animateController.stopPlayerAnimation();
        playerController.stopCameraRotation();
        inputListener.setCanOpenMenu(false);

        // Set the NPC name
        npcName.GetComponent<TextMeshProUGUI>().text = talkingTo.tag;

        if (updateDialogueText())
        {
            closeDialogue();
            return;
        }

        // Show the dialogue UI and enter dialogue mode
        showDialogue();
    }

    #endregion

    #region UI Management

    /// <summary>
    /// Closes the dialogue UI and restores normal gameplay state.
    /// Re-enables player input and animations.
    /// </summary>
    public void closeDialogue()
    {
        // COMPLEXITY ANALYSIS: closeDialogue() - O(1)
        // Hide the dialogue UI canvas
        dialUI.enabled = false;

        // Lock the cursor for normal gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set player state to not in dialogue
        playerStateManager.setInDialogue(false);

        // Re-enable input listener for normal gameplay input
        inputListener.enabled = true;

        // Re-enable input listener for normal gameplay input
        inputListener.setCanOpenMenu(true);
        // Re-enable animation controller for player animations
        animateController.enabled = true;

        // Re-enable player controller to prevent player movement during dialogue
        playerController.startCameraRotation();

        // Clear the reference to the NPC being talked to
        playerStateManager.setInteractingWith(null);
    }

    /// <summary>
    /// Shows the dialogue UI and enters dialogue mode.
    /// Disables player input and animations during conversation.
    /// </summary>
    public void showDialogue()
    {
        // COMPLEXITY ANALYSIS: showDialogue() - O(1)
        // Show the dialogue UI canvas
        dialUI.enabled = true;

        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        // Set player state to in dialogue
        playerStateManager.setInDialogue(true);

        // Disable input listener to prevent gameplay input during dialogue
        inputListener.enabled = false;

        // Disable input listener for normal gameplay input
        inputListener.setCanOpenMenu(false);

        // Disable animation controller to prevent player movement during dialogue
        animateController.enabled = false;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Checks whether the given GameObject is on one of the interactive layers.
    /// </summary>
    /// <param name="obj">GameObject to check.</param>
    /// <returns>True if the object's layer is within the interactiveLayers mask; false otherwise.</returns>
    private bool IsInTalkativeLayers(GameObject obj)
    {
        // COMPLEXITY ANALYSIS: IsInTalkativeLayers() - O(1)
        // Use bitwise AND to check if the object's layer is included in the talkativeLayers mask
        return (talkativeLayers.value & (1 << obj.layer)) != 0;
    }

    #endregion
}
