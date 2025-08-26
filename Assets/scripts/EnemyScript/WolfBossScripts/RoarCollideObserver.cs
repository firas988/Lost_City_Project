using System.Collections;
using UnityEngine;

/// <summary>
/// Observes collisions for the Wolf Boss roar attack, managing a growing sphere collider
/// that deals damage and applies strength debuffs to the player.
/// Integrates with WolfBossAttacking for attack coordination and Player for debuff effects.
/// </summary>
public class RoarCollideObserver : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the WolfBossAttacking script for attack coordination.</summary>
    WolfBossAttacking wolfBossAttacking;

    /// <summary>Reference to the SphereCollider for collision detection.</summary>
    private SphereCollider sphereCollider;
    #endregion

    #region Collider Growth Configuration
    /// <summary>Speed at which the collider grows during roar attack.</summary>
    private float growSpeed = 1f;

    /// <summary>Current radius of the collider during growth.</summary>
    private float currentRadius = 0;

    /// <summary>Maximum radius the collider can grow to during roar attack.</summary>
    private float maxRadius = 10f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes component references and sets up initial collider state.
    /// </summary>
    void Start()
    {
        // Get required components
        wolfBossAttacking = GetComponentInParent<WolfBossAttacking>();
        sphereCollider = GetComponent<SphereCollider>();

        // Store initial radius for reset
        currentRadius = sphereCollider.radius;
    }
    #endregion

    #region Collision Detection
    /// <summary>
    /// Called when another collider enters the roar attack trigger area.
    /// Deals damage and applies strength debuff to the player.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if roaring and player was hit
        if (wolfBossAttacking.getIsAttacking() && other.CompareTag("Player"))
        {
            // Get player reference and deal damage
            Player player = other.GetComponent<StartPlayer>().getPlayer();
            player.takeDamage(wolfBossAttacking.getCurrentAttackDMG());

            // Apply strength debuff effect
            StartCoroutine(StrengthDebuff(player));
        }

        // Disable collider after hit to prevent multiple hits
        sphereCollider.enabled = false;
    }
    #endregion

    #region Collider Management
    /// <summary>
    /// Enables the collider and starts the smooth growth animation.
    /// </summary>
    public void enableCollider()
    {
        sphereCollider.enabled = true;
        StartCoroutine(SmoothGrowCollider());
    }

    /// <summary>
    /// Coroutine that smoothly grows the collider from current to max radius.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator SmoothGrowCollider()
    {
        // Grow collider smoothly to maximum radius
        while (sphereCollider.radius < maxRadius)
        {
            sphereCollider.radius = Mathf.MoveTowards(
                sphereCollider.radius,
                maxRadius,
                growSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Wait before resetting to original size
        yield return new WaitForSeconds(1f);
        sphereCollider.radius = currentRadius;
    }
    #endregion

    #region Debuff System
    /// <summary>
    /// Coroutine that applies a strength debuff to the player for 30 seconds.
    /// </summary>
    /// <param name="player">The player to apply the debuff to.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator StrengthDebuff(Player player)
    {
        // Store current strength bonus
        float previousStrengthBonusSkill = player.getCurrentStrengthBonusSkill();

        // Remove all strength bonus (debuff)
        player.addStrengthBonusSkill(-previousStrengthBonusSkill);

        // Wait for debuff duration
        yield return new WaitForSeconds(30f);

        // Restore original strength bonus
        player.addStrengthBonusSkill(previousStrengthBonusSkill);
    }
    #endregion
}
