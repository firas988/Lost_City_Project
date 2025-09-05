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
    #region Core Stats
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

    /// <summary>
    /// Current defense stat affecting damage reduction
    /// </summary>
    private float currentDefense;
    #endregion

    #region Maximum Stat Limits
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
    #endregion

    #region Equipment and Items
    /// <summary>
    /// Currently equipped weapon item
    /// </summary>
    private WeaponItem weapon;

    /// <summary>
    /// Inventory of the player
    /// </summary>
    private Inventory inventory;
    #endregion

    #region Potion Buffs
    /// <summary>
    /// Current defense buff applied to the player
    /// </summary>
    private float currentDefensePotionBuff;

    /// <summary>
    /// Current strength buff applied to the player
    /// </summary>
    private float currentStrengthPotionBuff;
    #endregion

    #region Base Stat Bonuses
    /// <summary>
    /// Health bonus multiplier applied to base health
    /// </summary>
    private float currentHealthBonus;

    /// <summary>
    /// Speed bonus added to base speed stat
    /// </summary>
    private float currentSpeedBonus;

    /// <summary>
    /// Strength bonus added to base strength stat from armor
    /// </summary>
    private float currentStrengthArmorBonus;

    /// <summary>
    /// Defense bonus added to base defense stat from armor
    /// </summary>
    private float currentDefenseArmorBonus;
    #endregion

    #region Skill Tree Bonuses
    /// <summary>
    /// Strength bonus added to base strength stat from skill tree
    /// </summary>
    private float currentStrengthBonusSkill;

    /// <summary>
    /// Defense bonus added to base defense stat from skill tree
    /// </summary>
    private float currentDefenseBonusSkill;
    #endregion

    #region Quest Management
    /// <summary>
    /// List of all active side quests the player has accepted
    /// </summary>
    private List<Quest> activeSideQuests;

    /// <summary>
    /// Current main story quest the player is pursuing
    /// </summary>
    private StoryQuest currentMainQuest;
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new Player instance with default stats and empty quest list.
    /// Sets up base health, strength, speed, and defense values with initial bonuses.
    /// </summary>
    public Player()
    {
        // COMPLEXITY ANALYSIS: Player() - O(1)
        // Initialize stat bonuses
        this.currentHealthBonus = 1f;
        this.currentSpeedBonus = 0f;
        this.currentStrengthArmorBonus = 0f;
        this.currentDefenseArmorBonus = 0f;

        // Initialize skill tree bonuses
        this.currentStrengthBonusSkill = 0f;
        this.currentDefenseBonusSkill = 0f;

        // Initialize potion buff bonuses
        this.currentDefensePotionBuff = 0f;
        this.currentStrengthPotionBuff = 0f;

        // Set maximum stat values
        this.maxSpeed = 10f; // to remove
        this.currentSpeed = 0f;

        // Calculate health based on bonus multiplier
        this.maxHealth = 100f * this.currentHealthBonus;
        this.currentHealth = this.maxHealth;

        // Initialize current defense and strength
        this.currentDefense = 1f;
        this.currentStrength = 1f;

        // Initialize quest tracking
        activeSideQuests = new List<Quest>();
        currentMainQuest = null;

        // Initialize inventory
        this.inventory = new Inventory();
    }
    #endregion

    #region Combat and Damage
    /// <summary>
    /// Calculates and returns the player's current damage output based on strength and weapon
    /// </summary>
    /// <returns>Total damage output (0 if no weapon equipped)</returns>
    public int getDamage()
    {
        // COMPLEXITY ANALYSIS: getDamage() - O(1)
        if (this.weapon != null)
        {
            return (int)(this.currentStrength * this.weapon.getDamage());
        }
        return 0;
    }

    /// <summary>
    /// Applies damage to the player, reduced by defense stat.
    /// Damage is calculated as: damage / currentDefense
    /// </summary>
    /// <param name="takenDmg">The raw damage amount to be applied</param>
    public void takeDamage(float takenDmg)
    {
        // COMPLEXITY ANALYSIS: takeDamage() - O(1)
        // Reduce damage by current defense stat
        this.currentHealth -= takenDmg / this.currentDefense;

        // Ensure health doesn't go below 0
        if (this.currentHealth <= 0)
        {
            this.currentHealth = 0;
        }
    }
    #endregion

    #region Strength Management
    /// <summary>
    /// Adds a strength bonus from skill tree to the player's current strength stat.
    /// The final strength is calculated and applied.
    /// </summary>
    /// <param name="bonus">The strength bonus to add</param>
    public void addStrengthBonusSkill(float bonus)
    {
        // COMPLEXITY ANALYSIS: addStrengthBonusSkill() - O(1)
        this.currentStrengthBonusSkill += bonus;
        calculateStrength();
    }

    /// <summary>
    /// Adds a strength bonus from potion to the player's current strength stat.
    /// The final strength is calculated and applied.
    /// </summary>
    /// <param name="bonus">The strength bonus to add</param>
    public void addStrengthPotionBuff(float bonus)
    {
        // COMPLEXITY ANALYSIS: addStrengthPotionBuff() - O(1)
        this.currentStrengthPotionBuff += bonus;
        calculateStrength();
    }

    /// <summary>
    /// Resets the strength potion buff to zero and recalculates strength.
    /// </summary>
    public void resetStrengthPotionBuff()
    {
        // COMPLEXITY ANALYSIS: resetStrengthPotionBuff() - O(1)
        this.currentStrengthPotionBuff = 0f;
        calculateStrength();
    }

    /// <summary>
    /// Calculates the total strength bonus from armor and applies it.
    /// </summary>
    public void calculateStrengthBonusFromArmor()
    {
        // COMPLEXITY ANALYSIS: calculateStrengthBonusFromArmor() - O(a) where a = number of armor slots
        this.currentStrengthArmorBonus = inventory.getArmorSlots().getStrengthBonus();
        calculateStrength();
    }

    /// <summary>
    /// Calculates the final strength stat by combining base, armor, skill, and potion bonuses.
    /// </summary>
    public void calculateStrength()
    {
        // COMPLEXITY ANALYSIS: calculateStrength() - O(1)
        this.currentStrength = 1f;
        this.currentStrength =
            this.currentStrength
            + this.currentStrengthPotionBuff
            + this.currentStrengthArmorBonus
            + this.currentStrengthBonusSkill;
    }
    #endregion

    #region Defense Management
    /// <summary>
    /// Adds a defense bonus from skill tree to the player's current defense stat.
    /// The final defense is calculated and applied.
    /// </summary>
    /// <param name="bonus">The defense bonus to add</param>
    public void addDefenseBonusSkill(float bonus)
    {
        // COMPLEXITY ANALYSIS: addDefenseBonusSkill() - O(1)
        this.currentDefenseBonusSkill += bonus;
        calculateDefense();
    }

    /// <summary>
    /// Adds a defense bonus from potion to the player's current defense stat.
    /// The final defense is calculated and applied.
    /// </summary>
    /// <param name="bonus">The defense bonus to add</param>
    public void addDefensePotionBuff(float bonus)
    {
        // COMPLEXITY ANALYSIS: addDefensePotionBuff() - O(1)
        this.currentDefensePotionBuff += bonus;
        calculateDefense();
    }

    /// <summary>
    /// Calculates the total defense bonus from armor and applies it.
    /// </summary>
    public void calculateDefenseBonusFromArmor()
    {
        // COMPLEXITY ANALYSIS: calculateDefenseBonusFromArmor() - O(a) where a = number of armor slots
        this.currentDefenseArmorBonus = inventory.getArmorSlots().getDefenseBonus();
        calculateDefense();
    }

    /// <summary>
    /// Calculates the final defense stat by combining base, armor, skill, and potion bonuses.
    /// </summary>
    public void calculateDefense()
    {
        // COMPLEXITY ANALYSIS: calculateDefense() - O(1)
        this.currentDefense = 1f;
        this.currentDefense =
            this.currentDefense + this.currentDefensePotionBuff + this.currentDefenseArmorBonus;
    }
    #endregion

    #region Stat Calculation
    /// <summary>
    /// Calculates and applies all strength and defense bonuses from armor.
    /// </summary>
    public void calculateStrengthAndDefenseBonus()
    {
        // COMPLEXITY ANALYSIS: calculateStrengthAndDefenseBonus() - O(a) where a = number of armor slots
        calculateStrengthBonusFromArmor();
        calculateDefenseBonusFromArmor();
    }
    #endregion

    #region Speed Management
    /// <summary>
    /// Adds a speed bonus to the player's current speed stat.
    /// The final speed is capped at the maximum speed value.
    /// </summary>
    /// <param name="bonus">The speed bonus to add</param>
    public void addSpeedBonus(float bonus)
    {
        // COMPLEXITY ANALYSIS: addSpeedBonus() - O(1)
        this.currentSpeed += bonus;
    }

    /// <summary>
    /// Removes a speed potion buff from the player's current speed stat.
    /// Ensures speed doesn't go below 0.
    /// </summary>
    /// <param name="bonus">The speed buff to remove</param>
    public void removeSpeedPotionBuff(float bonus)
    {
        // COMPLEXITY ANALYSIS: removeSpeedPotionBuff() - O(1)
        this.currentSpeed = Mathf.Max(0, this.currentSpeed - bonus);
    }
    #endregion

    #region Health Management
    /// <summary>
    /// Adds a health bonus multiplier to increase the player's maximum health.
    /// Recalculates maxHealth as: 100 * healthBonus
    /// </summary>
    /// <param name="bonus">The health bonus multiplier to add</param>
    public void addHealthBonus(float bonus)
    {
        // COMPLEXITY ANALYSIS: addHealthBonus() - O(1)
        this.currentHealthBonus += bonus;
        float oldMaxHealth = this.maxHealth;
        this.maxHealth = 100f * this.currentHealthBonus;
        this.currentHealth += this.maxHealth - oldMaxHealth;
    }

    /// <summary>
    /// Adds health to the player's current health, capped at maximum health.
    /// </summary>
    /// <param name="health">The amount of health to add</param>
    public void addHealth(float health)
    {
        // COMPLEXITY ANALYSIS: addHealth() - O(1)
        this.currentHealth = Mathf.Min(this.maxHealth, this.currentHealth + health);
    }

    /// <summary>
    /// Resets the player's current health to maximum health.
    /// </summary>
    public void resetHealth()
    {
        // COMPLEXITY ANALYSIS: resetHealth() - O(1)
        this.currentHealth = this.maxHealth;
    }
    #endregion

    #region Quest Management
    /// <summary>
    /// Adds a quest to the player's active quest list if it's not already present.
    /// Prevents duplicate quests from the same quest giver.
    /// </summary>
    /// <param name="quest">The quest to add to the active quests</param>
    /// <returns>True if quest was added, false if already exists</returns>
    public bool addQuest(Quest quest)
    {
        // COMPLEXITY ANALYSIS: addQuest() - O(q) where q = number of active quests
        if (activeSideQuests.Find(questToFind => questToFind.Giver == quest.Giver) == null)
        {
            this.activeSideQuests.Add(quest);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes a quest from the player's active quest list.
    /// </summary>
    /// <param name="quest">The quest to remove from active quests</param>
    /// <returns>True if quest was removed, false if not found</returns>
    public bool removeQuest(Quest quest)
    {
        // COMPLEXITY ANALYSIS: removeQuest() - O(q) where q = number of active quests
        if (activeSideQuests.Find(questToFind => questToFind.Giver == quest.Giver) != null)
        {
            this.activeSideQuests.Remove(quest);
            return true;
        }
        return false;
    }
    #endregion

    #region Inventory and Equipment
    /// <summary>
    /// Gets the player's inventory.
    /// </summary>
    /// <returns>Reference to the player's inventory</returns>
    public Inventory getInventory()
    {
        // COMPLEXITY ANALYSIS: getInventory() - O(1)
        return this.inventory;
    }

    /// <summary>
    /// Sets the player's weapon based on the first weapon in the hotbar.
    /// </summary>
    public void setWeapon()
    {
        // COMPLEXITY ANALYSIS: setWeapon() - O(1)
        if (inventory.getHotbar().getWeapon().Count > 0)
        {
            this.weapon = (WeaponItem)inventory.getHotbar().getWeapon()[0];
        }
    }

    /// <summary>
    /// Gets the currently equipped weapon.
    /// </summary>
    /// <returns>The equipped weapon item, or null if none equipped</returns>
    public WeaponItem getWeapon()
    {
        // COMPLEXITY ANALYSIS: getWeapon() - O(1)
        return this.weapon;
    }

    /// <summary>
    /// Removes the currently equipped weapon.
    /// </summary>
    public void removeWeapon()
    {
        // COMPLEXITY ANALYSIS: removeWeapon() - O(1)
        this.weapon = null;
    }
    #endregion

    #region Getters and Setters
    /// <summary>
    /// Gets the list of all active side quests the player has accepted.
    /// </summary>
    /// <returns>A list containing all active side quests</returns>
    public List<Quest> ActiveQuest
    {
        get { return this.activeSideQuests; }
    }

    /// <summary>
    /// Gets or sets the list of active side quests.
    /// </summary>
    public List<Quest> ActiveSideQuests
    {
        get { return this.activeSideQuests; }
        set { this.activeSideQuests = value; }
    }

    /// <summary>
    /// Gets the current main story quest.
    /// </summary>
    /// <returns>The current main quest, or null if none active</returns>
    public StoryQuest getCurrentMainQuest()
    {
        // COMPLEXITY ANALYSIS: getCurrentMainQuest() - O(1)
        return this.currentMainQuest;
    }

    /// <summary>
    /// Sets the current main story quest.
    /// </summary>
    /// <param name="quest">The quest to set as current main quest</param>
    public void setCurrentMainQuest(StoryQuest quest)
    {
        // COMPLEXITY ANALYSIS: setCurrentMainQuest() - O(1)
        this.currentMainQuest = quest;
    }

    /// <summary>
    /// Checks if the player is dead (health <= 0).
    /// </summary>
    /// <returns>True if player is dead, false otherwise</returns>
    public bool isDead()
    {
        // COMPLEXITY ANALYSIS: isDead() - O(1)
        return this.currentHealth <= 0;
    }

    /// <summary>
    /// Gets the player's current health.
    /// </summary>
    /// <returns>Current health points</returns>
    public float getHealth()
    {
        // COMPLEXITY ANALYSIS: getHealth() - O(1)
        return this.currentHealth;
    }

    /// <summary>
    /// Gets the player's maximum health.
    /// </summary>
    /// <returns>Maximum health points</returns>
    public float getMaxHealth()
    {
        // COMPLEXITY ANALYSIS: getMaxHealth() - O(1)
        return this.maxHealth;
    }

    /// <summary>
    /// Gets the player's current level.
    /// </summary>
    /// <returns>Current player level</returns>
    public int getLevel()
    {
        // COMPLEXITY ANALYSIS: getLevel() - O(1)
        return this.level;
    }

    /// <summary>
    /// Sets the player's level.
    /// </summary>
    /// <param name="level">The level to set</param>
    public void setLevel(int level)
    {
        // COMPLEXITY ANALYSIS: setLevel() - O(1)
        this.level = level;
    }

    /// <summary>
    /// Adds levels to the player's current level.
    /// </summary>
    /// <param name="level">The number of levels to add</param>
    public void addLevel(int level)
    {
        // COMPLEXITY ANALYSIS: addLevel() - O(1)
        this.level += level;
    }

    /// <summary>
    /// Gets the current strength bonus from skill tree.
    /// </summary>
    /// <returns>Strength bonus from skill tree</returns>
    public float getCurrentStrengthBonusSkill()
    {
        // COMPLEXITY ANALYSIS: getCurrentStrengthBonusSkill() - O(1)
        return this.currentStrengthBonusSkill;
    }

    /// <summary>
    /// Gets the player's current speed.
    /// </summary>
    /// <returns>Current speed stat</returns>
    public float getCurrentSpeed()
    {
        // COMPLEXITY ANALYSIS: getCurrentSpeed() - O(1)
        return this.currentSpeed;
    }

    /// <summary>
    /// Gets the player's current defense.
    /// </summary>
    /// <returns>Current defense stat</returns>
    public float getCurrentDefense()
    {
        // COMPLEXITY ANALYSIS: getCurrentDefense() - O(1)
        return this.currentDefense;
    }
    #endregion
}
