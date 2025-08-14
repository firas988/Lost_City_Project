using UnityEngine;

[CreateAssetMenu(
    fileName = "MysteriousManQuestWhereToGo",
    menuName = "Quests/Part2/MysteriousMan/MysteriousManQuestWhereToGo"
)]
public class MysteriousManQuestWhereToGo : StoryQuest
{
    public MysteriousManQuestWhereToGo(Quest quest)
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
