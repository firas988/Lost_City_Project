using UnityEngine;

/// <summary>
/// Manages the minimap arrow and dot indicators for navigation purposes.
/// Displays either a dot (when target is visible) or arrow (when target is off-screen) on the minimap.
/// Handles positioning, rotation, and visibility based on target location relative to the player.
/// </summary>
public class MinimapArrow : MonoBehaviour
{
    #region Camera References
    /// <summary>
    /// Tag used to identify the minimap camera GameObject in the scene.
    /// </summary>
    private string minimapCameraTag = "MiniMapCamera";

    /// <summary>
    /// Reference to the minimap camera for coordinate transformations.
    /// Used to convert world positions to viewport coordinates.
    /// </summary>
    private Camera minimapCamera;
    #endregion

    #region UI Elements
    /// <summary>
    /// Reference to the minimap UI rectangle for coordinate calculations.
    /// Used to determine the minimap boundaries and positioning.
    /// </summary>
    [SerializeField]
    private RectTransform minimapRect;

    /// <summary>
    /// Reference to the arrow icon UI element for off-screen target indication.
    /// Shows direction to target when it's outside the minimap view.
    /// </summary>
    [SerializeField]
    private RectTransform arrowIcon;

    /// <summary>
    /// Reference to the dot icon UI element for on-screen target indication.
    /// Shows target location when it's visible within the minimap.
    /// </summary>
    [SerializeField]
    private RectTransform dotIcon;
    #endregion

    #region Player Reference
    /// <summary>
    /// Tag used to identify the player GameObject in the scene.
    /// </summary>
    private string playerTag = "Player";

    /// <summary>
    /// Reference to the player's transform for position calculations.
    /// Used to determine direction from player to target.
    /// </summary>
    private Transform player;
    #endregion

    #region Target Configuration
    /// <summary>
    /// The world position of the target to navigate towards.
    /// Set to (0,0,0) when no target is active.
    /// </summary>
    [SerializeField]
    private Vector3 targetPosition;

    /// <summary>
    /// Offset from the edge of the minimap for arrow positioning.
    /// Controls how close to the edge the arrow appears when target is off-screen.
    /// </summary>
    [Range(0.1f, 1f)]
    [SerializeField]
    private float edgeOffset = 0.9f;
    #endregion

    #region State Management
    /// <summary>
    /// Indicates whether the target is set to the zero position (no target).
    /// Used to skip processing when no target is active.
    /// </summary>
    private bool isTargetZero = true;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by setting default target and hiding UI elements.
    /// Sets up initial state with no target and hidden indicators.
    /// </summary>
    void Awake()
    {
        // Set default target to zero position (no target)
        targetPosition = new Vector3(0, 0, 0);
        isTargetZero = true;

        // Hide both UI elements initially
        arrowIcon.gameObject.SetActive(false);
        dotIcon.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the minimap indicators each frame based on target position.
    /// Handles coordinate transformations and UI element positioning/rotation.
    /// </summary>
    void Update()
    {
        // Skip processing if no target is set
        if (isTargetZero)
        {
            return;
        }

        // Safely get player reference
        try
        {
            player = GameObject.FindWithTag(playerTag).transform;
        }
        catch (System.Exception)
        {
            return;
        }

        // Get minimap camera reference
        minimapCamera = GameObject.FindWithTag(minimapCameraTag).GetComponent<Camera>();

        // Convert target world position to viewport coordinates
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(targetPosition);

        // Convert viewport coordinates to minimap UI coordinates
        Vector2 minimapPos = new Vector2(
            (viewportPos.x - 0.5f) * minimapRect.sizeDelta.x,
            (viewportPos.y - 0.5f) * minimapRect.sizeDelta.y
        );

        // Calculate the radius for edge detection
        float radius = (minimapRect.sizeDelta.x / 2f) * edgeOffset;

        // Check if target is inside the minimap view
        bool isInside = minimapPos.magnitude <= radius && viewportPos.z > 0;

        if (isInside)
        {
            // Target is visible - show dot at target position
            dotIcon.gameObject.SetActive(true);
            arrowIcon.gameObject.SetActive(false);
            dotIcon.anchoredPosition = minimapPos;
        }
        else
        {
            // Target is off-screen - show arrow pointing to target
            dotIcon.gameObject.SetActive(false);
            arrowIcon.gameObject.SetActive(true);

            // Position arrow at edge of minimap
            minimapPos = minimapPos.normalized * radius;
            arrowIcon.anchoredPosition = minimapPos;

            // Calculate direction from player to target
            Vector3 dir = (targetPosition - player.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            // Rotate arrow to point in correct direction
            float cameraYRotation = minimapCamera.transform.eulerAngles.y;
            arrowIcon.localRotation = Quaternion.Euler(0, 0, -(angle - cameraYRotation));
        }
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Sets a new target position for the minimap navigation.
    /// Updates UI element visibility and state based on target position.
    /// </summary>
    /// <param name="newTarget">The new world position to navigate towards.</param>
    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;

        // Check if target is set to zero (no target)
        if (newTarget == new Vector3(0, 0, 0))
        {
            // No target - hide indicators and set state
            isTargetZero = true;
            arrowIcon.gameObject.SetActive(false);
            dotIcon.gameObject.SetActive(false);
        }
        else
        {
            // Target set - show indicators and update state
            isTargetZero = false;
            arrowIcon.gameObject.SetActive(true);
            dotIcon.gameObject.SetActive(true);
        }
    }
    #endregion
}
