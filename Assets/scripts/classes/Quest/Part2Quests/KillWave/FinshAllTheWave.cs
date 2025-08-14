using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "FinshAllTheWave", menuName = "Quests/Part2/KillWave/FinshAllTheWave")]
public class FinshAllTheWave : StoryQuest
{
    public FinshAllTheWave(Quest quest)
        : base(quest) { }

    public override void progress()
    {
        return;
    }

    public override void CompleteQuest()
    {
        Destroy(GameObject.FindWithTag("FinshAllTheWaveMapPiece"));
        GameObject.FindWithTag("GhostCutScene").GetComponent<PlayableDirector>().Play();
        base.CompleteQuest();
    }
}
