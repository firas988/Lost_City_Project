using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MysteriousManQuestFoundArtifact",
    menuName = "Quests/Part2/MysteriousMan/MysteriousManQuestFoundArtifact"
)]
public class MysteriousManQuestFoundArtifact : StoryQuest
{
    public MysteriousManQuestFoundArtifact(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "MysteriousManWhereToGo"
        );
        (
            (TalkativeNpc)
                GameObject.Find("MysteriousMan").GetComponent<StartNpc>().GetNpcsInstance()
        ).setDialogue(dialogueData);
        base.CompleteQuest();
        GameObject mysteriousMan = GameObject.Find("MysteriousMan");
        QuestGiver questGiver = (QuestGiver)
            mysteriousMan.GetComponent<StartNpc>().GetNpcsInstance();
        questGiver.setQuestToGive(player.getCurrentMainQuest(), mysteriousMan);
        Destroy(GameObject.FindWithTag("MysteriousManMapPiece"));
    }
}
