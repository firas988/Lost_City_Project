using UnityEngine;

[CreateAssetMenu(fileName = "GoToCastel", menuName = "Quests/Part2/Castel/GoToCastel")]
public class GoToCastel : StoryQuest
{
    public GoToCastel(Quest quest)
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
