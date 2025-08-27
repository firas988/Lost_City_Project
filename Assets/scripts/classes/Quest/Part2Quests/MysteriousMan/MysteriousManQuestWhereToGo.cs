using UnityEngine;

/// <summary>
/// Quest for the Mysterious Man to guide the player on where to go next in Part 2 of the story
/// </summary>
[CreateAssetMenu(
    fileName = "MysteriousManQuestWhereToGo",
    menuName = "Quests/Part2/MysteriousMan/MysteriousManQuestWhereToGo"
)]
public class MysteriousManQuestWhereToGo : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the MysteriousManQuestWhereToGo quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public MysteriousManQuestWhereToGo(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the MysteriousManQuestWhereToGo quest
    /// </summary>
    public override void CompleteQuest()
    {
        // Call base class completion logic
        base.CompleteQuest();
    }
    #endregion
}
