using UnityEngine;

/// <summary>
/// Defines the basic structure and behavior of enemy attack logic.
/// Provides necessary data for animation, damage, range, and timing.
/// </summary>
public interface EnemyAttackBehavior
{
    /// <summary>
    /// Gets the range within which the enemy can hit its target.
    /// </summary>
    /// <returns>Attack range as a float.</returns>
    float getAttackRange();

    /// <summary>
    /// Gets the time it takes for the enemy to perform an attack.
    /// </summary>
    /// <returns>Attack duration or cooldown as a float.</returns>
    float getAttackTime();

    /// <summary>
    /// Gets the amount of damage dealt by the attack.
    /// </summary>
    /// <returns>Damage value as a float.</returns>
    float getAttackDamage();

    /// <summary>
    /// Gets the name identifier of the current attack.
    /// </summary>
    /// <returns>Name of the attack as a string.</returns>
    string getAttackName();

    /// <summary>
    /// Checks whether the attack animation is currently playing.
    /// </summary>
    /// <returns>True if the animation is playing, false otherwise.</returns>
    bool isAttackAnimationPlaying();
}
