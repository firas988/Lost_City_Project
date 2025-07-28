using UnityEngine;

[CreateAssetMenu(fileName = "RobertQuest", menuName = "Quests/RobertQuest")]
public class RobertQuest : StoryQuest
{
   

    public RobertQuest(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
    }

    public override void progress()
    {
        return;
    }
}
