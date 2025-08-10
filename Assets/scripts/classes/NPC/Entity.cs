using UnityEngine;

public class Entity : NPC
{
    private float health;
    private float maxHealth;
    private float currentDamage;
    private static readonly float MIN_HEALTH = 0f;

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
        : base(id, name, type, walkRadius, areaMask, waitTimeRange, navMeshAreaName, speed, maxSpeed)
    {
        this.health = health;
        this.maxHealth = health;
    }

    public float getCurrentDamage()
    {
        return this.currentDamage;
    }

    public void setCurrentDamage(float currentDamage)
    {
        this.currentDamage = currentDamage;
    }

    public void takeDamage(float damage)
    {
        this.health = Mathf.Max(this.health - damage, MIN_HEALTH);
    }

    public float getHealth()
    {
        return this.health;
    }

    public float getMaxHealth()
    {
        return this.maxHealth;
    }

    public void setHealth(float health)
    {
        this.health = Mathf.Max(health, MIN_HEALTH);
    }

    public bool isDead()
    {
        return this.health <= MIN_HEALTH;
    }
}
