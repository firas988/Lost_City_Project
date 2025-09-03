using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player level progression, experience points, and skill tree integration.
/// Handles XP accumulation, level-ups, and provides access to different skill categories.
/// Coordinates with the skill tree system to grant skill points on level-up events.
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Audio Management")]
    /// <summary>
    /// Reference to the AudioManager for playing sounds.
    /// Provides audio feedback for level-up events and interactions.
    /// </summary>
    [SerializeField]
    private AudioManager audioManager;

    [Header("UI Elements")]
    /// <summary>
    /// UI image for level up progress bar.
    /// Shows visual progress toward the next level.
    /// </summary>
    [SerializeField]
    private Image levelUpFiller;

    /// <summary>
    /// UI text for displaying the current level.
    /// Shows the player's current level in the interface.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI levelText;
    #endregion

    #region Private Fields
    [Header("System References")]
    /// <summary>
    /// AudioSource for playing level up sounds.
    /// Handles local audio playback for level-up events.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Reference to the player GameObject.
    /// Used to access player components for level updates.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Reference to the notifications manager for displaying level-up messages.
    /// Shows feedback when the player levels up.
    /// </summary>
    private NotificationsManager notificationsManager;

    /// <summary>
    /// Reference to the skill tree manager for skill point allocation.
    /// Coordinates skill point grants on level-up events.
    /// </summary>
    private SkillTreeManager skillTree;

    /// <summary>
    /// Reference to the quest manager for quest-related functionality.
    /// Coordinates level progression with quest systems.
    /// </summary>
    private QuestManager questManager;

    [Header("Level Progression")]
    /// <summary>
    /// Current experience points accumulated by the player.
    /// Tracks progress toward the next level.
    /// </summary>
    private float currentXP;

    /// <summary>
    /// Experience points required to reach the next level.
    /// Increases with each level to maintain progression challenge.
    /// </summary>
    private float XPtoNextLevel;

    /// <summary>
    /// Current player level.
    /// Represents the player's progression through the game.
    /// </summary>
    private int level;

    [Header("Skill Lists (Unused)")]
    /// <summary>
    /// Skill lists for different categories (not used directly in this script).
    /// Maintained for potential future skill system integration.
    /// </summary>
    private SkillList strengthSkillList;
    private SkillList speedSkillList;
    private SkillList defenseSkillList;
    private SkillList healthSkillList;
    private SkillList levelList;
    #endregion

    #region Events
    /// <summary>
    /// Event triggered when the player levels up, providing the new level.
    /// Subscribed to by the skill tree system for skill point allocation.
    /// </summary>
    public event Action<int> onLevelUp;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the level manager, sets up initial values, and subscribes to quest completion events.
    /// Finds and stores references to required system components.
    /// </summary>
    void Awake()
    {
        // Find and store references to system managers
        skillTree = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<SkillTreeManager>();

        questManager = GameObject.FindAnyObjectByType<QuestManager>();
        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();

        // Initialize level progression values
        currentXP = 0;
        XPtoNextLevel = 1000;
        level = 0;

        // Find remaining system references
        notificationsManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<NotificationsManager>();

        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Handles input for saving and loading level data using keyboard shortcuts.
    /// B: Save, G: Load
    /// </summary>
    void Update() { }
    #endregion

    #region XP and Level Logic Methods
    /// <summary>
    /// Adds experience points to the player and handles level progression.
    /// Automatically levels up the player when sufficient XP is accumulated.
    /// </summary>
    /// <param name="XpToAdd">The amount of experience points to add to the player.</param>
    public void addXP(float XpToAdd)
    {
        // Safety check to prevent null reference errors
        if (this == null)
            return;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Add XP and track level progression
        currentXP += XpToAdd;
        int oldLevel = player.GetComponent<StartPlayer>().getPlayer().getLevel();
        int levelsToAdd = 0;

        // Check for level-ups and calculate how many levels to add
        while (currentXP >= XPtoNextLevel)
        {
            levelUpFiller.fillAmount = 0;
            currentXP -= XPtoNextLevel;
            levelsToAdd++;
            XPtoNextLevel *= 1.10f; // Increase XP requirement by 10% per level
        }

        // Handle level-up events if any levels were gained
        if (levelsToAdd > 0)
        {
            // Trigger level-up event for skill point allocation
            onLevelUp?.Invoke(levelsToAdd);
            player.GetComponent<StartPlayer>().getPlayer().addLevel(levelsToAdd);

            // Show level-up notification
            notificationsManager.queueTopLeftNotification(
                "Level Up! You are now level "
                    + player.GetComponent<StartPlayer>().getPlayer().getLevel(),
                "levelup"
            );

            // Update UI and level tracking
            levelText.text = (player.GetComponent<StartPlayer>().getPlayer().getLevel()).ToString();
            level = player.GetComponent<StartPlayer>().getPlayer().getLevel();
            levelUpFiller.fillAmount = 0;
            StartCoroutine(transitionLevelUp(2f));
        }
        else
        {
            // Update progress bar even without level-up
            StartCoroutine(transitionLevelUp(2f));
        }
    }

    /// <summary>
    /// Smoothly transitions the level up progress bar fill amount.
    /// Provides visual feedback for XP progress toward the next level.
    /// </summary>
    /// <param name="transitionTime">Duration of the transition in seconds.</param>
    /// <returns>Coroutine for managing the smooth transition.</returns>
    public IEnumerator transitionLevelUp(float transitionTime = 0.01f)
    {
        float timeElapsed = 0;
        float startFillAmount = levelUpFiller.fillAmount;
        float endFillAmount = Mathf.Min(currentXP / XPtoNextLevel, 1f);

        // Smoothly interpolate the progress bar fill amount
        while (timeElapsed < transitionTime)
        {
            levelUpFiller.fillAmount = Mathf.Lerp(
                startFillAmount,
                endFillAmount,
                timeElapsed / transitionTime
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final fill amount is set correctly
        levelUpFiller.fillAmount = endFillAmount;
    }
    #endregion

    #region Getter and Setter Methods
    /// <summary>
    /// Gets the current player level.
    /// </summary>
    /// <returns>The current level of the player.</returns>
    public int getLevel() => level;

    /// <summary>
    /// Gets the current experience points accumulated by the player.
    /// </summary>
    /// <returns>The current XP value.</returns>
    public float getCurrentXP() => currentXP;

    /// <summary>
    /// Gets the experience points required to reach the next level.
    /// </summary>
    /// <returns>The XP required for the next level.</returns>
    public float getXPtoNextLevel() => XPtoNextLevel;

    /// <summary>
    /// Sets the player's level to a new value.
    /// Updates both internal tracking and UI display.
    /// </summary>
    /// <param name="newLevel">The new level to set for the player.</param>
    public void setLevel(int newLevel)
    {
        level = newLevel;
        levelText.text = (level).ToString();
    }

    /// <summary>
    /// Sets the current experience points to a new value.
    /// Updates both internal tracking and progress bar display.
    /// </summary>
    /// <param name="newCurrentXP">The new XP value to set.</param>
    public void setCurrentXP(float newCurrentXP)
    {
        currentXP = newCurrentXP;
        levelUpFiller.fillAmount = currentXP / XPtoNextLevel;
    }

    /// <summary>
    /// Sets the experience points required for the next level.
    /// Controls the difficulty of level progression.
    /// </summary>
    /// <param name="newXPtoNextLevel">The new XP requirement for the next level.</param>
    public void setXPtoNextLevel(float newXPtoNextLevel)
    {
        XPtoNextLevel = newXPtoNextLevel;
    }
    #endregion

    #region Save/Load System Methods
    /// <summary>
    /// Loads level data from saved game data and applies it to the current session.
    /// Restores player level, XP, and progression from previous game sessions.
    /// </summary>
    /// <param name="levelData">The level data to load from.</param>
    public void LoadLevel(LevelData levelData)
    {
        if (levelData != null)
        {
            // Restore level progression data
            level = levelData.Level;
            setLevel(levelData.Level);
            setXPtoNextLevel(levelData.XPtoNextLevel);
            setCurrentXP(levelData.CurrentXP);

            // Start coroutine to ensure player is available before setting level
            StartCoroutine(setLevelCoroutine(levelData.Level));
        }
    }

    /// <summary>
    /// Coroutine that waits for the player to be available before setting their level.
    /// Ensures proper synchronization between the level manager and player data.
    /// </summary>
    /// <param name="level">The level to set for the player.</param>
    /// <returns>Coroutine for managing the delayed level setting.</returns>
    private IEnumerator setLevelCoroutine(int level)
    {
        // Wait for player to be available and then set their level
        yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<StartPlayer>().getPlayer().setLevel(level);
    }
    #endregion
}
