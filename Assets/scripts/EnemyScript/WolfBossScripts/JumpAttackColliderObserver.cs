using UnityEngine;

/// <summary>
/// Observes collisions for the Wolf Boss jump attack, managing hit detection
/// and damage dealing to the player. Integrates with WolfBossAttacking for attack coordination.
/// </summary>
public class JumpAttackColliderObserver : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the WolfBossAttacking script for attack coordination.</summary>
    private WolfBossAttacking wolfBossAttacking;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes component references on start.
    /// </summary>
    void Start()
    {
        // Get reference to parent WolfBossAttacking script
        wolfBossAttacking = GetComponentInParent<WolfBossAttacking>();
    }

    /// <summary>
    /// Update method - currently unused but available for future implementation.
    /// </summary>
    void Update() { }
    #endregion

    #region Collision Detection
    /// <summary>
    /// Called when another collider enters the jump attack trigger area.
    /// Deals damage to the player if currently attacking.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if currently attacking and player was hit
        if (wolfBossAttacking.getIsAttacking() && other.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            other
                .GetComponent<StartPlayer>()
                .getPlayer()
                .takeDamage(wolfBossAttacking.getCurrentAttackDMG());
        }

        // Disable collider to prevent multiple hits
        GetComponent<SphereCollider>().enabled = false;
    }
    #endregion

    #region Collider Management
    /// <summary>
    /// Enables the jump attack collider for hit detection during jump attacks.
    /// </summary>
    public void enableJumpAttackCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }
    #endregion
}
