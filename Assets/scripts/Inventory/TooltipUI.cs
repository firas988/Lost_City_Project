using TMPro;
using UnityEngine;

/// <summary>
/// Manages the display of item tooltips in the inventory system, providing detailed
/// information about items when hovering over them. Handles tooltip positioning to
/// ensure it remains visible within the screen boundaries.
/// </summary>
public class TooltipUI : MonoBehaviour
{
    #region UI Components
    /// <summary>The main tooltip panel that contains all tooltip information.</summary>
    [SerializeField]
    private GameObject tooltipPanel;

    /// <summary>Text component displaying the item name and description.</summary>
    [SerializeField]
    private TextMeshProUGUI itemNameText;

    /// <summary>Text component displaying the item description.</summary>
    /// 
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    /// <summary>
    /// Text component displaying the items price
    /// </summary>
    [SerializeField] private TextMeshProUGUI price;

    /// <summary>Text component displaying the maximum stack size information.</summary>
    [SerializeField] private TextMeshProUGUI countText;
    #endregion

    #region Tooltip Display
    /// <summary>
    /// Shows the tooltip for a specific item at the given position.
    /// Calculates optimal positioning to keep the tooltip visible on screen.
    /// </summary>
    /// <param name="item">The item to display information for.</param>
    /// <param name="position">The desired position for the tooltip (usually mouse position).</param>
    // COMPLEXITY ANALYSIS: ShowTooltip() - O(1)
    public void ShowTooltip(Item item, Vector2 position)
    {
        // Activate the tooltip panel
        tooltipPanel.SetActive(true);

        // Set tooltip content
        itemNameText.text = item.getItemName() + "\n" + item.getDescription();
   
        countText.text = "Max Stack: " + item.getMaxStack().ToString();
        // Get tooltip panel dimensions for positioning calculations
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Vector2 panelSize = tooltipRect.sizeDelta * tooltipPanel.transform.lossyScale;

        // Start with the desired position
        Vector2 adjustedPosition = position;
        // Change the position of the tooltip
        changePosition(adjustedPosition, panelSize, position);
    }

    public void ShowToolTip(Skill skill, Vector2 position)
    {
        //Activate the toolTip panel
        tooltipPanel.SetActive(true);
        
        itemNameText.text =skill.SkillName;
        price.text = "Price: " + skill.Cost;
        descriptionText.text = "Gives " + skill.Bonus + " " + skill.SkillType;

        // Get tooltip panel dimensions for positioning calculations
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Vector2 panelSize = tooltipRect.sizeDelta * tooltipPanel.transform.lossyScale;

        // Start with the desired position
        Vector2 adjustedPosition = position;
        // Change the position of the tooltip
        changePosition(adjustedPosition, panelSize, position);


    }

    /// <summary>
    /// Changes the position of the tooltip to prevent it from going off-screen.
    /// </summary>
    /// <param name="adjustedPosition">The adjusted position of the tooltip.</param>
    /// <param name="panelSize">The size of the tooltip panel.</param>
    /// <param name="position">The desired position of the tooltip.</param>
    // COMPLEXITY ANALYSIS: changePosition() - O(1)
    private void changePosition(Vector2 adjustedPosition, Vector2 panelSize, Vector2 position)
    {
        // Adjust horizontal position to prevent tooltip from going off-screen right
        adjustedPosition.x -= panelSize.x - 25f;

        // If adjusted position goes off-screen left, revert to original position
        if (adjustedPosition.x < 0)
        {
            adjustedPosition.x = position.x;
        }

        // Adjust vertical position to prevent tooltip from going off-screen bottom
        if (adjustedPosition.y - panelSize.y < 0)
        {
            adjustedPosition.y = panelSize.y;
        }

        // Adjust vertical position to prevent tooltip from going off-screen top
        if (adjustedPosition.y > Screen.height - panelSize.y)
        {
            adjustedPosition.y = Screen.height - panelSize.y;
        }
        // Set the final tooltip position
        tooltipPanel.transform.position = adjustedPosition;
    }

    /// <summary>
    /// Hides the tooltip by deactivating the tooltip panel.
    /// </summary>
    // COMPLEXITY ANALYSIS: HideTooltip() - O(1)
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    #endregion
}
