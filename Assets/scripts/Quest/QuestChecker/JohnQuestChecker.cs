using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnQuestChecker : MonoBehaviour
{
    private QuestManager questManager;

    private string gameManagerTag = "GameManager";

    private void Awake()
    {
        questManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<QuestManager>();

        StartCoroutine(checkIfTheQuestIsCompleted());
    }

    private IEnumerator checkIfTheQuestIsCompleted()
    {
        yield return new WaitUntil(() => questManager.IsReadyToStartQuest);

        if (checkIfTheQuestIsTalkToJohnToGetWeaponCompleted())
        {
            Player player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
            ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
            if (checkIfTheQuestIsTalkToJohnToKnowWhereToGoCompleted())
            {
                if (checkIfTheQuestIsTimeToGetTheItemCompleted())
                {
                    Destroy(this.gameObject);
                    yield break;
                }
                else
                {
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "TalkToJohnToKnowWhereToGo"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );
                    QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();
                    questGiver.setQuestToGive(null, this.gameObject);
                }
            }
            else
            {
                Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                    "TalkToJohnToKnowWhereToGo"
                );
                ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                    dialogueData
                );
                QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();
                questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
            }
        }
    }

    private bool checkIfTheQuestIsTalkToJohnToGetWeaponCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TalkToJohnToGetWeapon)))
        {
            return true;
        }
        return false;
    }

    private bool checkIfTheQuestIsTalkToJohnToKnowWhereToGoCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TalkToJohnToKnowWhereToGo)))
        {
            return true;
        }
        return false;
    }

    private bool checkIfTheQuestIsTimeToGetTheItemCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(TimeToGetTheItem)))
        {
            return true;
        }
        return false;
    }
}
