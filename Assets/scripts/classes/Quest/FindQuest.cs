using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Quest type that requires the player to find and interact with specific objects.
/// Tracks found object count and marks quest as complete when target is reached.
/// </summary>
[CreateAssetMenu(fileName = "FindQuest", menuName = "Quests/ExpQuest/FindQuest")]
[System.Serializable]
public class FindQuest : Quest
{
    public FindQuest(FindQuest quest)
        : base(quest)
    {
        this.found = quest.found;
        this.findTarget = quest.findTarget;
        this.findTargetPrefab = quest.findTargetPrefab;
        this.findTargetTransform = quest.findTargetTransform;
    }

    /// <summary>
    /// Current number of target objects found by the player.
    /// </summary>
    [SerializeField]
    private int found;

    /// <summary>
    /// Target number of objects that must be found to complete this quest.
    /// </summary>
    [SerializeField]
    private int findTarget;

    /// <summary>
    /// Prefab of the object that the player needs to find for this quest.
    /// </summary>
    [SerializeField]
    private GameObject findTargetPrefab;

    /// <summary>
    /// Transform position where the find target should be spawned.
    /// </summary>
    [SerializeField]
    private Transform findTargetTransform;

    /// <summary>
    /// Increments the found count and checks if the quest is complete.
    /// Called when the player successfully finds and interacts with a target object.
    /// </summary>
    override public void progress()
    {
        found = Mathf.Min(found + 1, findTarget);

        Debug.Log(this.found);

        // Check if the quest is complete based on found count
        if (found == findTarget)
        {
            this.completed = true;

            if (ParentQuest != null)
            {
                ParentQuest.CompleteQuest();
            }
        }
        else
            this.completed = false;
    }

    public string GetProgress()
    {
        return found + "/" + findTarget;
    }
}
