using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel1", menuName = "Quests/DungeonLevel1")]
public class DungeonLevel1 : StoryQuest
{
    public DungeonLevel1(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        if (this.isCompleted)
            return;
        GameObject.FindAnyObjectByType<DungeonManager>().NextRoom();
        base.CompleteQuest();
    }
}
