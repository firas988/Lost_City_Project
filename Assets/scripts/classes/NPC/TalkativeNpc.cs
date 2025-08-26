using System.Collections.Generic;
using UnityEngine;

public class TalkativeNpc : NPC
{
    private Dictionary<string, Dialogue> dialogues;
    public string start;

    public TalkativeNpc(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float speed,
        float maxSpeed,
        Dictionary<string, Dialogue> dialogues,
        string start
    )
        : base(
            id,
            name,
            type,
            walkRadius,
            areaMask,
            waitTimeRange,
            navMeshAreaName,
            speed,
            maxSpeed
        )
    {
        this.dialogues = dialogues;
        this.start = start;
    }

    public Dictionary<string, Dialogue> getDialogues()
    {
        return this.dialogues;
    }

    public void setDialogue(Dictionary<string, Dialogue> dialogues)
    {
        this.dialogues = dialogues;
    }

    /// <summary>
    /// Responds to a dialogue interaction by providing the dialogue text and options.
    /// </summary>
    /// <param name="dialogue">The key identifying the dialogue to respond to.</param>
    /// <param name="options">The array of response options for the dialogue.</param>
    /// <returns>The text content of the dialogue response.</returns>
    public string respodToDialogue(string dialogue, out string[] options, out bool endDialogue)
    {
        try
        {
            if (!this.dialogues.ContainsKey(dialogue))
            {
                Debug.Log("dialogue not found");
                options = null;
                endDialogue = true;
                return null;
            }
            else
            {
                Debug.Log("dialogue found");
                options = this.dialogues[dialogue].GetOptions();
                endDialogue = false;
            }
            return this.dialogues[dialogue].GetText();
        }
        catch (System.Exception)
        {
            options = null;
            endDialogue = true;
            return null;
        }
    }
}
