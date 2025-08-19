using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelFinal", menuName = "Quests/DungeonLevelFinal")]
public class DungeonLevelFinal : StoryQuest
{
    [SerializeField]
    private GameObject cutscene;

    [SerializeField]
    private Transform cutscenePosition;

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
            Instantiate(cutscene, cutscenePosition.position, cutscenePosition.rotation);
            base.CompleteQuest();
        }
    }
}
