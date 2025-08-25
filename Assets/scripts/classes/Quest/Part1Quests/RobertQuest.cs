using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RobertQuest", menuName = "Quests/RobertQuest")]
public class RobertQuest : StoryQuest
{
    [SerializeField]
    private GameObject cutScenePrefab;

    [SerializeField]
    private Transform cutScenePosition;

    public RobertQuest(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        base.CompleteQuest();

        Instantiate(cutScenePrefab, cutScenePosition.position, cutScenePosition.rotation);
    }

    public override void progress()
    {
        return;
    }
}
