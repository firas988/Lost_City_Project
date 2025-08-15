using UnityEngine;

[CreateAssetMenu(fileName = "GoToTheCenter", menuName = "Quests/LastPart/GoToTheCenter")]
public class GoToTheCenter : StoryQuest
{
    public GoToTheCenter(Quest quest)
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
