using UnityEngine;

[CreateAssetMenu(
    fileName = "GoToFinshAllTheWave",
    menuName = "Quests/Part2/KillWave/GoToFinshAllTheWave"
)]
public class GoToFinshAllTheWave : StoryQuest
{
    public GoToFinshAllTheWave(Quest quest)
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
