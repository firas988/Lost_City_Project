using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the health system for the Wolf Boss enemy, including health tracking,
/// boss bar updates, death handling, and visual effects.
/// Integrates with Entity, BossBarHandler, and DissolvingController components.
/// </summary>
public class WolfHealthManager : MonoBehaviour
{
    #region Component References
    /// <summary>Reference to the Entity component for health and death status.</summary>
    [SerializeField]
    private Entity entity;

    /// <summary>Reference to the BossBarHandler for UI health bar management.</summary>
    private BossBarHandler bossBar;
    #endregion

    #region Health Variables
    /// <summary>Current health value of the wolf boss.</summary>
    [SerializeField]
    private float Curhealth;
    #endregion

    #region Configuration
    /// <summary>Tag identifier for the wolf boss enemy.</summary>
    [SerializeField]
    private string bossTag = "WolfBoss";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes health manager and sets up initial health values.
    /// </summary>
    void Start()
    {
        // Get entity reference and initial health
        entity = (Entity)GetComponent<StartNpc>().GetNpcsInstance();
        Curhealth = entity.getHealth();

        // Find boss bar handler for UI updates
        bossBar = GameObject.FindAnyObjectByType<BossBarHandler>();
    }

    /// <summary>
    /// Called every frame to monitor health changes and handle death logic.
    /// </summary>
    void Update()
    {
        // Debug key for testing (set health to 0)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.setHealth(0);
        }
        // Check if the bossBar is null
        if (bossBar == null)
        {
            bossBar = GameObject.FindAnyObjectByType<BossBarHandler>();
            return;
        }

        // Check if health has changed
        if (Curhealth != entity.getHealth())
        {
            // Update current health value
            Curhealth = entity.getHealth();

            // Check if boss is dead
            if (Curhealth <= 0)
            {
                GetComponent<Animator>().SetBool("IsDead", true);
            }

            // Update boss health bar UI
            bossBar.TakeDamage(Curhealth / entity.getMaxHealth());
        }
    }
    #endregion

    #region Death Handling
    /// <summary>
    /// Handles the wolf boss death sequence.
    /// </summary>
    public void WolfBossDead()
    {
        StartCoroutine(disAppearBossBar());
    }

    /// <summary>
    /// Coroutine that manages the boss death sequence and cleanup.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator disAppearBossBar()
    {
        // Start dissolve effect
        GetComponent<DissolvingController>().StartDissolve();

        // Wait for dissolve effect to complete
        yield return new WaitForSeconds(2.5f);

        // Notify enemy handler and destroy boss
        KillEnemyHandler.KilledEnemy(bossTag);
        bossBar.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    #endregion
}
