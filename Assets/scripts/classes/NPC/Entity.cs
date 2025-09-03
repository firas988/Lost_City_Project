using UnityEngine;

/// <summary>
/// Represents a basic entity with health and damage capabilities.
/// Extends the NPC class to add combat-related functionality.
/// </summary>
public class Entity : NPC
{
    #region Private Fields

    /// <summary>
    /// Current health of the entity.
    /// </summary>
    private float health;

    /// <summary>
    /// Maximum health capacity of the entity.
    /// </summary>
    private float maxHealth;

    /// <summary>
    /// Current damage output of the entity.
    /// </summary>
    private float currentDamage;

    /// <summary>
    /// Minimum health value (cannot go below 0).
    /// </summary>
    private static readonly float MIN_HEALTH = 0f;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new Entity with specified parameters.
    /// </summary>
    /// <param name="id">Unique identifier for the entity.</param>
    /// <param name="name">Display name of the entity.</param>
    /// <param name="type">Type/category of the entity.</param>
    /// <param name="walkRadius">Radius within which the entity can walk.</param>
    /// <param name="areaMask">Navigation area mask for the entity.</param>
    /// <param name="waitTimeRange">Range of wait times between actions.</param>
    /// <param name="navMeshAreaName">Name of the navigation mesh area.</param>
    /// <param name="health">Initial health value.</param>
    /// <param name="speed">Movement speed of the entity.</param>
    /// <param name="maxSpeed">Maximum movement speed.</param>
    public Entity(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float health,
        float speed,
        float maxSpeed
    )
        : base(
            id,
            name,
            type,
            walkRadius,
            areaMask,
            waitTimeRange,
            navMeshAreaName,
            speed,
            maxSpeed
        )
    {
        this.health = health;
        this.maxHealth = health;
    }

    #endregion

    #region Health Management

    /// <summary>
    /// Gets the current damage output of the entity.
    /// </summary>
    /// <returns>The current damage value.</returns>
    public float getCurrentDamage()
    {
        return this.currentDamage;
    }

    /// <summary>
    /// Sets the current damage output of the entity.
    /// </summary>
    /// <param name="currentDamage">The new damage value.</param>
    public void setCurrentDamage(float currentDamage)
    {
        this.currentDamage = currentDamage;
    }

    /// <summary>
    /// Applies damage to the entity, reducing health.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void takeDamage(float damage)
    {
        this.health = Mathf.Max(this.health - damage, MIN_HEALTH);
    }

    /// <summary>
    /// Gets the current health of the entity.
    /// </summary>
    /// <returns>The current health value.</returns>
    public float getHealth()
    {
        return this.health;
    }

    /// <summary>
    /// Gets the maximum health capacity of the entity.
    /// </summary>
    /// <returns>The maximum health value.</returns>
    public float getMaxHealth()
    {
        return this.maxHealth;
    }

    /// <summary>
    /// Sets the health of the entity, ensuring it doesn't go below minimum.
    /// </summary>
    /// <param name="health">The new health value.</param>
    public void setHealth(float health)
    {
        this.health = Mathf.Max(health, MIN_HEALTH);
    }

    /// <summary>
    /// Checks if the entity is dead (health at or below minimum).
    /// </summary>
    /// <returns>True if the entity is dead; false otherwise.</returns>
    public bool isDead()
    {
        return this.health <= MIN_HEALTH;
    }

    #endregion
}
