using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;

/// <summary>
/// Represents a player character in the game with health, combat stats, and quest management capabilities.
/// The Player class handles damage calculation, stat bonuses, and quest tracking.
/// </summary>
public class Player
{
    /// <summary>
    /// Level of the player
    /// </summary>
    private int level;

    /// <summary>
    /// Maximum health points the player can have
    /// </summary>
    private float maxHealth;

    /// <summary>
    /// Current health points of the player
    /// </summary>
    private float currentHealth;

    /// <summary>
    /// Current strength stat affecting damage output
    /// </summary>
    private float currentStrength;

    /// <summary>
    /// Current speed stat affecting movement
    /// </summary>
    private float currentSpeed;

    private WeaponItem weapon;

    /// <summary>
    /// Current defense stat affecting damage reduction
    /// </summary>
    private float currentDefense;

    /// <summary>
    /// Maximum speed stat the player can achieve
    /// </summary>
    private float maxSpeed;

    /// <summary>
    /// Maximum strength stat the player can achieve
    /// </summary>
    private float maxStrength;

    /// <summary>
    /// Maximum defense stat the player can achieve
    /// </summary>
    private float maxDefense;

    /// <summary>
    /// Current defense buff applied to the player
    /// </summary>
    private float currentDefensePotionBuff;

    /// <summary>
    /// Current strength buff applied to the player
    /// </summary>
    private float currentStrengthPotionBuff;

    /// <summary>
    /// Health bonus multiplier applied to base health
    /// </summary>
    private float currentHealthBonus;

    /// <summary>
    /// Speed bonus added to base speed stat
    /// </summary>
    private float currentSpeedBonus;

    /// <summary>
    /// Strength bonus added to base strength stat
    /// </summary>
    private float currentStrengthArmorBonus;

    /// <summary>
    /// Strength bonus added to base strength stat from skill tree
    /// </summary>
    private float currentStrengthBonusSkill;

    /// <summary>
    /// Strength bonus added to base strength stat from skill tree
    /// </summary>
    private float currentDefenseBonusSkill;

    /// <summary>
    /// Defense bonus added to base defense stat
    /// </summary>
    private float currentDefenseArmorBonus;

    /// <summary>
    /// Inventory of the player
    /// </summary>
    private Inventory inventory;

    /// <summary>
    /// List of all active quests the player has accepted
    /// </summary>
    private List<Quest> activeSideQuests;

    /// <summary>
    /// List of all active quests the player has accepted
    /// </summary>
    private StoryQuest currentMainQuest;

    /// <summary>
    /// Initializes a new Player instance with default stats and empty quest list.
    /// Sets up base health, strength, speed, and defense values with initial bonuses.
    /// </summary>
    public Player()
    {
        // Initialize stat bonuses
        this.currentHealthBonus = 1f;
        this.currentSpeedBonus = 0f;
        this.currentStrengthArmorBonus = 0f;
        this.currentDefenseArmorBonus = 0f;

        //skill tree bonuses
        this.currentStrengthBonusSkill = 0f;
        this.currentDefenseBonusSkill = 0f;

        //potion buffs bonuses
        this.currentDefensePotionBuff = 0f;
        this.currentStrengthPotionBuff = 0f;

        // Set maximum stat values
        this.maxSpeed = 10f; // to remove
        this.currentSpeed = 0f;

        // Calculate health based on bonus multiplier
        this.maxHealth = 100f * this.currentHealthBonus;
        this.currentHealth = this.maxHealth;

        // Initialize current defense
        this.currentDefense = 1f;

        // Initialize current strength
        this.currentStrength = 1f;

        // Initialize quest tracking
        activeSideQuests = new List<Quest>();
        currentMainQuest = null;

        this.inventory = new Inventory();
    }

    /// <summary>
    /// Applies damage to the player, reduced by defense stat.
    /// Damage is calculated as: damage / maxDefense
    /// </summary>
    /// <param name="takenDmg">The raw damage amount to be applied</param>
    public int getDamage()
    {
        if (this.weapon != null)
        {
            return (int)(this.currentStrength * this.weapon.getDamage());
        }
        return 0;
    }

    /// <summary>
    /// Adds a strength bonus to the player's current strength stat.
    /// The final strength is capped at the maximum strength value.
    /// </summary>
    /// <param name="bonus">The strength bonus to add</param>
    public void addStrengthBonusSkill(float bonus)
    {
        this.currentStrengthBonusSkill += bonus;
        calculateStrength();
    }

    public void addDefenseBonusSkill(float bonus)
    {
        this.currentDefenseBonusSkill += bonus;
        calculateDefense();
    }

    public void addStrengthPotionBuff(float bonus)
    {
        this.currentStrengthPotionBuff += bonus;
        calculateStrength();
    }

    public void resetStrengthPotionBuff()
    {
        this.currentStrengthPotionBuff = 0f;
        calculateStrength();
    }

    public void addDefensePotionBuff(float bonus)
    {
        this.currentDefensePotionBuff += bonus;
        calculateDefense();
    }

