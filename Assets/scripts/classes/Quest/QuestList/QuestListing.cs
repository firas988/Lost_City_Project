using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of individual quest information in the quest list UI.
/// Handles quest name, description, and progress display with automatic layout updates.
/// </summary>
public class QuestListing : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Unique identifier for this quest listing.
    /// </summary>
    [SerializeField]
    private int questId;

    #endregion

    #region Private Fields

    /// <summary>
    /// Text component for displaying the quest name.
    /// </summary>
    private TMP_Text questName;

    /// <summary>
    /// Text component for displaying the quest description.
    /// </summary>
    private TMP_Text questDescription;

    /// <summary>
    /// Text component for displaying the quest progress.
    /// </summary>
    private TMP_Text questProgress;

    /// <summary>
    /// Reference to the quest data to be displayed.
    /// </summary>
    private Quest questToAdd;

    /// <summary>
    /// Flag indicating if this quest was just added to the UI.
    /// </summary>
    private bool justAdded = false;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the quest listing by finding UI components and setting initial values.
    /// </summary>
    void Awake()
    {
        // Find and assign UI text components
        questName = transform.Find("Details").Find("Name").GetComponent<TMP_Text>();
        questDescription = transform.Find("Details").Find("Description").GetComponent<TMP_Text>();

        // Find and assign progress text component if available
        Transform progress = transform.Find("Progress");
        if (progress != null)
        {
            questProgress = progress.GetComponentInChildren<TMP_Text>();
            questProgress.text = "0/" + questToAdd?.QuestTarget.Count;
        }

        // Set initial quest information if available
        if (questToAdd != null)
        {
            questName.text = questToAdd.GetQuestName();
            questDescription.text = questToAdd.GetDescription();
        }

        justAdded = true;
    }

    /// <summary>
    /// Updates quest information display and handles layout rebuilding for newly added quests.
    /// </summary>
    void Update()
    {
        // Update quest information display
        if (questToAdd != null)
        {
            questName.text = questToAdd.GetQuestName();
            questDescription.text = questToAdd.GetDescription();
            questProgress.text = questToAdd.GetProgress();
        }

        // Handle layout rebuilding for newly added quests
        if (justAdded)
        {
            // Force layout rebuild to ensure proper positioning
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                transform.parent.GetComponent<RectTransform>()
            );
            justAdded = false;
        }
    }

    #endregion

    #region Quest Management

    /// <summary>
    /// Sets the quest data to be displayed in this listing.
    /// </summary>
    /// <param name="quest">The quest to display.</param>
    public void SetQuestToAdd(Quest quest)
    {
        questToAdd = quest;
    }

    /// <summary>
    /// Sets the quest ID and updates the GameObject name.
    /// </summary>
    /// <param name="id">The quest ID to set.</param>
    public void SetQuestId(int id)
    {
        gameObject.name = id.ToString();
    }

    #endregion

    #region UI Update Methods

    /// <summary>
    /// Sets the quest name text and rebuilds the layout.
    /// </summary>
    /// <param name="name">The quest name to display.</param>
    public void SetName(string name)
    {
        questName.text = name;

        // Rebuild layout to accommodate text changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Sets the quest description text and rebuilds the layout.
    /// </summary>
    /// <param name="description">The quest description to display.</param>
    public void SetDescription(string description)
    {
        questDescription.text = description;

        // Rebuild layout to accommodate text changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Sets the quest progress text and rebuilds the layout.
    /// </summary>
    /// <param name="progress">The quest progress to display.</param>
    public void SetProgress(string progress)
    {
        questProgress.text = progress;

        // Rebuild layout to accommodate text changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    #endregion
}
