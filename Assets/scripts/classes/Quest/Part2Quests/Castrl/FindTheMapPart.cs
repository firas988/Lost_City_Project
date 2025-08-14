using UnityEngine;

[CreateAssetMenu(fileName = "FindTheMapPart", menuName = "Quests/Part2/Castel/FindTheMapPart")]
public class FindTheMapPart : StoryQuest
{
    public FindTheMapPart(Quest quest)
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
