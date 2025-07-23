using UnityEngine;

/// <summary>
/// Handles interaction between interactive objects and the player for quest completion.
/// Detects when the player is in range and processes interaction input for find quests.
/// </summary>
public class ObjectInteraction : MonoBehaviour
{
  [SerializeField]
 private Canvas objectInteractionCanvas;
    /// <summary>
    /// Layer mask for detecting the player's presence within interaction range.
    /// </summary>
    [SerializeField]
   private LayerMask playerLayer;

    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    [SerializeField]
    private string playerTag;

    /// <summary>
    /// Tag used to identify the game manager GameObject containing required components.
    /// </summary>
    [SerializeField]
    private string gameManagerTag;

    /// <summary>
    /// Reference to the player's script component for state management.
    /// </summary>
    private playerScript playerStateManager;

    /// <summary>
    /// Reference to the input listener component for detecting interaction input.
    /// </summary>
    private InputListener listener;

    /// <summary>
    /// Reference to the quest manager for processing find quest interactions.
    /// </summary>
    private QuestManager questManager;

    /// <summary>
    /// Flag indicating whether the player is currently within interaction range of this object.
    /// </summary>
    private bool playerIsInRange;

    /// <summary>
    /// Initializes the object interaction system by finding required components and checking initial player proximity.
    /// </summary>
    void Awake()
    {
       playerIsInRange =  Physics.CheckSphere(gameObject.transform.position, 2f, playerLayer, QueryTriggerInteraction.Ignore );

        playerStateManager = GameObject.FindGameObjectWithTag(playerTag).GetComponent<playerScript>();

        listener = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponent<InputListener>();

        questManager = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponent<QuestManager>();
    }

    /// <summary>
    /// Continuously checks for player proximity and processes interaction input for quest completion.
    /// Triggers find quest progress when player interacts while in range.
    /// </summary>
    void Update()
    {
        playerIsInRange = Physics.CheckSphere(gameObject.transform.position, 2f, playerLayer, QueryTriggerInteraction.Ignore);
        if(objectInteractionCanvas != null)
        objectInteractionCanvas.enabled = playerIsInRange;
        if (playerIsInRange && listener.isInteracting()) {

            questManager.addFind(gameObject);

        }

    }
}
