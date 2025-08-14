using UnityEngine;

[CreateAssetMenu(fileName = "ActivateTheKey", menuName = "Quests/Part2/Key/ActivateTheKey")]
public class ActivateTheKey : StoryQuest
{
    public ActivateTheKey(Quest quest)
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
