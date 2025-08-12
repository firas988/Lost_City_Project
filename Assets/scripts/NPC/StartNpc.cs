using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes an NPC in the scene based on data retrieved from a JSON converter.
/// Stores an instance of the NPC with relevant configuration data for later use.
/// </summary>
public class StartNpc : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Prefab reference for the NPC (not instantiated in this script).
    /// </summary>
    [SerializeField]
    private GameObject npcPrefab;

    /// <summary>
    /// Reference to kill quests configuration.
    /// </summary>
    [SerializeField]
    private ExpKillQuests killQuests;

    /// <summary>
    /// Reference to find quests configuration.
    /// </summary>
    [SerializeField]
    private ExpFindQuests findQuests;

    /// <summary>
    /// Reference to story quest configuration.
    /// </summary>
    [SerializeField]
    private StoryQuest storyQuest;

    #endregion

    #region Private Fields

    /// <summary>
    /// Index for tracking quest assignments.
    /// </summary>
    private int questIndex = 0;

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

    /// <summary>
    /// The layer name of the NPC GameObject.
    /// </summary>
    private string layerName;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Unity's Awake method, called once when the script instance is being loaded.
    /// Initializes the NPC data from the converter and creates the NPC instance.
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
    }

    /// <summary>
    /// Unity's Update method, called every frame.
    /// Ensures NPC instance is properly initialized.
    /// </summary>
    private void Update()
    {
        if (npcsInstance == null)
        {
            Awake();
            return;
        }
    }

    #endregion

    #region NPC Creation Methods

    /// <summary>
    /// Creates an NPC instance based on the layer type and configuration data.
    /// </summary>
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

                    Quest quest = null;

                    if (npcName == "Robert" || npcName == "MysteriousMan")
                    {
                        quest = storyQuest;
                    }

                    if (npcName == "ConfusedPerson")
                    {
                        quest = new FindQuest(findQuests.RandomQuest);
                    }
                    else if (npcName == "KillPerson")
                    {
                        quest = new KillQuest(killQuests.RandomQuest);
                    }

                    npcsInstance = createQuestGiver(typeInstance.name, dialogueData, quest);
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
    /// Creates a QuestGiver NPC instance with dialogue and quest data.
    /// </summary>
    /// <param name="npcName">The name of the NPC.</param>
    /// <param name="dialogueData">The dialogue data for the NPC.</param>
    /// <param name="quest">The quest to be given by this NPC.</param>
    /// <returns>A new QuestGiver instance.</returns>
    private QuestGiver createQuestGiver(
        string npcName,
        Dictionary<string, Dialogue> dialogueData,
        Quest quest
    )
    {
        return new QuestGiver(
            gameObject.GetInstanceID(),
            npcName,
            layerName,
            typeInstance.walkRadius,
            typeInstance.areaMask,
            new Vector2(typeInstance.waitTimeRange[0], typeInstance.waitTimeRange[1]),
            "",
            typeInstance.speed,
            typeInstance.maxSpeed,
            "start",
            dialogueData,
            quest,
            this.gameObject
        );
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the initialized NPC instance.
    /// </summary>
    /// <returns>The NPC instance with configuration and identity data.</returns>
    public NPC GetNpcsInstance()
    {
        return npcsInstance;
    }

    #endregion


    public void refreshQuestGiver()
    {
        if (npcName == "ConfusedPerson")
        {
            (npcsInstance as QuestGiver).setQuestToGive(findQuests.RandomQuest, this.gameObject);
        }
        else if (npcName == "KillPerson")
        {
            (npcsInstance as QuestGiver).setQuestToGive(killQuests.RandomQuest, this.gameObject);
        }
    }
}
