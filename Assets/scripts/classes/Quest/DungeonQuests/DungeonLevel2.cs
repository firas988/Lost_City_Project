using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel2", menuName = "Quests/DungeonLevel2")]
public class DungeonLevel2 : StoryQuest
{
    public DungeonLevel2(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        if (this.isCompleted)
            return;

        GameObject.FindAnyObjectByType<DungeonManager>().NextRoom();
        base.CompleteQuest();
    }
}
