using UnityEngine;

[CreateAssetMenu(fileName = "GoToActivateTheKey", menuName = "Quests/Part2/Key/GoToActivateTheKey")]
public class GoToActivateTheKey : StoryQuest
{
    public GoToActivateTheKey(Quest quest)
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
