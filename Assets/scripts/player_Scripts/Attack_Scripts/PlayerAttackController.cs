using UnityEngine;

/// <summary>
/// Manages the player's attacking system, including weapon setup, animations, input handling, and damage logic.
/// </summary>
[RequireComponent(typeof(AnimateAttackController))]
[RequireComponent(typeof(StartPlayer))]
public class PlayerAttackController : MonoBehaviour
{
    /// ===== INSTANCE VARIABLES =====
    /// <summary>Controls attack and death animations.</summary>
    private AnimateAttackController animateAttackController;

    /// <summary>Initializes player reference.</summary>
    private StartPlayer startPlayer;

    /// <summary>Player instance containing stats and inventory.</summary>
    private Player player;

    /// <summary>Handles input events (attack, toggle, etc.).</summary>
    private InputListener inputListener;

    /// <summary>Reference to player's inventory.</summary>
    private Inventory inventory;

    /// <summary>Currently equipped weapon in player's hand.</summary>
    private GameObject currentWeapon = null;

    /// <summary>Visual copy of the weapon displayed in holder.</summary>
    private GameObject currentWeaponCopy = null;

    /// <summary>Prefab of the current weapon.</summary>
    private GameObject currentWeaponPrefab = null;

    /// <summary>Handles hit detection through collider activation.</summary>
    private WeaponHitRelay weaponHitRelay;

    /// <summary>Parent object that holds the active weapon (for attack).</summary>
    [SerializeField]
    private GameObject WeaponHand;

    /// <summary>Parent object that visually displays weapon when not in use.</summary>
    [SerializeField]
    private GameObject WeaponHolder;

    /// <summary>Reference to the statistics handler.</summary>
    private StatisticsHandler statisticsHandler;

    /// ===== BOOLEANS =====
    /// <summary>Tracks whether the player is currently attacking.</summary>
    [SerializeField]
    private bool isAttacking = false;

    /// <summary>Tracks whether the attack system is toggled on/off.</summary>
    private bool isToggleActivateAttack = false;

    /// <summary>True when the animation is done and ready for the next hit.</summary>
    private bool isHitAnimationDone = false;

    /// <summary>Prevents multiple damage events in one swing.</summary>
    private bool isHit = false;

    /// <summary>Tracks whether the player is dead.</summary>
    private bool isDead = false;

    /// <summary>Tracks whether the player can deal damage.</summary>
    private bool canDealDamage = true;

    /// ===== METHODS =====
    /// <summary>Initializes player, animator, and references.</summary>
    void Start()
    {
        animateAttackController = GetComponent<AnimateAttackController>();
        startPlayer = GetComponent<StartPlayer>();
        player = startPlayer.getPlayer();
        inputListener = FindAnyObjectByType<InputListener>();
        inventory = player.getInventory();
        statisticsHandler = FindAnyObjectByType<StatisticsHandler>();
    }

    /// <summary>Checks input and weapon state, and handles toggled attack state.</summary>
    void Update()
    {
        checkWeapon();

        checkActivateAttack();

        ToggleActivateAttackAnimation();

        if (isToggleActivateAttack && isAttacking)
        {
            if (inputListener.isAttacking())
            {
                hit();
            }
        }

        checkDeath();
    }

    private void checkActivateAttack()
    { // Toggle attack mode (e.g. ranged vs melee stance)
        if (inputListener.isToggleActivateAttack() && player.getWeapon() != null)
        {
            isToggleActivateAttack = !isToggleActivateAttack;
        }
        else if (isToggleActivateAttack && player.getWeapon() == null)
        {
            isToggleActivateAttack = false;
        }
    }

    /// <summary>
    /// Disables input and plays death animation if the player is dead.
    /// </summary>
    public void checkDeath()
    {
        if (player.isDead() && !isDead)
        {
            inputListener.setCanAttack(false);
            inputListener.setCanMove(false);
            animateAttackController.DeathAnimation();
            statisticsHandler.Death();
            isDead = true;
        }
    }

