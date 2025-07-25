using System;
using TMPro;
using UniRx;
using UnityEngine;

/// <summary>
/// Manages the available and spent skill points for the skill tree system.
/// Provides methods to check, update, and observe skill point changes.
/// </summary>
public class SkillAmountLimit : MonoBehaviour, ISkillAmountLimit
{
    #region Inspector Fields

    [SerializeField] public int totalAvailable;         // Initial total available skill points
    [SerializeField] public int totalSpent;             // Initial total spent skill points
    [SerializeField] public TMP_Text availableText;     // UI text for available points
    [SerializeField] public TMP_Text spentText;         // UI text for spent points

    #endregion

    #region Private Fields

    private readonly Subject<Unit> _amountChangedObserver = new Subject<Unit>(); // Observable for amount changes
    private int _totalAvailable;    // Current total available points
    private int _available;         // Current available points
    private int _spent;             // Current spent points

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the skill amount values and renders the UI.
    /// </summary>
    public void Awake()
    {
        _totalAvailable = totalAvailable;
        _spent = totalSpent;
        _available = _totalAvailable - _spent;
        Render();
    }

    #endregion

    #region Skill Point Logic

    /// <summary>
    /// Updates the spent points by the given amount and refreshes the UI.
    /// </summary>
    /// <param name="spent">Amount to add to spent points.</param>
    public void UpdateSpent(int spent)
    {
        if (!AreValidAmounts())
            return;

        _spent += spent;
        Debug.Log(_spent);
        _available = _totalAvailable - _spent;
        _amountChangedObserver.OnNext(Unit.Default);
        Render();
    }

    /// <summary>
    /// Checks if the spent amount does not exceed the total available.
    /// </summary>
    private bool AreValidAmounts() => _spent <= _totalAvailable;

    /// <summary>
    /// Returns true if at least one point can be spent.
    /// </summary>
    public bool CanSpend() => _available > 0;

    /// <summary>
    /// Returns true if the specified amount can be spent.
    /// </summary>
    public bool CanSpend(int amount) => _available >= amount;

    /// <summary>
    /// Returns true if any points have been spent (can take back).
    /// </summary>
    public bool CanTakeBack() => _spent > 0;

    #endregion

    #region Public API

    /// <summary>
    /// Gets the total skill points.
    /// </summary>
    public int GetTotalSkillPoints() => _totalAvailable;

    /// <summary>
    /// Gets the current available points.
    /// </summary>
    public int GetAvailable() => _available;

    /// <summary>
    /// Gets the current total spent points.
    /// </summary>
    public int GetTotalSpent() => _spent;

    /// <summary>
    /// Sets the spent points to a specific value.
    /// </summary>
    public void setSpent(int spent)
    {
        _spent = spent;
        _available = _totalAvailable - _spent;
        Render();
    }

    /// <summary>
    /// Sets the available points to a specific value.
    /// </summary>
    public void setAvailable(int available)
    {
        _available = available;
    }

    /// <summary>
    /// Sets the total available points and updates available points accordingly.
    /// </summary>
    public void setTotalAvailable(int totalAvailable)
    {
        _totalAvailable = totalAvailable;
        _available = _totalAvailable - _spent;
        Render();
    }

    /// <summary>
    /// Returns an observable that notifies when the amount changes.
    /// </summary>
    public IObservable<Unit> ObserveAmountChanged() => _amountChangedObserver;

    /// <summary>
    /// Adds to the total available points and updates the UI.
    /// </summary>
    /// <param name="amount">Amount to add to total available points.</param>
    public void AddTotalAvailable(int amount)
    {
        _totalAvailable += amount;
        Debug.Log(_totalAvailable);
        _available = _totalAvailable - _spent;
        _amountChangedObserver.OnNext(Unit.Default);
        Render();
    }

    /// <summary>
    /// Updates the UI text fields for available and spent points.
    /// </summary>
    public void Render()
    {
        availableText.text = (_totalAvailable - _spent).ToString();
        spentText.text = _spent.ToString();
    }

    #endregion
}
