using UnityEngine;

[CreateAssetMenu(
    fileName = "TalkToJohnToKnowWhereToGo",
    menuName = "Quests/Part2/John/TalkToJohnToKnowWhereToGo"
)]
public class TalkToJohnToKnowWhereToGo : StoryQuest
{
    public TalkToJohnToKnowWhereToGo(Quest quest)
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
