using UnityEngine;

[CreateAssetMenu(fileName = "KillAllTheEnemyInTheCastel", menuName = "Quests/Part2/Castel/KillAllTheEnemyInTheCastel")]
public class KillAllTheEnemyInTheCastel : StoryQuest
{
    public KillAllTheEnemyInTheCastel(Quest quest)
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
