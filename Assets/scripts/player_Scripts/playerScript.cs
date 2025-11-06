using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles player interaction detection with nearby interactable objects using a physics sphere check,
/// and manages the interaction UI and dialogue state.
/// Provides comprehensive interaction system for NPCs, objects, and environmental interactions.
/// Manages dialogue state and cutscene coordination for immersive gameplay.
/// </summary>
public class playerScript : MonoBehaviour
{
    #region Serialized Fields

    [Header("Detection Settings")]
    [Tooltip("The Transform used as the center point of the detection sphere.")]
    /// <summary>
    /// The Transform used as the center point of the detection sphere.
    /// Positioned on the player for accurate interaction range detection.
    /// </summary>
    [SerializeField]
    private Transform detectionPoint;

    [Tooltip("The radius of the detection sphere.")]
    /// <summary>
    /// The radius of the detection sphere in world units.
    /// Defines the maximum distance for detecting interactable objects.
    /// </summary>
    [SerializeField]
    private float detectionRadius = 2f;

    [Tooltip("Layer mask used to filter interactable objects.")]
    /// <summary>
    /// Layer mask used to filter interactable objects.
    /// Ensures only objects on interactive layers are detected.
    /// </summary>
    [SerializeField]
    private LayerMask interactiveLayers;

    [Header("UI Settings")]
    [Tooltip("UI element to be shown or hidden when near an interactable.")]
    /// <summary>
    /// UI element to be shown or hidden when near an interactable.
    /// Provides visual feedback for available interactions.
    /// </summary>
    [SerializeField]
    private GameObject interactionUI;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the input listener script for handling player input.
    /// Manages input state and interaction controls.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Tracks if the player is currently in a dialogue session.
    /// Controls interaction availability and input handling during conversations.
    /// </summary>
    private bool in_dialogue;

    /// <summary>
    /// Stores the currently detected interactable GameObject.
    /// Reference to the object the player can interact with.
    /// </summary>
    private GameObject currentInteractable;

    /// <summary>
    /// Flag indicating if the player is near an interactable object.
    /// Used to control interaction UI and input state.
    /// </summary>
    private bool isNearInteractable;

    /// <summary>
    /// Flag indicating if the player is in a cutscene.
    /// Static variable for global cutscene state management.
    /// </summary>
    private static bool isInCutscene;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Called once on script initialization. Attempts to find the input listener in the scene.
    /// Sets up the input system connection for interaction management.
    /// </summary>
    private void Awake()
    {
        // Looks for any object in the scene of type InputListener.
        inputListener = FindAnyObjectByType<InputListener>();
    }

    /// <summary>
    /// Called at a fixed interval (good for physics checks).
    /// Detects nearby interactables and manages interaction UI and dialogue triggering.
    /// Handles the core interaction detection logic each physics frame.
    /// </summary>
    private void Update()
    {
        try
        {
            // Lock the interaction UI to the player's camera
            if (currentInteractable != null)
            {
                interactionUI.transform.LookAt(Camera.main.transform);
            }
            // Check if any interactable is within the detection radius
            isNearInteractable = Physics.CheckSphere(
                detectionPoint.position,
                detectionRadius,
                interactiveLayers,
                QueryTriggerInteraction.Ignore
            );

            // If no interactables are nearby, hide UI and reset interaction state
            if (!isNearInteractable)
            {
                // Hide interaction UI if it's visible
                if (interactionUI.activeSelf)
                    interactionUI.SetActive(false);

                // Allow input listener again since we're not in interaction range
                inputListener.enabled = true;

                // Clear current interactable reference
                currentInteractable = null;
                return;
            }

            // If an interactable is nearby, find the closest one
            Collider[] colliders = Physics.OverlapSphere(
                detectionPoint.position,
                detectionRadius,
                interactiveLayers
            );
            foreach (Collider col in colliders)
            {
                if (IsInInteractiveLayers(col.gameObject))
                {
                    currentInteractable = col.gameObject;
                    break; // Stop after finding the first valid interactable
                }
            }

            // Show the interaction UI if it's not already visible
            if (!interactionUI.activeSelf)
            {
                interactionUI.SetActive(true);
                interactionUI.GetComponentInChildren<TextMeshProUGUI>().text =
                    "Press " + inputListener.getKeybind("Interact").ToString();
            }
        }
        catch (System.Exception) { } // Silently handle any errors in update
    }

