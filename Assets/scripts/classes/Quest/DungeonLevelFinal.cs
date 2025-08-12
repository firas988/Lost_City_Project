using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelFinal", menuName = "Quests/DungeonLevelFinal")]
public class DungeonLevelFinal : StoryQuest
{
    public DungeonLevelFinal(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        if (this.childQuests.All(quest => quest.isCompleted))
        {
            base.CompleteQuest();
        }
    }
}
