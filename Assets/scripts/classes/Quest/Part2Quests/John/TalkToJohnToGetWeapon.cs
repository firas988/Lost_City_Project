using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "TalkToJohnToGetWeapon",
    menuName = "Quests/Part2/John/TalkToJohnToGetWeapon"
)]
public class TalkToJohnToGetWeapon : StoryQuest
{
    public TalkToJohnToGetWeapon(Quest quest)
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
            "TalkToJohnToKnowWhereToGo"
        );
        (
            (TalkativeNpc)GameObject.FindWithTag("John").GetComponent<StartNpc>().GetNpcsInstance()
        ).setDialogue(dialogueData);
        RewardManager rewardManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<RewardManager>();
        rewardManager.GiveReward(base.Reward);

        base.CompleteQuest();

        GameObject john = GameObject.FindWithTag("John");
        QuestGiver questGiver = (QuestGiver)john.GetComponent<StartNpc>().GetNpcsInstance();
        questGiver.setQuestToGive(player.getCurrentMainQuest(), john);
    }
}
