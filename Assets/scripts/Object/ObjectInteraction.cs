using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles interaction between interactive objects and the player for quest completion.
/// Detects when the player is in range and processes interaction input for find quests.
/// </summary>
public class ObjectInteraction : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Layer mask for detecting the player's presence within interaction range.
    /// </summary>
    [SerializeField]
    private LayerMask playerLayer;

    /// <summary>
    /// Range for detecting player proximity.
    /// </summary>
    [SerializeField]
    private float range = 2f;

    /// <summary>
    /// Reference to the progress bar UI element for chest opening progress.
    /// </summary>
    [SerializeField]
    private Image progressBar;

    /// <summary>
    /// Reference to the canvas UI element for chest interaction UI.
    /// </summary>
    [SerializeField]
    private Canvas canvas;

    /// <summary>
    /// Reference to the game object containing the chest opening effect particle system.
    /// </summary>
    [SerializeField]
    private GameObject openChestEffect;

    #endregion

    #region Private Fields

    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Tag used to identify the game manager GameObject containing required components.
    /// </summary>
    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Reference to the player's script component for state management.
    /// </summary>
    private playerScript playerStateManager;

    /// <summary>
    /// Reference to the input listener component for detecting interaction input.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to the quest manager for processing find quest interactions.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Flag indicating whether the player is currently within interaction range of this object.
    /// </summary>
    private bool playerIsInRange;

    /// <summary>
    /// Flag indicating whether the object is a chest.
    /// </summary>
    private bool isAchect = false;

    /// <summary>
    /// Reference to the audio manager for playing chest opening sounds.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the chest reward manager for handling chest rewards.
    /// </summary>
    private ChestRewardManager chestRewardManager;

    /// <summary>
    /// Reference to the audio source component for playing chest opening sounds.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Reference to the particle system for chest opening effects.
    /// </summary>
    private ParticleSystem openChestEffectParticleSystem;

    /// <summary>
    /// Reference to the animator component for controlling chest opening animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Flag indicating whether the chest is currently open.
    /// </summary>
    private bool isOpen = false;

    /// <summary>
    /// Flag indicating whether the chest can be opened.
    /// </summary>
    private bool canOpen = true;

    /// <summary>
    /// Flag indicating whether the player is currently interacting with the chest.
    /// </summary>
    private bool isInteracting = false;

    /// <summary>
    /// Name of the trigger parameter for opening the chest animation.
    /// </summary>
    private string isOpenTrigger = "isOpen";

    /// <summary>
    /// Time required to hold the chest open to complete the opening process.
    /// </summary>
    public float holdTime = 2f;

    /// <summary>
    /// Timer for tracking the duration of the player's interaction with the chest.
    /// </summary>
    private float holdTimer = 0f;

    /// <summary>
    /// Flag indicating whether the object is a key.
    /// </summary>
    private bool isAKey = false;

    private bool isFinshed = false;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the object interaction system by finding required components and checking initial player proximity.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(1)
    void Awake()
    {
        isAchect = transform.CompareTag("Chest");
        isAKey = transform.CompareTag("KeyToFind");
        playerIsInRange = Physics.CheckSphere(
            gameObject.transform.position,
            range,
            playerLayer,
            QueryTriggerInteraction.Ignore
        );

        playerStateManager = GameObject
            .FindGameObjectWithTag(playerTag)
            .GetComponent<playerScript>();

        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();

        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        if (isAchect)
        {
            chestRewardManager = GetComponent<ChestRewardManager>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            audioManager = GameObject
                .FindGameObjectWithTag(gameManagerTag)
                .GetComponentInChildren<AudioManager>();
            openChestEffectParticleSystem = openChestEffect.GetComponent<ParticleSystem>();
        }
        if (isAKey || isAchect)
        {
            canvas.enabled = false;
        }
    }

    /// <summary>
    /// Continuously checks for player proximity and processes interaction input for quest completion.
    /// Triggers find quest progress when player interacts while in range.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        if (isFinshed)
        {
            return;
        }
        updateText();
        playerIsInRange = Physics.CheckSphere(
            gameObject.transform.position,
            range,
            playerLayer,
            QueryTriggerInteraction.Ignore
        );

        if (playerIsInRange && inputListener.isInteracting() && !isAchect && !isAKey)
        {
            questManager.addFind(gameObject);
        }

        if (isAchect)
        {
            checkIfThePlayerIsNearTheChest();
        }
        if (isAchect && playerIsInRange)
        {
            if (canOpen)
            {
                openChestProgress();
            }
            lockToThePlayer();
        }

        if (isAKey)
        {
            checkIfThePlayerIsNearTheKey();

            if (playerIsInRange && inputListener.isInteracting())
            {
                gameObject.GetComponent<ItemToFindTopenTheMiddel_Hnadler>().foundIT();
                setIsFinshed(true);
                canvas.enabled = false;
            }
        }
    }

    #endregion

    #region Quest Interaction

    /// <summary>
    /// Checks if the player is near the key and shows/hides the interaction UI accordingly.
    /// </summary>
    // COMPLEXITY ANALYSIS: checkIfThePlayerIsNearTheKey() - O(1)
    private void checkIfThePlayerIsNearTheKey()
    {
        if (playerIsInRange && !isFinshed)
        {
            canvas.enabled = true;
            lockToThePlayer();
        }
        else
        {
            canvas.enabled = false;
        }
    }

    #endregion

    #region UI Management

    /// <summary>
    /// Updates the interaction text based on the object type and current input keybind.
    /// </summary>
    // COMPLEXITY ANALYSIS: updateText() - O(1)
    private void updateText()
    {
        if (isAchect)
        {
            canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                "Press     " + inputListener.getKeybind("Interact").ToString();
        }
        if (isAKey)
        {
            canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                "Press " + inputListener.getKeybind("Interact").ToString();
        }
    }

    /// <summary>
    /// Hides the interaction canvas.
    /// </summary>
    // COMPLEXITY ANALYSIS: hideCanvas() - O(1)
    public void hideCanvas()
    {
        canvas.enabled = false;
    }

    /// <summary>
    /// Locks the canvas to face the player's camera.
    /// </summary>
    // COMPLEXITY ANALYSIS: lockToThePlayer() - O(1)
    private void lockToThePlayer()
    {
        canvas.transform.LookAt(Camera.main.transform);
    }

    #endregion

    #region Chest Management

    /// <summary>
    /// Checks if the player is near the chest and shows/hides the interaction UI accordingly.
    /// </summary>
    // COMPLEXITY ANALYSIS: checkIfThePlayerIsNearTheChest() - O(1)
    private void checkIfThePlayerIsNearTheChest()
    {
        if (playerIsInRange)
        {
            if (!isOpen)
            {
                canvas.enabled = true;
            }
        }
        else
        {
            progressBar.fillAmount = 0f;
            canvas.enabled = false;
        }
    }

    /// <summary>
    /// Handles the chest opening progress based on player interaction input.
    /// </summary>
    // COMPLEXITY ANALYSIS: openChestProgress() - O(1)
    private void openChestProgress()
    {
        if (inputListener.isInteracting())
        {
            isInteracting = true;
            holdTimer += Time.deltaTime;
            progressBar.fillAmount = holdTimer / holdTime;
            if (holdTimer >= holdTime && !isOpen && canOpen)
            {
                isOpen = true;
                canOpen = false;
                canvas.enabled = false;
                animator.SetTrigger(isOpenTrigger);
                audioManager.playSFX(audioSource, "chestOpen");
                openChestEffectParticleSystem.Play();
            }
        }
        else if (isInteracting)
        {
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                isInteracting = false;
                holdTimer = 0f;
                progressBar.fillAmount = 0f;
            }
            else
            {
                progressBar.fillAmount = holdTimer / holdTime;
            }
        }
    }

    /// <summary>
    /// Handles the completion of chest opening progress.
    /// </summary>
    // COMPLEXITY ANALYSIS: openChestProgressDone() - O(1)
    private void openChestProgressDone()
    {
        chestRewardManager.OpenChest();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets whether the chest is currently open.
    /// </summary>
    /// <returns>True if the chest is open, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getIsOpen() - O(1)
    public bool getIsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// Sets whether the chest can be opened.
    /// </summary>
    /// <param name="canOpen">Whether the chest can be opened.</param>
    // COMPLEXITY ANALYSIS: setCanOpen() - O(1)
    public void setCanOpen(bool canOpen)
    {
        this.canOpen = canOpen;
    }

    /// <summary>
    /// Sets whether the object interaction is finished.
    /// </summary>
    /// <param name="isFinshed">Whether the interaction is finished.</param>
    // COMPLEXITY ANALYSIS: setIsFinshed() - O(1)
    public void setIsFinshed(bool isFinshed)
    {
        this.isFinshed = isFinshed;
    }

    #endregion
}
