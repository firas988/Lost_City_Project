using UnityEngine;
using System.Collections.Generic;

public class HotBarHandler : MonoBehaviour
{
    [SerializeField]
    private List<Slot> slots;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    public void updateHotBar()
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
}
