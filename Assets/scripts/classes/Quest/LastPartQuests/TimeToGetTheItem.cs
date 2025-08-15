using UnityEngine;

[CreateAssetMenu(fileName = "TimeToGetTheItem", menuName = "Quests/LastPart/TimeToGetTheItem")]
public class TimeToGetTheItem : StoryQuest
{
    public TimeToGetTheItem(Quest quest)
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
