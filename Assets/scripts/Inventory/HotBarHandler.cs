using System.Collections.Generic;
using UnityEngine;

public class HotBarHandler : MonoBehaviour
{
    [SerializeField]
    private List<Slot> slots;

    private InventoryManager inventoryManager;

    private HotBarHandlerInInventory hotBarHandlerInInventory;

    private InputListener inputListener;

    private PotionHandler potionHandler;

    private string playerTag = "Player";

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        inputListener = transform.parent.GetComponentInChildren<InputListener>();
        potionHandler = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PotionHandler>();
        hotBarHandlerInInventory = GetComponent<HotBarHandlerInInventory>();
    }

    void Update()
    {
        if (inputListener.isPressingP1())
        {
            tryUsePotion(1);
        }
        if (inputListener.isPressingP2())
        {
            tryUsePotion(2);
        }
        if (inputListener.isPressingP3())
        {
            tryUsePotion(3);
        }
    }

    private void tryUsePotion(int index)
    {
        ConsumableItem item = inventoryManager.UseConsumableFromHotBar(index);

        if (item != null)
        {
            potionHandler.UsePotion(item);
            updateHotBarUI(index);
            hotBarHandlerInInventory.updateHotBarInInventoryUI(index);
        }
        else
        {
            Debug.Log("No item in hotbar");
        }
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

    public void updateHotBarUI(int index)
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
