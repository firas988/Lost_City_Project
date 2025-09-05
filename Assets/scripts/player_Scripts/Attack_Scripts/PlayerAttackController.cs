using UnityEngine;

/// <summary>
/// Manages the player's attacking system, including weapon setup, animations, input handling, and damage logic.
/// Coordinates between weapon management, animation control, and combat mechanics for comprehensive attack handling.
/// Integrates with inventory system for weapon equipping and hit detection for enemy damage.
/// </summary>
[RequireComponent(typeof(AnimateAttackController))]
[RequireComponent(typeof(StartPlayer))]
public class PlayerAttackController : MonoBehaviour
{
    #region Component References
    /// <summary>
    /// Controls attack and death animations for the player character.
    /// Required component for animation management.
    /// </summary>
    private AnimateAttackController animateAttackController;

    /// <summary>
    /// Initializes player reference and manages player data.
    /// Required component for player instance access.
    /// </summary>
    private StartPlayer startPlayer;

    /// <summary>
    /// Player instance containing stats, inventory, and weapon information.
    /// Core player data for attack calculations and weapon management.
    /// </summary>
    private Player player;

    /// <summary>
    /// Handles input events (attack, toggle, movement, jumping) from the player.
    /// Controls when attacks can be performed and movement restrictions.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to player's inventory for weapon access and management.
    /// Used to check currently equipped weapon and update weapon instances.
    /// </summary>
    private Inventory inventory;
    #endregion

    #region Weapon Management
    /// <summary>
    /// Currently equipped weapon GameObject in player's hand.
    /// Active weapon used for attack animations and hit detection.
    /// </summary>
    private GameObject currentWeapon = null;

    /// <summary>
    /// Visual copy of the weapon displayed in the weapon holder.
    /// Shown when weapon is not actively being used for attacks.
    /// </summary>
    private GameObject currentWeaponCopy = null;

    /// <summary>
    /// Prefab of the current weapon for instantiation and comparison.
    /// Used to detect weapon changes and create new weapon instances.
    /// </summary>
    private GameObject currentWeaponPrefab = null;

    /// <summary>
    /// Handles hit detection through collider activation and deactivation.
    /// Manages weapon collision detection during attack animations.
    /// </summary>
    private WeaponHitRelay weaponHitRelay;
    #endregion

    #region Weapon Positioning
    /// <summary>
    /// Parent object that holds the active weapon during attacks.
    /// Weapon is positioned here when performing attack animations.
    /// </summary>
    [SerializeField]
    private GameObject WeaponHand;

    /// <summary>
    /// Parent object that visually displays weapon when not in use.
    /// Weapon is positioned here during idle and non-attack states.
    /// </summary>
    [SerializeField]
    private GameObject WeaponHolder;
    #endregion

    #region Attack State Variables
    /// <summary>
    /// Tracks whether the player is currently in an attacking state.
    /// Controls attack animation playback and weapon positioning.
    /// </summary>
    [SerializeField]
    private bool isAttacking = false;

    /// <summary>
    /// Tracks whether the attack system is toggled on/off.
    /// Controls ranged vs melee stance and attack mode activation.
    /// </summary>
    private bool isToggleActivateAttack = false;

    /// <summary>
    /// True when the attack animation is done and ready for the next hit.
    /// Prevents multiple attacks during the same animation sequence.
    /// </summary>
    private bool isHitAnimationDone = false;

    /// <summary>
    /// Prevents multiple damage events in one swing.
    /// Ensures only one hit per attack animation.
    /// </summary>
    private bool isHit = false;

    /// <summary>
    /// Tracks whether the player can deal damage.
    /// Can be disabled for invulnerability or special game states.
    /// </summary>
    private bool canDealDamage = true;
    #endregion

