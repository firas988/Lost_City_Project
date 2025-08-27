using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelFinal", menuName = "Quests/DungeonLevelFinal")]
public class DungeonLevelFinal : StoryQuest
{
    public DungeonLevelFinal(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        if (this.isCompleted)
            return;

        GameObject.FindAnyObjectByType<DungeonManager>().openDungeonExit();
        base.CompleteQuest();
    }
}
