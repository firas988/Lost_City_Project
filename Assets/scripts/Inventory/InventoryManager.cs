using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages inventory-related actions including item addition, removal, and interaction with the UI slots.
/// </summary>
[RequireComponent(typeof(SlotManager))]
[RequireComponent(typeof(ArmorSlotManager))]
public class InventoryManager : MonoBehaviour
{
    /// ===== INSTANCE VARIABLES =====
    /// <summary>
    /// The inventory data structure.
    /// </summary>
    private Inventory inventory;

    /// <summary>
    /// Responsible for displaying and updating the slot UI.
    /// </summary>
    private SlotManager slotManager;

    /// <summary>
    /// Responsible for initializing the player.
    /// </summary>
    private StartPlayer startPlayer;

    /// <summary>
    /// Reference to the current player.
    /// </summary>
    private Player player;

    /// <summary>
    /// Responsible for initializing the armor slot manager.
    /// </summary>
    private ArmorSlotManager armorSlotManager;

    /// <summary>
    /// Reference to all items.
    /// </summary>
    [SerializeField]
    private ItemDatabase AllItems;

    /// <summary>
    /// Reference to the NotificationsManager.
    /// </summary>
    private NotificationsManager notificationsManager;

    /// <summary>
    /// Reference to the HotBarHandler.
    /// </summary>
    private HotBarHandler hotBarHandler;

    /// <summary>
    /// Reference to the HotBarHandlerInInventory.
    /// </summary>
    private HotBarHandlerInInventory hotBarHandlerInInventory;

    /// <summary>
    /// Returns the current Inventory instance.
    /// </summary>
    /// <returns>The inventory object.</returns>
    public Inventory getInventory()
    {
        return inventory;
    }

    /// <summary>
    /// Initializes references and sets the first hotbar item as the player's weapon.
    /// </summary>
    void Start()
    {
        slotManager = GetComponent<SlotManager>();
        armorSlotManager = GetComponent<ArmorSlotManager>();
        startPlayer = FindAnyObjectByType<StartPlayer>();
        player = startPlayer.getPlayer();
        inventory = player.getInventory();
        notificationsManager = FindAnyObjectByType<NotificationsManager>();
        hotBarHandler = GetComponent<HotBarHandler>();
        hotBarHandlerInInventory = GetComponent<HotBarHandlerInInventory>();
    }

