using UnityEngine;

/// <summary>
/// Manages the grid-based slot system for the inventory interface, providing methods
/// to set and clear slots based on their row and column positions. Acts as a bridge
/// between the inventory data structure and the visual slot UI components.
/// </summary>
public class SlotManager : MonoBehaviour
{
    #region UI Components
    /// <summary>Array of Slot components that make up the inventory grid interface.</summary>
    [SerializeField]
    private Slot[] slots;
    #endregion

    #region Slot Management
    /// <summary>
    /// Sets the content of a specific slot in the inventory grid.
    /// Finds the slot by row and column coordinates and updates it with the provided item and count.
    /// </summary>
    /// <param name="item">The item to place in the slot.</param>
    /// <param name="count">The number of items in the stack.</param>
    /// <param name="row">The row index of the target slot.</param>
    /// <param name="column">The column index of the target slot.</param>
    // COMPLEXITY ANALYSIS: SetSlot() - O(s) where s = number of slots
    public void SetSlot(Item item, int count, int row, int column)
    {
        // Find the slot with matching coordinates
        foreach (Slot slot in slots)
        {
            if (slot.getRow() == row && slot.getColumn() == column)
            {
                // Set the slot content and exit
                slot.SetItem(item, count);
                return;
            }
        }
    }

    /// <summary>
    /// Clears the content of a specific slot in the inventory grid.
    /// Finds the slot by row and column coordinates and removes its content.
    /// </summary>
    /// <param name="row">The row index of the target slot.</param>
    /// <param name="column">The column index of the target slot.</param>
    // COMPLEXITY ANALYSIS: ClearSlot() - O(s) where s = number of slots
    public void ClearSlot(int row, int column)
    {
        // Find the slot with matching coordinates
        foreach (Slot slot in slots)
        {
            if (slot.getRow() == row && slot.getColumn() == column)
            {
                // Clear the slot content and exit
                slot.ClearSlot();
                return;
            }
        }
    }
    #endregion
}