    #endregion

    #region Detection Methods

    /// <summary>
    /// Checks whether the given GameObject is on one of the interactive layers.
    /// Validates that detected objects are actually interactable.
    /// </summary>
    /// <param name="obj">GameObject to check.</param>
    /// <returns>True if the object's layer is within the interactiveLayers mask; false otherwise.</returns>
    private bool IsInInteractiveLayers(GameObject obj)
    {
        // Use bitwise operations to check if object's layer is in the interactive layers mask
        return (interactiveLayers.value & (1 << obj.layer)) != 0;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Property indicating if the player is near an NPC or interactable object.
    /// Provides easy access to interaction proximity state.
    /// </summary>
    public bool isNearNPC => isNearInteractable;

    #endregion

    #region Interaction Management Methods

    /// <summary>
    /// Gets the currently detected interactable GameObject.
    /// Returns the object the player can interact with.
    /// </summary>
    /// <returns>The GameObject the player is near.</returns>
    public GameObject GetCurrentInteractable()
    {
        return currentInteractable;
    }

    /// <summary>
    /// Sets the current interactable GameObject explicitly.
    /// Useful if another system needs to override the detected interactable.
    /// </summary>
    /// <param name="obj">The GameObject to set as current interactable.</param>
    public void SetCurrentInteractable(GameObject obj)
    {
        currentInteractable = obj;
    }

    /// <summary>
    /// Legacy alias for GetCurrentInteractable (kept for backward compatibility).
    /// </summary>
    /// <returns>The interactable GameObject.</returns>
    public GameObject getInteractingWith()
    {
        return currentInteractable;
    }

    /// <summary>
    /// Legacy alias for SetCurrentInteractable (kept for backward compatibility).
    /// </summary>
    /// <param name="obj">The GameObject to set as the current interactable.</param>
    public void setInteractingWith(GameObject obj)
    {
        currentInteractable = obj;
    }

    #endregion

    #region Dialogue State Management

    /// <summary>
    /// Gets the current dialogue state of the player.
    /// Indicates whether the player is currently in a conversation.
    /// </summary>
    /// <returns>True if the player is in dialogue, false otherwise.</returns>
    public bool isInDialogue()
    {
        return in_dialogue;
    }

    /// <summary>
    /// Sets the dialogue state of the player.
    /// Controls interaction availability and input handling during conversations.
    /// </summary>
    /// <param name="inDialogue">True to set player in dialogue, false to exit dialogue.</param>
    public void setInDialogue(bool inDialogue)
    {
        in_dialogue = inDialogue;
    }

    #endregion

    #region Cutscene Management
    /// <summary>
    /// Sets the global cutscene state for the player.
    /// Used by other systems to indicate cutscene status.
    /// </summary>
    /// <param name="isInCutscene">True if player is in cutscene, false otherwise.</param>
    public static void setIsInCutscene(bool isInCutscene)
    {
        playerScript.isInCutscene = isInCutscene;
    }

    /// <summary>
    /// Gets the global cutscene state for the player.
    /// Used by other systems to check cutscene status.
    /// </summary>
    /// <returns>True if player is in cutscene, false otherwise.</returns>
    public static bool getIsInCutscene()
    {
        return isInCutscene;
    }
    #endregion

    #region Debug Visualization

    /// <summary>
    /// Unity callback to draw debug gizmos in the Scene view.
    /// Draws a colored wire sphere at the detection point:
    /// Green = interactable found, Red = no interactables nearby.
    /// Helps with debugging interaction ranges and detection areas.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (detectionPoint == null)
            return;

        // Default to yellow for visualization
        Gizmos.color = Color.yellow;

#if UNITY_EDITOR
        // Check if there's an interactable nearby to decide color
        Collider[] colliders = Physics.OverlapSphere(
            detectionPoint.position,
            detectionRadius,
            interactiveLayers
        );
        bool found = false;
        foreach (var col in colliders)
        {
            if ((interactiveLayers.value & (1 << col.gameObject.layer)) != 0)
            {
                found = true;
                break;
            }
        }

        // Green = found, Red = not found
        Gizmos.color = found ? Color.green : Color.red;
#endif

        // Draw the detection sphere
        Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
    }

    #endregion
}
