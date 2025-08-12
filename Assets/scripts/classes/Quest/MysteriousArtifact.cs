using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "MysteriousArtifact", menuName = "Quests/MysteriousArtifact")]
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
        Debug.Log(player.getCurrentMainQuest().GetDescription());
        GameObject mysteriousMan = GameObject.Find("MysteriousMan");
        QuestGiver questGiver = (QuestGiver)
            mysteriousMan.GetComponent<StartNpc>().GetNpcsInstance();
        questGiver.setQuestToGive(player.getCurrentMainQuest(), mysteriousMan);

    }
}
