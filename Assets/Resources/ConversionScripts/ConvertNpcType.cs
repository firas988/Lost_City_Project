using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class NpcType
{
    public string name { get; set; }
    public float walkRadius { get; set; }
    public int areaMask { get; set; }
    public float[] waitTimeRange { get; set; }
    public string navMeshAreaName { get; set; }
    public float health { get; set; }
    public float speed { get; set; }
    public float maxSpeed { get; set; }
}

public class ConvertNpcType : MonoBehaviour
{
    [SerializeField]
    private TextAsset jsonFile;

    private List<Dictionary<string, List<NpcType>>> npcTypes;

    public List<NpcType> GetNpcTypes(string npcType)
    {
        if (npcTypes == null)
            return null;

        foreach (var npc in npcTypes)
        {
            foreach (var type in npc)
            {
                if (type.Key == npcType)
                {
                    return type.Value;
                }
            }
        }
        return null;
    }

    void Awake()
    {
        npcTypes = JsonConvert.DeserializeObject<List<Dictionary<string, List<NpcType>>>>(
            jsonFile.text
        );
    }
}