    #region System References
    /// <summary>
    /// Tag used to find the GameManager GameObject in the scene.
    /// </summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes player, animator, and component references.
    /// Sets up the attack system and connects to required components.
    /// </summary>
    // COMPLEXITY ANALYSIS: Start() - O(1)
    void Start()
    {
        // Get required components for attack system
        animateAttackController = GetComponent<AnimateAttackController>();
        startPlayer = GetComponent<StartPlayer>();
        player = startPlayer.getPlayer();

        // Find input listener for attack controls
        inputListener = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<InputListener>();

        // Get inventory reference for weapon management
        inventory = player.getInventory();
    }

    /// <summary>
    /// Checks input and weapon state, and handles toggled attack state.
    /// Manages attack flow and input processing each frame.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Check weapon state and update if needed
        checkWeapon();

        // Check for attack toggle input
        checkActivateAttack();

        // Update attack animation state
        ToggleActivateAttackAnimation();

        // Handle attack input when system is active
        if (isToggleActivateAttack && isAttacking)
        {
            if (inputListener.isAttacking())
            {
                hit();
            }
        }
    }
    #endregion

    #region Attack System Management
    /// <summary>
    /// Toggles attack mode (e.g., ranged vs melee stance).
    /// Only allows toggle when player has a weapon equipped.
    /// </summary>
    // COMPLEXITY ANALYSIS: checkActivateAttack() - O(1)
    private void checkActivateAttack()
    {
        // Toggle attack mode if input detected and weapon is equipped
        if (inputListener.isToggleActivateAttack() && player.getWeapon() != null)
        {
            isToggleActivateAttack = !isToggleActivateAttack;
        }
        // Disable attack mode if no weapon is equipped
        else if (isToggleActivateAttack && player.getWeapon() == null)
        {
            isToggleActivateAttack = false;
        }
    }

    /// <summary>
    /// Instantiates weapon if needed, or swaps weapon if a new one is equipped.
    /// Also subscribes to weapon hit events for damage detection.
    /// </summary>
    // COMPLEXITY ANALYSIS: checkWeapon() - O(1)
    public void checkWeapon()
    {
        // Handle case when no weapon is equipped
        if (player.getWeapon() == null)
        {
            // Clean up existing weapon instances
            Destroy(currentWeapon);
            Destroy(currentWeaponCopy);
            currentWeapon = null;
            currentWeaponCopy = null;
            return;
        }

        // Create new weapon if none exists
        if (currentWeapon == null)
        {
            // Instantiate weapon in hand and holder
            currentWeaponPrefab = player.getWeapon().getItemPrefab();
            currentWeapon = Instantiate(currentWeaponPrefab);
            currentWeapon.transform.SetParent(WeaponHand.transform, false);
            currentWeaponCopy = Instantiate(currentWeapon);
            currentWeaponCopy.transform.SetParent(WeaponHolder.transform, false);

            // Set up hit detection for new weapon
            weaponHitRelay = currentWeapon.GetComponent<WeaponHitRelay>();
            weaponHitRelay.Subscribe(OnWeaponHit);
        }
        // Handle weapon swapping if different weapon is equipped
        else if (currentWeaponPrefab != player.getWeapon().getItemPrefab())
        {
            // Clean up old weapon instances
            Destroy(currentWeapon);
            Destroy(currentWeaponCopy);

            // Create new weapon instances
            currentWeaponPrefab = player.getWeapon().getItemPrefab();
            currentWeapon = Instantiate(currentWeaponPrefab);
            currentWeapon.transform.SetParent(WeaponHand.transform, false);
            currentWeaponCopy = Instantiate(currentWeapon);
            currentWeaponCopy.transform.SetParent(WeaponHolder.transform, false);

            // Set up hit detection for new weapon
            weaponHitRelay = currentWeapon.GetComponent<WeaponHitRelay>();
            weaponHitRelay.Subscribe(OnWeaponHit);
        }
    }

    /// <summary>
    /// Triggers start/stop of the attack animation based on toggle state.
    /// Manages the transition between attack and idle animation states.
    /// </summary>
    // COMPLEXITY ANALYSIS: ToggleActivateAttackAnimation() - O(1)
    public void ToggleActivateAttackAnimation()
    {
        // Start attack animation when toggle is activated
        if (isToggleActivateAttack && !isAttacking)
        {
            isAttacking = true;
            animateAttackController.StartAttackAnimation();
        }

        // Stop attack animation when toggle is deactivated
        if (!isToggleActivateAttack && isAttacking)
        {
            isAttacking = false;
            animateAttackController.StopAttackAnimation();
        }
    }
    #endregion

    #region Attack Execution
    /// <summary>
    /// Triggers an attack hit animation and enables the weapon's hit collider.
    /// Only works when the previous hit animation is complete.
    /// </summary>
    // COMPLEXITY ANALYSIS: hit() - O(1)
    public void hit()
    {
        if (isHitAnimationDone)
        {
            // Enable weapon hit detection and restrict movement
            weaponHitRelay.EnableCollider();
            inputListener.setCanMove(false);

            // Prevent repeat hits and play attack animation
            hitAnimationDisable();
            animateAttackController.AttackAnimation();
        }
    }

    /// <summary>
    /// Called when weapon hits a collider. If it's an enemy, deal damage.
    /// Prevents multiple hits during the same attack swing.
    /// </summary>
    /// <param name="other">The collider hit by the weapon.</param>
    // COMPLEXITY ANALYSIS: OnWeaponHit() - O(1)
    private void OnWeaponHit(Collider other)
    {
        // Check if hit target is an enemy and damage hasn't been dealt
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isHit && canDealDamage)
        {
            // Get enemy entity and apply damage
            StartNpc startNpc = other.gameObject.GetComponent<StartNpc>();
            Entity enemy = (Entity)startNpc.GetNpcsInstance();
            enemy.takeDamage(player.getDamage());

            // Mark hit as processed to prevent multiple damage
            isHit = true;
        }
    }
    #endregion

    #region Animation Event Handlers
    /// <summary>
    /// Called by animation event. Resets hit status and disables collider.
    /// Re-enables player movement and prepares for next attack.
    /// </summary>
    // COMPLEXITY ANALYSIS: hitAnimationDone() - O(1)
    public void hitAnimationDone()
    {
        // Reset attack state and re-enable movement
        isHitAnimationDone = true;
        inputListener.setCanMove(true);
        weaponHitRelay.DisableCollider();
        isHit = false;
    }

    /// <summary>
    /// Prevents attack from repeating until animation is done.
    /// Called at the start of attack animation to prevent multiple hits.
    /// </summary>
    // COMPLEXITY ANALYSIS: hitAnimationDisable() - O(1)
    public void hitAnimationDisable()
    {
        isHitAnimationDone = false;
    }

    /// <summary>
    /// Called by animation event to show weapon in the player's hand.
    /// Disables jumping and moves weapon to attack position.
    /// </summary>
    // COMPLEXITY ANALYSIS: startAttack() - O(1)
    public void startAttack()
    {
        // Disable jumping and show weapon in hand
        inputListener.setCanJump(false);
        WeaponHolder.SetActive(false);
        WeaponHand.SetActive(true);
    }

    /// <summary>
    /// Called by animation event to move weapon to holder.
    /// Re-enables jumping and moves weapon to idle position.
    /// </summary>
    // COMPLEXITY ANALYSIS: stopAttack() - O(1)
    public void stopAttack()
    {
        // Re-enable jumping and show weapon in holder
        inputListener.setCanJump(true);
        WeaponHolder.SetActive(true);
        WeaponHand.SetActive(false);
    }
    #endregion

    #region Damage Control
    /// <summary>
    /// Gets whether the player can currently deal damage.
    /// </summary>
    /// <returns>True if player can deal damage, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: GetCanDealDamage() - O(1)
    public bool GetCanDealDamage()
    {
        return this.canDealDamage;
    }

    /// <summary>
    /// Sets whether the player can currently deal damage.
    /// </summary>
    /// <param name="other">True to enable damage, false to disable.</param>
    // COMPLEXITY ANALYSIS: SetCanDealDamage() - O(1)
    public void SetCanDealDamage(bool other)
    {
        canDealDamage = other;
    }
    #endregion
}
