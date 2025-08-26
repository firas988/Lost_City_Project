using UnityEngine;

/// <summary>
/// Observes collisions for the Wolf Boss hand/swing attack, managing hit detection
/// and damage dealing to the player. Integrates with WolfBossAttacking for attack coordination.
/// </summary>
public class HandCollisionObserver : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>Flag indicating if the player has been hit by this attack.</summary>
    [SerializeField]
    private bool playerHit = false;
    #endregion

    #region Private Variables
    /// <summary>Counter for tracking how many times the trigger has been entered.</summary>
    private int countEntries = 0;

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
    /// Called when another collider enters the hand attack trigger area.
    /// Deals damage to the player if currently attacking.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if currently attacking
        if (wolfBossAttacking.getIsAttacking())
        {
            // Check if player was hit
            if (other.gameObject.CompareTag("Player"))
            {
                // Deal damage to the player
                other
                    .GetComponent<StartPlayer>()
                    .getPlayer()
                    .takeDamage(wolfBossAttacking.getCurrentAttackDMG());
            }
        }

        // Disable collider to prevent multiple hits
        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Called when another collider exits the hand attack trigger area.
    /// Currently unused but available for future implementation.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        // Exit logic can be implemented here if needed
    }
    #endregion

    #region Collider Management
    /// <summary>
    /// Enables the hand collider for hit detection during swing attacks.
    /// </summary>
    public void enableHandCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// Disables the hand collider to prevent multiple hits.
    /// </summary>
    public void disableHandCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
    #endregion
}
