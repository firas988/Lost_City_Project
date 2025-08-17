using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField]
    private Slot[] slots;


    public void SetSlot(Item item, int count, int row, int column)
    {
        foreach (Slot slot in slots)
        {
            if (slot.getRow() == row && slot.getColumn() == column)
            {
                slot.SetItem(item, count);
                return;
            }
        }
    }

    public void ClearSlot(int row, int column)
    {
        foreach (Slot slot in slots)
        {
            if (slot.getRow() == row && slot.getColumn() == column)
            {
                slot.ClearSlot();
                return;
            }
        }
    }
}
