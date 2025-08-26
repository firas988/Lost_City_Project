using UnityEngine;

/// <summary>
/// ScriptableObject that defines the position of a slot within the inventory grid system.
/// Provides a reusable way to identify slot positions with row and column coordinates.
/// Can be created through the Unity menu system for easy inventory grid configuration.
/// </summary>
[CreateAssetMenu(fileName = "New SlotID", menuName = "Inventory/SlotID")]
public class SlotID : ScriptableObject
{
    #region Grid Coordinates
    /// <summary>
    /// The column index of the slot in the inventory grid.
    /// Range 0-4: represents the horizontal position in the 5-column grid.
    /// </summary>
    [SerializeField]
    [Range(0, 4)]
    private int column;

    /// <summary>
    /// The row index of the slot in the inventory grid.
    /// Range 0-3: represents the vertical position in the 4-row grid.
    /// </summary>
    [SerializeField]
    [Range(0, 3)]
    private int row;
    #endregion

    #region Public Interface Methods
    /// <summary>
    /// Gets the column index of this slot.
    /// </summary>
    /// <returns>The column coordinate (0-4) of the slot.</returns>
    public int getColumn()
    {
        return column;
    }

    /// <summary>
    /// Gets the row index of this slot.
    /// </summary>
    /// <returns>The row coordinate (0-3) of the slot.</returns>
    public int getRow()
    {
        return row;
    }
    #endregion
}
