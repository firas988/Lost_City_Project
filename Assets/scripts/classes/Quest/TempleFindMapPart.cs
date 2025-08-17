using UnityEngine;

[CreateAssetMenu(fileName = "TempleFindMapPart", menuName = "Quests/TempleFindMapPart")]
public class TempleFindMapPart : StoryQuest
{
    public TempleFindMapPart(Quest quest)
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
