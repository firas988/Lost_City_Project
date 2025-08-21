using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MysteriousArtifact",
    menuName = "Quests/Part2/MysteriousMan/MysteriousArtifact"
)]
public class MysteriousArtifact : StoryQuest
{
    public MysteriousArtifact(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        if (!childQuests.All(quest => quest.isCompleted))
        {
            return;
        }
        Player player = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<StartPlayer>()
            .getPlayer();
        ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
        Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
            "MysteriousManFoundArtifact"
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
    }
}
