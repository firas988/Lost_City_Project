using UnityEngine;

/// <summary>
/// Controls the character preview system for inventory and character customization.
/// Manages the visibility of preview camera and lighting for character inspection.
/// Provides methods to show and hide the character preview environment.
/// </summary>
public class CharacterPrevController : MonoBehaviour
{
    #region Preview Components
    /// <summary>
    /// Camera GameObject used for character preview in inventory.
    /// Positioned to provide optimal viewing angle for character inspection.
    /// </summary>
    [SerializeField]
    private GameObject inventoryCamera;

    /// <summary>
    /// Light GameObject used to illuminate the character during preview.
    /// Provides proper lighting for character appearance evaluation.
    /// </summary>
    [SerializeField]
    private GameObject light;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the preview system by hiding preview components.
    /// Ensures preview is not visible when the game starts.
    /// </summary>
    private void Awake()
    {
        // Hide preview camera and lighting initially
        inventoryCamera.SetActive(false);
        light.SetActive(false);
    }
    #endregion

    #region Preview Control
    /// <summary>
    /// Shows the character preview by activating camera and lighting.
    /// Called when player opens character customization or inventory preview.
    /// </summary>
    public void showCharacterPreview()
    {
        // Activate preview camera and lighting for character inspection
        inventoryCamera.SetActive(true);
        light.SetActive(true);
    }

    /// <summary>
    /// Hides the character preview by deactivating camera and lighting.
    /// Called when player closes character customization or inventory preview.
    /// </summary>
    public void hideCharacterPreview()
    {
        // Deactivate preview camera and lighting to hide preview
        inventoryCamera.SetActive(false);
        light.SetActive(false);
    }
    #endregion
}
