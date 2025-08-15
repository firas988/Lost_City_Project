using UnityEngine;

[CreateAssetMenu(fileName = "KillTheFinalBoss", menuName = "Quests/LastPart/KillTheFinalBoss")]
public class KillTheFinalBoss : StoryQuest
{
    public KillTheFinalBoss(Quest quest)
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