    /// <summary>
    /// Checks for input keys to simulate item addition, listing, and mouse unlocking.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddItemToInventory(AllItems.GetRandomItem());
        }
    }

    /// <summary>
    /// Removes one item from a specific inventory slot and updates the slot UI.
    /// </summary>
    /// <param name="row">Row index of the item slot.</param>
    /// <param name="column">Column index of the item slot.</param>
    /// <returns>The item that was taken, or null if none.</returns>
    public Item TakeOnItem(int row, int column)
    {
        List<Item> items = inventory.GetItem(row, column);
        if (items != null)
        {
            Item item = items[0];
            if (items.Count == 1)
            {
                RemoveItemAndUpdateUI(row, column);
            }
            else
            {
                items.RemoveAt(0);
                slotManager.SetSlot(item, items.Count, row, column);
            }
            return item;
        }
        return null;
    }

    public Item TakeOnItemFromHotBar(int index, Slot slot)
    {
        List<Item> items = null;
        if (index == 0)
        {
            items = inventory.getHotbar().getWeapon();
            player.calculateStrengthAndDefenseBonus();
        }
        else
        {
            items = inventory.getHotbar().getConsumable(index);
        }
        if (items != null && items.Count > 0)
        {
            Item item = items[0];
            if (items.Count == 1)
            {
                RemoveItemFromHotBar(index);
                slot.ClearSlot();
            }
            else
            {
                items.RemoveAt(0);
                slot.SetItem(item, items.Count);
            }
            return item;
        }
        return null;
    }

    /// <summary>
    /// Completely removes all items from a given slot and clears its UI.
    /// </summary>
    /// <param name="row">Row index of the slot.</param>
    /// <param name="column">Column index of the slot.</param>
    public void RemoveItemAndUpdateUI(int row, int column)
    {
        inventory.RemoveItem(row, column);
        slotManager.ClearSlot(row, column);
    }

    public void RemoveItemFromHotBar(int index)
    {
        if (index == 0)
        {
            inventory.getHotbar().removeWeapon();
            player.removeWeapon();
            player.calculateStrengthAndDefenseBonus();
        }
        else
        {
            inventory.getHotbar().removeConsumable(index);
        }
        hotBarHandler.updateHotBar();
    }

    /// <summary>
    /// Adds items to an empty inventory slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Row index of the target slot.</param>
    /// <param name="column">Column index of the target slot.</param>
    /// <param name="count">The number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToEmptySlot(Item item, int row, int column, int count)
    {
        return inventory.AddItemToEmptySlot(item, row, column, count);
    }

    /// <summary>
    /// Adds items to a slot that already contains the same item type.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="row">Row index of the target slot.</param>
    /// <param name="column">Column index of the target slot.</param>
    /// <param name="count">The number of items to add.</param>
    /// <returns>True if added successfully, false otherwise.</returns>
    public bool AddItemToNotEmptySlot(Item item, int row, int column, int count)
    {
        return inventory.AddItemToNotEmptySlot(item, row, column, count);
    }

    public void AddItemToInventory(Item item)
    {
        Item newItem = ScriptableObject.Instantiate(item);

        if (newItem is WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)newItem;
            weaponItem.setDamage(GiveWeaponDamage.getDamage(weaponItem.rarity));
        }
        if (newItem is CosmeticItem)
        {
            CosmeticItem cosmeticItem = (CosmeticItem)newItem;
            cosmeticItem.setStrength(GiveCosmeticStrengthDefense.getStrength(cosmeticItem.rarity));
            cosmeticItem.setDefense(GiveCosmeticStrengthDefense.getDefense(cosmeticItem.rarity));
        }
        if (inventory.TryAddItem(newItem, out int row, out int column))
        {
            List<Item> items = inventory.GetItem(row, column);
            slotManager.SetSlot(newItem, items.Count, row, column);
            notificationsManager.ShowBottomLeftNotificationInventory(
                $"You have added {newItem.itemName} to your inventory."
            );
        }
        else
        {
            notificationsManager.ShowBottomLeftNotificationInventory("Inventory is full");
        }
    }

    public bool TryPutItemToArmorSlot(Item item, CosmeticType cosmeticType)
    {
        if (item is CosmeticItem)
        {
            switch (cosmeticType)
            {
                case CosmeticType.Helmet:
                    inventory.getArmorSlots().setHelmet(item);
                    armorSlotManager.setHelmet(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Chestplate:
                    inventory.getArmorSlots().setChestplate(item);
                    armorSlotManager.setChestplate(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Leggings:
                    inventory.getArmorSlots().setLeggings(item);
                    armorSlotManager.setLeggings(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
                case CosmeticType.Boots:
                    inventory.getArmorSlots().setBoots(item);
                    armorSlotManager.setBoots(item);
                    player.calculateStrengthAndDefenseBonus();
                    return true;
            }
        }
        return false;
    }

    public void TryRemoveItemFromArmorSlot(CosmeticType cosmeticType)
    {
        switch (cosmeticType)
        {
            case CosmeticType.Helmet:
                armorSlotManager.removeHelmet();
                inventory.getArmorSlots().removeHelmet();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Chestplate:
                armorSlotManager.removeChestplate();
                inventory.getArmorSlots().removeChestplate();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Leggings:
                armorSlotManager.removeLeggings();
                inventory.getArmorSlots().removeLeggings();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Boots:
                armorSlotManager.removeBoots();
                inventory.getArmorSlots().removeBoots();
                player.calculateStrengthAndDefenseBonus();
                break;
        }
    }

    public bool TryMoveItemToEmptyHotBar(Item item, int count, int index)
    {
        if (index == 0 && item is WeaponItem)
        {
            inventory.getHotbar().setWeapon(item);
            player.setWeapon();
            player.calculateStrengthAndDefenseBonus();
            hotBarHandler.updateHotBar();
            return true;
        }
        else if (index > 0 && index < 4 && item is ConsumableItem)
        {
            inventory.getHotbar().setConsumable(item, count, index);
            hotBarHandler.updateHotBar();
            return true;
        }
        hotBarHandler.updateHotBar();
        return false;
    }

    public bool AddItemToNotEmptyHotBar(Item item, int count, int index)
    {
        if (index == 0)
        {
            return false;
        }
        hotBarHandler.updateHotBar();
        return inventory.getHotbar().addToConsumable(item, count, index);
    }

    public List<Item> GetItemFromHotBar(int index)
    {
        if (index == 0)
        {
            player.setWeapon();
            player.calculateStrengthAndDefenseBonus();
            return inventory.getHotbar().getWeapon();
        }
        else
        {
            return inventory.getHotbar().getConsumable(index);
        }
    }

    public List<Item> GetItemFromInventory(int row, int column)
    {
        return inventory.GetItem(row, column);
    }

    /// <summary>
    /// Loads the inventory from a file.
    /// </summary>
    public void LoadInventory(InventroyData inventroyData)
    {
        if (inventroyData != null)
        {
            StartCoroutine(WaitForInventoryAndLoad(inventroyData));
        }
    }

    public IEnumerator WaitForInventoryAndLoad(InventroyData inventroyData)
    {
        yield return new WaitUntil(() => inventory != null);
        if (inventory.LoadInventory(inventroyData, AllItems))
        {
            updateInventoryUI();
            hotBarHandler.updateHotBar();
            hotBarHandlerInInventory.updateHotBarInInventory();
            updateArmorSlotsUI();
        }
    }

    public void updateInventoryUI()
    {
        List<Item>[,] items = inventory.GetItems();
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                if (items[i, j] != null)
                {
                    slotManager.SetSlot(items[i, j][0], items[i, j].Count, i, j);
                }
            }
        }
    }

    public void updateArmorSlotsUI()
    {
        armorSlotManager.setHelmet(inventory.getArmorSlots().getHelmet());
        armorSlotManager.setChestplate(inventory.getArmorSlots().getChestplate());
        armorSlotManager.setLeggings(inventory.getArmorSlots().getLeggings());
        armorSlotManager.setBoots(inventory.getArmorSlots().getBoots());
    }
}