    /// <summary>
    /// Instantiates weapon if needed, or swaps weapon if a new one is equipped.
    /// Also subscribes to weapon hit events.
    /// </summary>
    public void checkWeapon()
    {
        if (player.getWeapon() == null)
        {
            Destroy(currentWeapon);
            Destroy(currentWeaponCopy);
            currentWeapon = null;
            currentWeaponCopy = null;
            return;
        }
        if (currentWeapon == null)
        {
            currentWeaponPrefab = player.getWeapon().itemPrefab;
            currentWeapon = Instantiate(currentWeaponPrefab);
            currentWeapon.transform.SetParent(WeaponHand.transform, false);
            currentWeaponCopy = Instantiate(currentWeapon);
            currentWeaponCopy.transform.SetParent(WeaponHolder.transform, false);

            weaponHitRelay = currentWeapon.GetComponent<WeaponHitRelay>();
            weaponHitRelay.Subscribe(OnWeaponHit);
        }
        else if (currentWeaponPrefab != player.getWeapon().itemPrefab)
        {
            Destroy(currentWeapon);
            Destroy(currentWeaponCopy);

            currentWeaponPrefab = player.getWeapon().itemPrefab;
            currentWeapon = Instantiate(currentWeaponPrefab);
            currentWeapon.transform.SetParent(WeaponHand.transform, false);
            currentWeaponCopy = Instantiate(currentWeapon);
            currentWeaponCopy.transform.SetParent(WeaponHolder.transform, false);

            weaponHitRelay = currentWeapon.GetComponent<WeaponHitRelay>();
            weaponHitRelay.Subscribe(OnWeaponHit);
        }
    }

    /// <summary>
    /// Triggers start/stop of the attack animation based on toggle state.
    /// </summary>
    public void ToggleActivateAttackAnimation()
    {
        if (isToggleActivateAttack && !isAttacking)
        {
            isAttacking = true;
            animateAttackController.StartAttackAnimation();
        }

        if (!isToggleActivateAttack && isAttacking)
        {
            isAttacking = false;
            animateAttackController.StopAttackAnimation();
        }
    }

    /// <summary>
    /// Triggers an attack hit animation and enables the weapon's hit collider.
    /// </summary>
    public void hit()
    {
        if (isHitAnimationDone)
        {
            weaponHitRelay.EnableCollider();
            inputListener.setCanMove(false);
            hitAnimationDisable(); // Prevent repeat hits
            animateAttackController.AttackAnimation();
        }
    }

    /// <summary>
    /// Called when weapon hits a collider. If it's an enemy, deal damage.
    /// </summary>
    /// <param name="other">The collider hit by the weapon.</param>
    private void OnWeaponHit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isHit && canDealDamage)
        {
            StartNpc startNpc = other.gameObject.GetComponent<StartNpc>();
            Entity enemy = (Entity)startNpc.GetNpcsInstance();
            enemy.takeDamage(player.getDamage());
            isHit = true;
        }
    }

    /// <summary>
    /// Called by animation event. Resets hit status and disables collider.
    /// </summary>
    public void hitAnimationDone()
    {
        isHitAnimationDone = true;
        inputListener.setCanMove(true);
        weaponHitRelay.DisableCollider();
        isHit = false;
    }

    /// <summary>
    /// Prevents attack from repeating until animation is done.
    /// </summary>
    public void hitAnimationDisable()
    {
        isHitAnimationDone = false;
    }

    /// <summary>
    /// Called by animation event to show weapon in the player's hand.
    /// </summary>
    public void startAttack()
    {
        inputListener.setCanJump(false);
        WeaponHolder.SetActive(false);
        WeaponHand.SetActive(true);
    }

    /// <summary>
    /// Called by animation event to move weapon to holder.
    /// </summary>
    public void stopAttack()
    {
        inputListener.setCanJump(true);
        WeaponHolder.SetActive(true);
        WeaponHand.SetActive(false);
    }

    public bool GetCanDealDamage()
    {
        return this.canDealDamage;
    }

    public void SetCanDealDamage(bool other)
    {
        canDealDamage = other;
    }
}
