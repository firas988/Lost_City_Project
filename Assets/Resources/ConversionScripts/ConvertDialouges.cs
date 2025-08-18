using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class DialogueListWrapper
{
    public List<Dialogue> dialogue;
}

public class ConvertDialouges : MonoBehaviour
{
    [SerializeField]
    private TextAsset jsonFile;

    public Dictionary<string, Dialogue> GetDialogueByNpcName(string npcName)
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file assigned.");
            return null;
        }

        // Deserialize top-level dictionary: npcName => dialogue tree
        var allNpcData = JsonConvert.DeserializeObject<
            Dictionary<string, Dictionary<string, DialogueListWrapper>>
        >(jsonFile.text);

        if (!allNpcData.TryGetValue(npcName, out var npcDialogues))
        {
            Debug.LogWarning($"NPC '{npcName}' not found in JSON.");
            return null;
        }

        var result = new Dictionary<string, Dialogue>();

        foreach (var entry in npcDialogues)
        {
            if (entry.Value.dialogue != null && entry.Value.dialogue.Count > 0)
            {
                result[entry.Key] = entry.Value.dialogue[0]; // Take the first dialogue per node
            }
        }

        return result;
    }
}
