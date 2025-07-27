using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Initializes an NPC in the scene based on data retrieved from a JSON converter.
/// Stores an instance of the NPC with relevant configuration data for later use.
/// </summary>
public class StartNpc : MonoBehaviour
{
    /// <summary>
    /// Prefab reference for the NPC (not instantiated in this script).
    /// </summary>
    [SerializeField]
    private GameObject npcPrefab;

    [SerializeField]
    private ExpKillQuests killQuests;

    [SerializeField]
    private ExpFindQuests findQuests;

    /// <summary>
    /// The tag/name of the NPC GameObject, used to look up type data.
    /// </summary>
    private string npcName;

    /// <summary>
    /// The generated NPC instance with initialized data.
    /// </summary>
    private NPC npcsInstance;

    /// <summary>
    /// The type data of the NPC, retrieved from the converter based on its name.
    /// </summary>
    private NpcType typeInstance;

    private string layerName;

    /// <summary>
    /// Unity's Start method, called once before the first frame update.
    /// Initializes the NPC data from the converter and logs it.
    /// </summary>
    private void Awake()
    {
        npcName = gameObject.tag;
        layerName = LayerMask.LayerToName(gameObject.layer);

        // Find the JSON converter GameObject and retrieve its script
        ConvertNpcType script = FindAnyObjectByType<ConvertNpcType>();
        List<NpcType> npcTypes = script.GetNpcTypes(layerName);

        if (npcTypes == null)
            return;

        // Retrieve the matching NPC type data using the name/tag
        typeInstance = npcTypes.Find(t => t.name == npcName);
        // Initialize an NPC instance with the retrieved data

        if (typeInstance != null)
        {
            createNpcInstance();
        }
        // Output the NPC instance information to the console
    }

    private void Update()
    {
        if (npcsInstance == null)
        {
            Awake();
            return;
        }
    }

    private void createNpcInstance()
    {
        switch (layerName)
        {
            case "Pedestrian":
                npcsInstance = new NPC(
                    gameObject.GetInstanceID(),
                    typeInstance.name,
                    layerName,
                    typeInstance.walkRadius,
                    typeInstance.areaMask,
                    new Vector2(typeInstance.waitTimeRange[0], typeInstance.waitTimeRange[1]),
                    "",
                    typeInstance.speed,
                    typeInstance.maxSpeed
                );
                break;
            case "Enemy":
                npcsInstance = new Entity(
                    gameObject.GetInstanceID(),
                    typeInstance.name,
                    layerName,
                    typeInstance.walkRadius,
                    typeInstance.areaMask,
                    new Vector2(typeInstance.waitTimeRange[0], typeInstance.waitTimeRange[1]),
                    "",
                    typeInstance.health,
                    typeInstance.speed,
                    typeInstance.maxSpeed
                );
                break;
            case "QuestGiver":
                {
                    ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        typeInstance.name
                    );

                    npcsInstance = new QuestGiver(
                        gameObject.GetInstanceID(),
                        typeInstance.name,
                        layerName,
                        typeInstance.walkRadius,
                        typeInstance.areaMask,
                        new Vector2(typeInstance.waitTimeRange[0], typeInstance.waitTimeRange[1]),
                        "",
                        typeInstance.speed,
                        typeInstance.maxSpeed,
                        "start",
                        dialogueData,
                        npcName == "ConfusedPerson"
                            ? new FindQuest(findQuests.RandomQuest)
                            : new KillQuest(killQuests.RandomQuest)
                    );

                    Debug.Log(npcsInstance);
                }
                break;
            case "TalkativePerson":
                {
                    ConvertDialouges dialogueConv = FindAnyObjectByType<ConvertDialouges>();
                    Dictionary<string, Dialogue> dialogueData = dialogueConv.GetDialogueByNpcName(
                        typeInstance.name
                    );
                    npcsInstance = new TalkativeNpc(
                        gameObject.GetInstanceID(),
                        typeInstance.name,
                        layerName,
                        typeInstance.walkRadius,
                        typeInstance.areaMask,
                        new Vector2(typeInstance.waitTimeRange[0], typeInstance.waitTimeRange[1]),
                        "",
                        typeInstance.speed,
                        typeInstance.maxSpeed,
                        dialogueData,
                        "start"
                    );
                }

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns the initialized NPC instance.
    /// </summary>
    /// <returns>The NPC instance with configuration and identity data.</returns>
    public NPC GetNpcsInstance()
    {
        return npcsInstance;
    }
}
