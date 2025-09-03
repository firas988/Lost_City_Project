using UnityEngine;

/// <summary>
/// Story quest that opens a dungeon entrance door upon completion.
/// Manages dungeon access by activating the entrance door GameObject.
/// </summary>
[CreateAssetMenu(fileName = "TempleFindMapPart", menuName = "Quests/TempleFindMapPart")]
public class TempleFindMapPart : StoryQuest
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the TempleFindMapPart class.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public TempleFindMapPart(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the quest and opens the dungeon entrance door.
    /// </summary>
    public override void CompleteQuest()
    {
        GameObject.Find("dungeonEntrance").transform.Find("openDoor").gameObject.SetActive(true);
        base.CompleteQuest();
    }

    #endregion
}
