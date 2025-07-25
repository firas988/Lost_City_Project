using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the overall skill tree system, handling skill upgrades, player bonuses, and skill point allocation.
/// Coordinates between different skill categories and the player's level progression system.
/// </summary>
public class SkillTreeManager : MonoBehaviour
{
    #region Inspector Fields

    /// <summary>
    /// Reference to the player GameObject for applying skill bonuses.
    /// </summary>
    [SerializeField]
    private GameObject playerObject;

    /// <summary>
    /// Reference to the StartPlayer component for player data access.
    /// </summary>
    [SerializeField]
    private StartPlayer startPlayer;

    /// <summary>
    /// Reference to the level manager for level-up event handling and skill point allocation.
    /// </summary>
    [SerializeField]
    private LevelManager levelSystem;

    /// <summary>
    /// Manages skill point limits and spending for skill upgrades.
    /// </summary>
    [SerializeField]
    private SkillAmountLimit skillAmountLimit;

    /// <summary>
    /// Skill list for strength-based skills and upgrades.
    /// </summary>
    [SerializeField]
    private SkillList strengthSkillList;

    [SerializeField]
    private List<Button> strengthSkillButtons;

    /// <summary>
    /// Skill list for speed-based skills and upgrades.
    /// </summary>
    [SerializeField]
    private SkillList speedSkillList;

    /// <summary>
    /// Skill list for defense-based skills and upgrades.
    /// </summary>
    [SerializeField]
    private SkillList defenseSkillList;

    /// <summary>
    /// Skill list for health-based skills and upgrades.
    /// </summary>
    [SerializeField]
    private SkillList healthSkillList;

    #endregion

    #region Private Fields

    /// <summary>
    /// Stores the previous level for level-up detection.
    /// </summary>
    private int prevLevel;

    private AudioSource audioSource;

    private AudioManager audioManager;

    private NotificationsManager notificationsManager;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the skill tree manager, sets up skill point limits, and initializes strength skill list.
    /// </summary>
    void Awake()
    {
            audioManager = GameObject.Find("GameManger").GetComponent<AudioManager>();
        notificationsManager = GameObject.Find("GameManger").GetComponent<NotificationsManager>();
        audioSource = GetComponent<AudioSource>();
     
    
    }

    /// <summary>
    /// Handles input for skill upgrades using keyboard shortcuts.
    /// I: Strength, O: Speed, P: Defense, H: Health
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpgradeSkill(strengthSkillList);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            UpgradeSkill(speedSkillList);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpgradeSkill(defenseSkillList);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            UpgradeSkill(healthSkillList);
        }

        // Uncomment to enable skill tree save/load debug controls
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Saving skill tree");
            SaveSystem.SaveSkills(this);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
              SkillTreeData skillTreeData = SaveSystem.LoadSkills();
        if (skillTreeData != null)
        {
            skillAmountLimit.setTotalAvailable(skillTreeData.totalSkillPoints);
            skillAmountLimit.setSpent(skillTreeData.spent);
            skillAmountLimit.Render();
        }
     
        startPlayer = playerObject.GetComponent<StartPlayer>();
        Init(GetComponent<LevelManager>());
        strengthSkillList.Init(
            skillTreeData.strengthLevel,
            strengthSkillList.getMaxLevel(),
            strengthSkillButtons,
            skillAmountLimit
        );
        }
    }

    #endregion

    #region Initialization & Event Handling

    /// <summary>
    /// Initializes the skill tree manager by subscribing to level-up events.
    /// </summary>
    /// <param name="levelSystem">The level manager to subscribe to for level-up events.</param>
    public void Init(LevelManager levelSystem)
    {
        // Subscribe to the event
        levelSystem.onLevelUp += HandleLevelUp;
    }

    #endregion

    #region Skill Upgrade Logic

    /// <summary>
    /// Upgrades the specified skill list and updates the skill point limit.
    /// </summary>
    /// <param name="skillList">The skill list to upgrade.</param>
    public bool UpgradeSkill(SkillList skillList)
    {
        try
        {
            startPlayer.getPlayer().addStrengthBonusSkill(skillList.currentBonus);
            skillList.Upgrade();
            notificationsManager.queueTopLeftNotification("Skill Upgraded");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error upgrading skill: " + e.Message);
            audioManager.queueUI(audioSource, "Error");
            notificationsManager.queueTopLeftNotification("Skill Maxed Out or Not Enough Skill Points");
        }
        return false;
    }

    /// <summary>
    /// Handles level-up events by allocating additional skill points every 10 levels.
    /// </summary>
    /// <param name="newLevel">The new level the player has reached.</param>
    private void HandleLevelUp(int newLevel)
    {
        if (newLevel % 10 == 0)
            skillAmountLimit.AddTotalAvailable(2);
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the current player level from the level system.
    /// </summary>
    public int getLevel()
    {
        return levelSystem.getLevel();
    }

    /// <summary>
    /// Gets the current strength skill level.
    /// </summary>
    public int getStrengthLevel()
    {
        return strengthSkillList.getCurrentLevel();
    }

    // Uncomment and implement if needed for other skill types
    // public int getSpeedLevel()
    // {
    //     return speedSkillList.getCurrentLevel();
    // }
    // public int getDefenseLevel()
    // {
    //     return defenseSkillList.getCurrentLevel();
    // }
    // public int getHealthLevel()
    // {
    //     return healthSkillList.getCurrentLevel();
    // }

    /// <summary>
    /// Gets the SkillAmountLimit instance for skill point management.
    /// </summary>
    public SkillAmountLimit getSkillAmountLimit()
    {
        return skillAmountLimit;
    }

    #endregion

    #region Save/Load

    public void SaveSkills()
    {
        SaveSystem.SaveSkills(this);
    }

    public void LoadSkills()
    {
        SkillTreeData skillTreeData = SaveSystem.LoadSkills();
    }

    #endregion
}
