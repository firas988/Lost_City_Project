using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player level progression, experience points, and skill tree integration.
/// Handles XP accumulation, level-ups, and provides access to different skill categories.
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    private AudioManager audioManager; // Reference to the AudioManager for playing sounds

    [SerializeField]
    private Image levelUpFiller; // UI image for level up progress bar

    [SerializeField]
    private TextMeshProUGUI levelText; // UI text for displaying the current level
    #endregion

    #region Private Fields

    private AudioSource audioSource; // AudioSource for playing level up sounds
    private GameObject player; // Reference to the player GameObject
    private NotificationsManager notificationsManager; // Reference to the notifications manager
    private SkillTreeManager skillTree; // Reference to the skill tree manager
    private QuestManager questManager; // Reference to the quest manager

    private float currentXP; // Current experience points accumulated by the player
    private float XPtoNextLevel; // Experience points required to reach the next level
    private int level; // Current player level

    // Skill lists for different categories (not used directly in this script)
    private SkillList strengthSkillList;
    private SkillList speedSkillList;
    private SkillList defenseSkillList;
    private SkillList healthSkillList;
    private SkillList levelList;

    #endregion

    #region Events

    /// <summary>
    /// Event triggered when the player levels up, providing the new level.
    /// </summary>
    public event Action<int> onLevelUp;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the level manager, sets up initial values, and subscribes to quest completion events.
    /// </summary>
    void Awake()
    {
        skillTree = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<SkillTreeManager>();

        questManager = GameObject.FindAnyObjectByType<QuestManager>();
        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();
        currentXP = 0;
        XPtoNextLevel = 1000;
        level = 0;
        questManager.onQuestFinish += addXP;
        notificationsManager = GameObject
            .FindWithTag("GameManager")
            .GetComponentInChildren<NotificationsManager>();

        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(level);
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Handles input for saving and loading level data using keyboard shortcuts.
    /// B: Save, G: Load
    /// </summary>
    void Update() { }

    #endregion

    #region XP and Level Logic

    /// <summary>
    /// Adds experience points to the player and handles level progression.
    /// Automatically levels up the player when sufficient XP is accumulated.
    /// </summary>
    /// <param name="XpToAdd">The amount of experience points to add to the player.</param>
    public void addXP(float XpToAdd)
    {
        currentXP += XpToAdd;
        int oldLevel = player.GetComponent<StartPlayer>().getPlayer().getLevel();
        int levelsToAdd = 0;
        Debug.Log(currentXP);
        while (currentXP >= XPtoNextLevel)
        {
            levelUpFiller.fillAmount = 0;
            StartCoroutine(transitionLevelUp(2f));
            currentXP -= XPtoNextLevel;
            levelsToAdd++;
            XPtoNextLevel *= 1.10f;
        }
        if (levelsToAdd == 0)
            return;
        onLevelUp?.Invoke(levelsToAdd);
        player.GetComponent<StartPlayer>().getPlayer().addLevel(levelsToAdd);
        notificationsManager.queueTopLeftNotification(
            "Level Up! You are now level "
                + player.GetComponent<StartPlayer>().getPlayer().getLevel(),
            "levelup"
        );
        levelText.text = (player.GetComponent<StartPlayer>().getPlayer().getLevel()).ToString();
        level = player.GetComponent<StartPlayer>().getPlayer().getLevel();
        levelUpFiller.fillAmount = 0;
        StartCoroutine(transitionLevelUp(2f));
    }

    /// <summary>
    /// Smoothly transitions the level up progress bar fill amount.
    /// </summary>
    /// <param name="transitionTime">Duration of the transition in seconds.</param>
    public IEnumerator transitionLevelUp(float transitionTime = 0.01f)
    {
        float timeElapsed = 0;
        float startFillAmount = levelUpFiller.fillAmount;
        float endFillAmount = Mathf.Min(currentXP / XPtoNextLevel, 1f);
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
        levelUpFiller.fillAmount = endFillAmount;
    }

    #endregion

    #region Getters and Setters

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
    /// </summary>
    /// <param name="newLevel">The new level to set for the player.</param>
    public void setLevel(int newLevel)
    {
        level = newLevel;
        levelText.text = (level).ToString();
    }

    /// <summary>
    /// Sets the current experience points to a new value.
    /// </summary>
    /// <param name="newCurrentXP">The new XP value to set.</param>
    public void setCurrentXP(float newCurrentXP)
    {
        currentXP = newCurrentXP;
        levelUpFiller.fillAmount = currentXP / XPtoNextLevel;
    }

    /// <summary>
    /// Sets the experience points required for the next level.
    /// </summary>
    /// <param name="newXPtoNextLevel">The new XP requirement for the next level.</param>
    public void setXPtoNextLevel(float newXPtoNextLevel)
    {
        XPtoNextLevel = newXPtoNextLevel;
    }

    #endregion


    public void LoadLevel(LevelData levelData)
    {
        if (levelData != null)
        {
            level = levelData.Level;
            setLevel(levelData.Level);
            setXPtoNextLevel(levelData.XPtoNextLevel);
            setCurrentXP(levelData.CurrentXP);
            StartCoroutine(setLevelCoroutine(levelData.Level));
        }
    }

    private IEnumerator setLevelCoroutine(int level)
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<StartPlayer>().getPlayer().setLevel(level);
    }
}
