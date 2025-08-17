using UnityEngine;

[CreateAssetMenu(fileName = "TempleKillAllGaurds", menuName = "Quests/TempleKillAllGaurds")]
public class TempleKillAllGaurds : StoryQuest
{
    public TempleKillAllGaurds(Quest quest)
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
