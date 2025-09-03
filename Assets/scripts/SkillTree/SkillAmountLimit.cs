using System;
using TMPro;
using UniRx;
using UnityEngine;

/// <summary>
/// Manages the available and spent skill points for the skill tree system.
/// Provides methods to check, update, and observe skill point changes.
/// Coordinates skill point allocation and provides reactive updates for UI synchronization.
/// </summary>
public class SkillAmountLimit : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Display")]
    /// <summary>
    /// UI text for displaying available skill points.
    /// Shows the player how many skill points they can spend.
    /// </summary>
    [SerializeField]
    public TMP_Text availableText;

    /// <summary>
    /// UI text for displaying spent skill points.
    /// Shows the player how many skill points they have already used.
    /// </summary>
    [SerializeField]
    public TMP_Text spentText;
    #endregion

    #region Private Fields
    [Header("Observable System")]
    /// <summary>
    /// Observable for amount changes that notifies subscribers when skill points change.
    /// Enables reactive UI updates when skill point values are modified.
    /// </summary>
    private readonly Subject<Unit> _amountChangedObserver = new Subject<Unit>();

    [Header("Skill Point Tracking")]
    /// <summary>
    /// Current total available skill points for the player.
    /// Represents the maximum skill points that can be spent.
    /// </summary>
    [SerializeField]
    private int _totalAvailable;

    /// <summary>
    /// Current available skill points that can be spent.
    /// Calculated as total available minus spent points.
    /// </summary>
    [SerializeField]
    private int _available;

    /// <summary>
    /// Current spent skill points.
    /// Tracks how many skill points the player has already used.
    /// </summary>
    [SerializeField]
    private int _spent;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the skill amount values and renders the UI.
    /// Sets up initial display of skill point information.
    /// </summary>
    public void Awake()
    {
        // Render the initial UI state
        Render();
    }
    #endregion

    #region Skill Point Logic Methods
    /// <summary>
    /// Updates the spent points by the given amount and refreshes the UI.
    /// Manages skill point spending and updates available points accordingly.
    /// </summary>
    /// <param name="spent">Amount to add to spent points.</param>
    public void UpdateSpent(int spent)
    {
        // Add to spent points and recalculate available points
        _spent += spent;
        _available = _totalAvailable - _spent;

        // Update the UI to reflect changes
        Render();
    }

    /// <summary>
    /// Returns true if at least one point can be spent.
    /// </summary>
    /// <returns>True if skill points are available for spending.</returns>
    public bool CanSpend() => _available > 0;

    /// <summary>
    /// Returns true if the specified amount can be spent.
    /// </summary>
    /// <param name="amount">The amount of skill points to check.</param>
    /// <returns>True if the specified amount can be spent, false otherwise.</returns>
    public bool CanSpend(int amount)
    {
        return _available > 0 && _available >= amount;
    }
    #endregion

    #region Public API Methods
    /// <summary>
    /// Gets the total skill points available.
    /// </summary>
    /// <returns>The total number of skill points the player can earn.</returns>
    public int GetTotalSkillPoints() => _totalAvailable;

    /// <summary>
    /// Gets the current available points.
    /// </summary>
    /// <returns>The number of skill points currently available for spending.</returns>
    public int GetAvailable() => _available;

    /// <summary>
    /// Gets the current total spent points.
    /// </summary>
    /// <returns>The total number of skill points already spent.</returns>
    public int GetTotalSpent() => _spent;

    /// <summary>
    /// Sets the spent points to a specific value.
    /// Updates available points and refreshes the UI.
    /// </summary>
    /// <param name="spent">The new value for spent skill points.</param>
    public void setSpent(int spent)
    {
        _spent = spent;
        _available = _totalAvailable - _spent;
        Render();
    }

    /// <summary>
    /// Sets the available points to a specific value.
    /// Note: This method only sets available points without updating total or spent.
    /// </summary>
    /// <param name="available">The new value for available skill points.</param>
    public void setAvailable(int available)
    {
        _available = available;
    }

    /// <summary>
    /// Sets the total available points and updates available points accordingly.
    /// Recalculates available points based on current spent amount.
    /// </summary>
    /// <param name="totalAvailable">The new total available skill points.</param>
    public void setTotalAvailable(int totalAvailable)
    {
        _totalAvailable = totalAvailable;
        _available = _totalAvailable - _spent;
        Render();
    }

    /// <summary>
    /// Returns an observable that notifies when the amount changes.
    /// Enables reactive programming patterns for UI updates.
    /// </summary>
    /// <returns>Observable that emits when skill point amounts change.</returns>
    public IObservable<Unit> ObserveAmountChanged() => _amountChangedObserver;

    /// <summary>
    /// Adds to the total available points and updates the UI.
    /// Triggers amount change notifications for reactive UI updates.
    /// </summary>
    /// <param name="amount">Amount to add to total available points.</param>
    public void AddTotalAvailable(int amount)
    {
        // Add to total available and recalculate available points
        _totalAvailable += amount;
        _available = _totalAvailable - _spent;

        // Notify observers of the change and update UI
        _amountChangedObserver.OnNext(Unit.Default);
        Render();
    }

    /// <summary>
    /// Updates the UI text fields for available and spent points.
    /// Ensures the display reflects current skill point values.
    /// </summary>
    public void Render()
    {
        // Update UI text to show current skill point values
        availableText.text = _available.ToString();
        spentText.text = _spent.ToString();
    }
    #endregion
}
