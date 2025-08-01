using UnityEngine;

[CreateAssetMenu(fileName = "MysteriousManQuest", menuName = "Quests/MysteriousManQuest")]
public class MysteriousManQuest : StoryQuest
{
    [SerializeField]
    private FindQuest findArtifactQuest;

    public MysteriousManQuest(Quest quest)
        : base(quest) {
            findArtifactQuest.setParentQuest(this);
        }

    public override void progress()
    {
        if (findArtifactQuest.isCompleted)
        {
            findArtifactQuest.progress();
        }
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
    }
}
