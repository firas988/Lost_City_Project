using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteriousManQuestChecker : MonoBehaviour
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

        if (checkIfTheQuestIsMysteriousManQuestCompleted())
        {
            Player player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
            ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
            QuestGiver questGiver = (QuestGiver)GetComponent<StartNpc>().GetNpcsInstance();

            if (checkIfTheQuestIsMysteriousArtifactCompleted())
            {
                if (checkIfTheQuestIsMysteriousManQuestFoundArtifactCompleted())
                {
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "MysteriousManWhereToGo"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );
                    Destroy(GameObject.FindWithTag("MysteriousManMapPiece"));
                    if (checkIfTheQuestIsMysteriousManQuestWhereToGoCompleted())
                    {
                        questGiver.setQuestToGive(null, this.gameObject);
                    }
                    else
                    {
                        questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
                    }
                }
                else
                {
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        "MysteriousManFoundArtifact"
                    );
                    ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                        dialogueData
                    );
                    questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
                }
            }
            else
            {
                Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                    "MysteriousManDidntFindArtifact"
                );
                ((TalkativeNpc)GetComponent<StartNpc>().GetNpcsInstance()).setDialogue(
                    dialogueData
                );
                questGiver.setQuestToGive(player.getCurrentMainQuest(), this.gameObject);
            }
        }
        Debug.Log("MysteriousManQuestChecker");
    }

    private bool checkIfTheQuestIsMysteriousManQuestCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuest)))
        {
            return true;
        }
        return false;
    }

    private bool checkIfTheQuestIsMysteriousArtifactCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousArtifact)))
        {
            return true;
        }
        return false;
    }

    private bool checkIfTheQuestIsMysteriousManQuestFoundArtifactCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuestFoundArtifact)))
        {
            return true;
        }
        return false;
    }

    private bool checkIfTheQuestIsMysteriousManQuestWhereToGoCompleted()
    {
        if (questManager.checkingCompletedStoryQuest(typeof(MysteriousManQuestWhereToGo)))
        {
            return true;
        }
        return false;
    }
}
