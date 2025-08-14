using UnityEngine;

[CreateAssetMenu(fileName = "GoToBridge", menuName = "Quests/Part2/Bridge/GoToBridge")]
public class GoToBridge : StoryQuest
{
    public GoToBridge(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
    }
}
