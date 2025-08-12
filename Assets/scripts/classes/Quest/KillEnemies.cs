using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "KillEnemies", menuName = "Quests/KillEnemies")]
public class KillEnemies : StoryQuest
{
    public KillEnemies(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        if (this.childQuests.All(quest => quest.isCompleted))
        {
            return;
        }

        base.CompleteQuest();
    }
}