    //////////////////////////////////////////////////////////////
    public void calculateStrengthAndDefenseBonus()
    {
        calculateStrengthBonusFromArmor();
        calculateDefenseBonusFromArmor();
    }

    //////////////////////////////////////////////////////////////

    public void calculateStrengthBonusFromArmor()
    {
        this.currentStrengthArmorBonus = inventory.getArmorSlots().getStrengthBonus();
        calculateStrength();
    }

    public void calculateDefenseBonusFromArmor()
    {
        this.currentDefenseArmorBonus = inventory.getArmorSlots().getDefenseBonus();
        calculateDefense();
    }

    public void calculateDefense()
    {
        this.currentDefense = 1f;
        this.currentDefense =
            this.currentDefense + this.currentDefensePotionBuff + this.currentDefenseArmorBonus;
    }

    public void calculateStrength()
    {
        this.currentStrength = 1f;
        this.currentStrength =
            this.currentStrength
            + this.currentStrengthPotionBuff
            + this.currentStrengthArmorBonus
            + this.currentStrengthBonusSkill;
    }

    /// <summary>
    /// Adds a speed bonus to the player's current speed stat.
    /// The final speed is capped at the maximum speed value.
    /// </summary>
    /// <param name="bonus">The speed bonus to add</param>
    public void addSpeedBonus(float bonus)
    {
        this.currentSpeed += bonus;
    }

    public void removeSpeedPotionBuff(float bonus)
    {
        this.currentSpeed = Mathf.Max(0, this.currentSpeed - bonus);
    }

    /// <summary>
    /// Adds a health bonus multiplier to increase the player's maximum health.
    /// Recalculates maxHealth as: 100 * healthBonus
    /// </summary>
    /// <param name="bonus">The health bonus multiplier to add</param>
    public void addHealthBonus(float bonus)
    {
        this.currentHealthBonus += bonus;
        this.maxHealth = 100f * this.currentHealthBonus;
    }

    public void addHealth(float health)
    {
        this.currentHealth = Mathf.Min(this.maxHealth, this.currentHealth + health);
    }

    public void resetHealth()
    {
        this.currentHealth = this.maxHealth;
    }

    /// <summary>
    /// Adds a quest to the player's active quest list if it's not already present.
    /// Prevents duplicate quests from the same quest giver.
    /// </summary>
    /// <param name="quest">The quest to add to the active quests</param>
    public bool addQuest(Quest quest)
    {
        if (activeSideQuests.Find(questToFind => questToFind.Giver == quest.Giver) == null)
        {
            this.activeSideQuests.Add(quest);
            return true;
        }
        return false;
    }

    public bool removeQuest(Quest quest)
    {
        if (activeSideQuests.Find(questToFind => questToFind.Giver == quest.Giver) != null)
        {
            this.activeSideQuests.Remove(quest);
            return true;
        }
        return false;
    }

    public Inventory getInventory()
    {
        return this.inventory;
    }

    public void takeDamage(float takenDmg)
    {
        this.currentHealth -= takenDmg / this.currentDefense;
        if (this.currentHealth <= 0)
        {
            this.currentHealth = 0;
        }
    }

    //public void addQuest(Quest quest)
    //{
    //    if (activeQuests.Find(questToFind => questToFind.GiverId == quest.GiverId) == null)
    //    {
    //        this.activeQuests.Add(quest);
    //        Debug.Log(this.activeQuests[0]);
    //    }
    //}

    //public List<Quest> ActiveQuest
    //{
    //    get { return this.activeQuests; }
    //}

    public void setWeapon()
    {
        if (inventory.getHotbar().getWeapon().Count > 0)
        {
            this.weapon = (WeaponItem)inventory.getHotbar().getWeapon()[0];
        }
    }

    public WeaponItem getWeapon()
    {
        return this.weapon;
    }

    public void removeWeapon()
    {
        this.weapon = null;
    }

    /// <summary>
    /// Gets the list of all active quests the player has accepted.
    /// </summary>
    /// <returns>A list containing all active quests</returns>
    public List<Quest> ActiveQuest
    {
        get { return this.activeSideQuests; }
    }

    public List<Quest> ActiveSideQuests
    {
        get { return this.activeSideQuests; }
        set { this.activeSideQuests = value; }
    }

    public StoryQuest getCurrentMainQuest()
    {
        return this.currentMainQuest;
    }

    public void setCurrentMainQuest(StoryQuest quest)
    {
        this.currentMainQuest = quest;
    }

    public bool isDead()
    {
        return this.currentHealth <= 0;
    }

    public float getHealth()
    {
        return this.currentHealth;
    }

    public float getMaxHealth()
    {
        return this.maxHealth;
    }

    public int getLevel()
    {
        return this.level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public void addLevel(int level)
    {
        this.level += level;
    }

    public float getCurrentStrengthBonusSkill()
    {
        return this.currentStrengthBonusSkill;
    }

    public float getCurrentSpeed()
    {
        return this.currentSpeed;
    }
}
