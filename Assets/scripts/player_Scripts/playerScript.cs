using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles player interaction detection with nearby interactable objects using a physics sphere check,
/// and manages the interaction UI and dialogue state.
/// </summary>
public class playerScript : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("The Transform used as the center point of the detection sphere.")]
    [SerializeField]
    private Transform detectionPoint;

    [Tooltip("The radius of the detection sphere.")]
    [SerializeField]
    private float detectionRadius = 2f;

    [Tooltip("Layer mask used to filter interactable objects.")]
    [SerializeField]
    private LayerMask interactiveLayers;

    [Header("UI Settings")]
    [Tooltip("UI element to be shown or hidden when near an interactable.")]
    [SerializeField]
    private UIBehaviour interactionUI;

    // Reference to the input listener script (e.g., handling player input).
    private InputListener inputListener;

    // Tracks if the player is currently in a dialogue session.
    private bool in_dialogue;

    // Stores the currently detected interactable GameObject.
    private GameObject currentInteractable;

    private bool isNearInteractable;

    /// <summary>
    /// Called once on script initialization. Attempts to find the input listener in the scene.
    /// </summary>
    private void Awake()
    {
        // Looks for any object in the scene of type InputListener.
        inputListener = FindAnyObjectByType<InputListener>();
    }

    /// <summary>
    /// Called at a fixed interval (good for physics checks).
    /// Detects nearby interactables and manages interaction UI and dialogue triggering.
    /// </summary>
    private void FixedUpdate()
    {
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
            // Hide interaction UI if it�s visible
            // if (interactionUI.enabled)
            //     UIcontroller.ToggleUI(interactionUI);

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
        Debug.Log(currentInteractable);

        // // Show the interaction UI if it's not already visible
        // if (!interactionUI.enabled)
        //     UIcontroller.ToggleUI(interactionUI);
    }

    /// <summary>
    /// Checks whether the given GameObject is on one of the interactive layers.
    /// </summary>
    /// <param name="obj">GameObject to check.</param>
    /// <returns>True if the object's layer is within the interactiveLayers mask; false otherwise.</returns>
    private bool IsInInteractiveLayers(GameObject obj)
    {
        return (interactiveLayers.value & (1 << obj.layer)) != 0;
    }

    public bool isNearNPC => isNearInteractable;

    /// <summary>
    /// Gets the currently detected interactable GameObject.
    /// </summary>
    /// <returns>The GameObject the player is near.</returns>
    public GameObject GetCurrentInteractable()
    {
        return currentInteractable;
    }

    /// <summary>
    /// Sets the current interactable GameObject explicitly.
    /// Useful if another system needs to override it.
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

    public bool isInDialogue()
    {
        return in_dialogue;
    }

    public void setInDialogue(bool inDialogue)
    {
        in_dialogue = inDialogue;
    }

    /// <summary>
    /// Unity callback to draw debug gizmos in the Scene view.
    /// Draws a colored wire sphere at the detection point:
    /// Green = interactable found, Red = no interactables nearby.
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
}
