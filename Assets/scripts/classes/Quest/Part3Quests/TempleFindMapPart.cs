using UnityEngine;

[CreateAssetMenu(fileName = "TempleFindMapPart", menuName = "Quests/TempleFindMapPart")]
public class TempleFindMapPart : StoryQuest
{
    public TempleFindMapPart(Quest quest)
        : base(quest) { }

    public override void CompleteQuest()
    {
        GameObject.Find("dungeonEntrance").transform.Find("openDoor").gameObject.SetActive(true);
        base.CompleteQuest();
    }
}
