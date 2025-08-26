using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Quest for completing all enemy waves in Part 2 of the story
/// </summary>
[CreateAssetMenu(fileName = "FinshAllTheWave", menuName = "Quests/Part2/KillWave/FinshAllTheWave")]
public class FinshAllTheWave : StoryQuest
{
    #region Constructor
    /// <summary>
    /// Constructor that initializes the FinshAllTheWave quest with the base quest data
    /// </summary>
    /// <param name="quest">The base quest data to initialize with</param>
    public FinshAllTheWave(Quest quest)
        : base(quest) { }
    #endregion

    #region Quest Progression
    /// <summary>
    /// Handles quest progression logic (currently no progression needed)
    /// </summary>
    public override void progress()
    {
        // This quest doesn't require progression tracking
        // Quest completion is triggered when all enemy waves are defeated
        return;
    }
    #endregion

    #region Quest Completion
    /// <summary>
    /// Completes the quest by cleaning up map pieces and triggering cutscene
    /// </summary>
    public override void CompleteQuest()
    {
        // Remove the map piece that was blocking progress
        Destroy(GameObject.FindWithTag("FinshAllTheWaveMapPiece"));

        // Play the ghost cutscene to continue the story
        GameObject.FindWithTag("GhostCutScene").GetComponent<PlayableDirector>().Play();

        // Complete the base quest
        base.CompleteQuest();
    }
    #endregion
}
