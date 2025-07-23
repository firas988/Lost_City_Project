using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI countText;

    public void ShowTooltip(Item item, Vector2 position)
    {
        tooltipPanel.SetActive(true);

        itemNameText.text = item.itemName + "\n" + item.getDescription();
        countText.text = "Max Stack: " + item.maxStack.ToString();

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        Vector2 panelSize = tooltipRect.sizeDelta * tooltipPanel.transform.lossyScale;

        Vector2 adjustedPosition = position;

        adjustedPosition.x -= panelSize.x - 25f;

        if (adjustedPosition.x < 0)
        {
            adjustedPosition.x = position.x;
        }

        if (adjustedPosition.y - panelSize.y < 0)
        {
            adjustedPosition.y = panelSize.y;
        }

        if (adjustedPosition.y > Screen.height - panelSize.y)
        {
            adjustedPosition.y = Screen.height - panelSize.y;
        }

        tooltipPanel.transform.position = adjustedPosition;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
