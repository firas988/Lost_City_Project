using System.Collections.Generic;
using UnityEngine;

public class HotBarHandlerInInventory : MonoBehaviour
{
    [SerializeField]
    private List<Slot> slots;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    public void updateHotBarInInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            List<Item> items = inventoryManager.GetItemFromHotBar(slots[i].getHotBarIndex());
            if (items.Count > 0)
            {
                slots[i].SetItem(items[0], items.Count);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void updateHotBarInInventoryUI(int index)
    {
        List<Item> items = inventoryManager.GetItemFromHotBar(index);
        Slot slot = slots.Find(slot => slot.getHotBarIndex() == index);
        if (items.Count > 0)
        {
            slot.SetItem(items[0], items.Count);
        }
        else
        {
            slot.ClearSlot();
        }
    }
}
