using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the overall skill tree system, handling skill upgrades, player bonuses, and skill point allocation.
/// Coordinates between different skill categories and the player's level progression system.
/// Provides centralized management for strength, speed, defense, and health skill trees.
/// </summary>
public class SkillTreeManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Player References")]
    /// <summary>
    /// Reference to the player GameObject for applying skill bonuses.
    /// Used to access player components for skill effect application.
    /// </summary>
    [SerializeField]
    private GameObject playerObject;

    /// <summary>
    /// Reference to the StartPlayer component for player data access.
    /// Provides access to player stats and skill bonus application methods.
    /// </summary>
    [SerializeField]
    private StartPlayer startPlayer;

    [Header("System Managers")]
    /// <summary>
    /// Reference to the level manager for level-up event handling and skill point allocation.
    /// Subscribes to level-up events to grant additional skill points.
    /// </summary>
    [SerializeField]
    private LevelManager levelSystem;

    /// <summary>
    /// Manages skill point limits and spending for skill upgrades.
    /// Controls the total available and spent skill points for the player.
    /// </summary>
    [SerializeField]
    private SkillAmountLimit skillAmountLimit;

    [Header("Strength Skill Tree")]
    /// <summary>
    /// Skill list for strength-based skills and upgrades.
    /// Manages strength skill progression and bonus calculations.
    /// </summary>
    [SerializeField]
    private SkillList strengthSkillList;

    /// <summary>
    /// List of UI buttons for the strength skill tree.
    /// Provides interactive elements for strength skill purchases.
    /// </summary>
    [SerializeField]
    private List<Button> strengthSkillButtons;

    [Header("Speed Skill Tree")]
    /// <summary>
    /// Skill list for speed-based skills and upgrades.
    /// Manages speed skill progression and bonus calculations.
    /// </summary>
    [SerializeField]
    private SkillList speedSkillList;

    /// <summary>
    /// List of UI buttons for the speed skill tree.
    /// Provides interactive elements for speed skill purchases.
    /// </summary>
    [SerializeField]
    private List<Button> speedSkillButtons;

    [Header("Defense Skill Tree")]
    /// <summary>
    /// Skill list for defense-based skills and upgrades.
    /// Manages defense skill progression and bonus calculations.
    /// </summary>
    [SerializeField]
    private SkillList defenseSkillList;

    /// <summary>
    /// List of UI buttons for the defense skill tree.
    /// Provides interactive elements for defense skill purchases.
    /// </summary>
    [SerializeField]
    private List<Button> defenseSkillButtons;

    [Header("Health Skill Tree")]
    /// <summary>
    /// Skill list for health-based skills and upgrades.
    /// Manages health skill progression and bonus calculations.
    /// </summary>
    [SerializeField]
    private SkillList healthSkillList;

    /// <summary>
    /// List of UI buttons for the health skill tree.
    /// Provides interactive elements for health skill purchases.
    /// </summary>
    [SerializeField]
    private List<Button> healthSkillButtons;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// Stores the previous level for level-up detection.
    /// Used to track level changes and trigger skill point allocation.
    /// </summary>
    private int prevLevel;

    /// <summary>
    /// Audio source component for playing skill-related sound effects.
    /// Provides audio feedback for skill upgrades and interactions.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Reference to the audio manager for playing skill-related sounds.
    /// Coordinates audio playback for skill tree interactions.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the notifications manager for displaying skill-related messages.
    /// Shows feedback when skills are upgraded or when errors occur.
    /// </summary>
    private NotificationsManager notificationsManager;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the skill tree manager, sets up skill point limits, and initializes strength skill list.
    /// Finds and stores references to required system components and sets up event subscriptions.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(1)
    void Awake()
    {
        // Find and store references to system managers
        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();

        notificationsManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<NotificationsManager>();

        // Get local components
        audioSource = GetComponent<AudioSource>();
        skillAmountLimit = GetComponentInChildren<SkillAmountLimit>();

        // Find level system and player references
        levelSystem = GameObject.FindWithTag("GameManager").GetComponentInChildren<LevelManager>();

        playerObject = GameObject.FindWithTag("Player");
        startPlayer = playerObject.GetComponent<StartPlayer>();

        // Initialize the skill tree system
        Init(levelSystem);
    }

    /// <summary>
    /// Handles input for skill upgrades using keyboard shortcuts.
    /// I: Strength, O: Speed, P: Defense, H: Health
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Ensure startPlayer reference is valid
        if (startPlayer == null)
        {
            startPlayer = playerObject.GetComponent<StartPlayer>();
        }
    }
    #endregion

    #region Initialization & Event Handling Methods
    /// <summary>
    /// Initializes the skill tree manager by subscribing to level-up events.
    /// Sets up event handling for automatic skill point allocation on level-up.
    /// </summary>
    /// <param name="levelSystem">The level manager to subscribe to for level-up events.</param>
    // COMPLEXITY ANALYSIS: Init() - O(1)
    public void Init(LevelManager levelSystem)
    {
        // Subscribe to the level-up event for automatic skill point allocation
        levelSystem.onLevelUp += HandleLevelUp;
    }
    #endregion

    #region Skill Upgrade Logic Methods
    /// <summary>
    /// Upgrades the specified skill list and updates the skill point limit.
    /// Applies skill bonuses to the player and manages skill point spending.
    /// </summary>
    /// <param name="skillList">The skill list to upgrade.</param>
    /// <returns>True if the upgrade was successful, false if conditions are not met.</returns>
    // COMPLEXITY ANALYSIS: UpgradeSkill() - O(1)
    public bool UpgradeSkill(SkillList skillList)
    {
        // Check if skill can be upgraded (not at max level and enough skill points)
        if (skillList.isMaxLevel() || !skillAmountLimit.CanSpend(skillList.currentCost))
        {
            return false;
        }

        // Apply skill bonus based on skill type
        if (skillList.getSkillType() == SkillType.Strength)
        {
            startPlayer.getPlayer().addStrengthBonusSkill(skillList.currentBonus);
        }
        else if (skillList.getSkillType() == SkillType.Health)
        {
            startPlayer.getPlayer().addHealthBonus(skillList.currentBonus);
        }
        else if (skillList.getSkillType() == SkillType.Speed)
        {
            startPlayer.getPlayer().addSpeedBonus(skillList.currentBonus);
        }
        else if (skillList.getSkillType() == SkillType.Defense)
        {
            startPlayer.getPlayer().addDefenseBonusSkill(skillList.currentBonus);
        }

        // Perform the skill upgrade and return success
        skillList.Upgrade();
        return true;
    }

    /// <summary>
    /// Handles level-up events by allocating additional skill points every 10 levels.
    /// Automatically grants skill points when the player levels up.
    /// </summary>
    /// <param name="newLevel">The new level the player has reached.</param>
    // COMPLEXITY ANALYSIS: HandleLevelUp() - O(1)
    private void HandleLevelUp(int newLevel)
    {
        // Grant skill points based on the new level (2 points per level)
        skillAmountLimit.AddTotalAvailable(newLevel * 2);
    }
    #endregion

    #region Getter Methods
    /// <summary>
    /// Gets the current player level from the level system.
    /// </summary>
    /// <returns>The current level of the player.</returns>
    // COMPLEXITY ANALYSIS: getLevel() - O(1)
    public int getLevel()
    {
        return levelSystem.getLevel();
    }

    /// <summary>
    /// Gets the current strength skill level.
    /// </summary>
    /// <returns>The current level of the strength skill tree.</returns>
    // COMPLEXITY ANALYSIS: getStrengthLevel() - O(1)
    public int getStrengthLevel()
    {
        return strengthSkillList.getCurrentLevel();
    }

    /// <summary>
    /// Gets the current speed skill level.
    /// </summary>
    /// <returns>The current level of the speed skill tree.</returns>
    // COMPLEXITY ANALYSIS: getSpeedLevel() - O(1)
    public int getSpeedLevel()
    {
        return speedSkillList.getCurrentLevel();
    }

    /// <summary>
    /// Gets the current defense skill level.
    /// </summary>
    /// <returns>The current level of the defense skill tree.</returns>
    // COMPLEXITY ANALYSIS: getDefenseLevel() - O(1)
    public int getDefenseLevel()
    {
        return defenseSkillList.getCurrentLevel();
    }

    /// <summary>
    /// Gets the current health skill level.
    /// </summary>
    /// <returns>The current level of the health skill tree.</returns>
    // COMPLEXITY ANALYSIS: getHealthLevel() - O(1)
    public int getHealthLevel()
    {
        return healthSkillList.getCurrentLevel();
    }

    /// <summary>
    /// Gets the SkillAmountLimit instance for skill point management.
    /// </summary>
    /// <returns>Reference to the skill amount limit manager.</returns>
    // COMPLEXITY ANALYSIS: getSkillAmountLimit() - O(1)
    public SkillAmountLimit getSkillAmountLimit()
    {
        return skillAmountLimit;
    }
    #endregion

    #region Save/Load Methods
    /// <summary>
    /// Loads skill data from saved game data and applies it to the current session.
    /// Restores skill levels and skill point allocation from previous game sessions.
    /// </summary>
    /// <param name="skillTreeData">The skill tree data to load from.</param>
    // COMPLEXITY ANALYSIS: LoadSkills() - O(1)
    public void LoadSkills(SkillTreeData skillTreeData)
    {
        // Reset spent skill points and set total available from save data
        skillAmountLimit.setSpent(0);
        skillAmountLimit.setTotalAvailable(
            skillTreeData != null ? skillTreeData.TotalSkillPoints : 0
        );
        skillAmountLimit.Render();

        // Initialize each skill tree with saved levels
        strengthSkillList.Init(
            skillTreeData != null ? skillTreeData.StrengthLevel : 0,
            strengthSkillList.getMaxLevel(),
            strengthSkillButtons,
            skillAmountLimit
        );
        healthSkillList.Init(
            skillTreeData != null ? skillTreeData.HealthLevel : 0,
            healthSkillList.getMaxLevel(),
            healthSkillButtons,
            skillAmountLimit
        );
        speedSkillList.Init(
            skillTreeData != null ? skillTreeData.SpeedLevel : 0,
            speedSkillList.getMaxLevel(),
            speedSkillButtons,
            skillAmountLimit
        );
        defenseSkillList.Init(
            skillTreeData != null ? skillTreeData.DefenseLevel : 0,
            defenseSkillList.getMaxLevel(),
            defenseSkillButtons,
            skillAmountLimit
        );
    }
    #endregion
}
