using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel1", menuName = "Quests/DungeonLevel1")]
public class DungeonLevel1 : StoryQuest
{
    public DungeonLevel1(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        if (this.isCompleted)
            return;

        Debug.Log("DungeonLevel1 CompleteQuest");
        if (childQuests == null || childQuests.All(quest => quest.isCompleted))
        {
            GameObject.Find("dungeon").GetComponent<DungeonManager>().NextRoom();

            base.CompleteQuest();
        }
    }
}
