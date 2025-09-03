using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Story quest specific to Robert that includes a cutscene upon completion.
/// Manages cutscene spawning and quest completion logic.
/// </summary>
[CreateAssetMenu(fileName = "RobertQuest", menuName = "Quests/RobertQuest")]
public class RobertQuest : StoryQuest
{
    #region Serialized Fields

    [SerializeField]
    private GameObject cutScenePrefab;

    [SerializeField]
    private Transform cutScenePosition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the RobertQuest class.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public RobertQuest(Quest quest)
        : base(quest) { }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the quest and spawns a cutscene if not already completed.
    /// </summary>
    public override void CompleteQuest()
    {
        if (
            !GameObject
                .FindAnyObjectByType<QuestManager>()
                .checkingCompletedStoryQuest(this.GetType())
        )
        {
            base.CompleteQuest();
        }

        Instantiate(cutScenePrefab, cutScenePosition.position, cutScenePosition.rotation);
    }

    #endregion
}
