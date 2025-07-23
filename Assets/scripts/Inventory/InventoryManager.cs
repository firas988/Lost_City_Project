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

    /// ===== TEST CODE =====
    /// <summary>
    /// Test item to be added via keyboard input.
    /// </summary>
    public Item item;

    /// <summary>
    /// Test item to be added via keyboard input.
    /// </summary>
    public Item item3;

    /// <summary>
    /// Second test item to be added via keyboard input.
    /// </summary>
    public Item item2;

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
        LoadInventory();

        // Add a test item to the hotbar
        Item newItem = ScriptableObject.Instantiate(item);
        ((WeaponItem)newItem).setDamage(GiveWeaponDamage.getDamage(newItem.rarity));
    }

    /// <summary>
    /// Checks for input keys to simulate item addition, listing, and mouse unlocking.
    /// </summary>
    void Update()
    {
        Debug.Log(player.getWeapon());
        if (Input.GetKeyDown(KeyCode.J))
        {
            SaveInventory();
        }

        // [Test] Add item to inventory with Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // AddItemToInventory(item);
            AddItemToInventory(item3);
        }

        // // [Test] Log items in slot (0,0) with A key
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     List<Item> items = inventory.GetItem(0, 0);
        //     Debug.Log(items.Count);
        //     foreach (Item item in items)
        //     {
        //         Debug.Log(item.itemName);
        //     }
        // }

        // // [Test] Add item2 to inventory with C key
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     if (inventory.TryAddItem(item2, out int row, out int column))
        //     {
        //         List<Item> items = inventory.GetItem(row, column);
        //         slotManager.SetSlot(item2, items.Count, row, column);
        //     }
        // }

        // [Test] Unlock mouse with B key
        if (Input.GetKeyDown(KeyCode.B))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Chestplate:
                armorSlotManager.removeChestplate();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Leggings:
                armorSlotManager.removeLeggings();
                player.calculateStrengthAndDefenseBonus();
                break;
            case CosmeticType.Boots:
                armorSlotManager.removeBoots();
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
            return true;
        }
        else if (index > 0 && index < 4 && item is ConsumableItem)
        {
            inventory.getHotbar().setConsumable(item, count, index);
            return true;
        }
        return false;
    }

    public bool AddItemToNotEmptyHotBar(Item item, int count, int index)
    {
        if (index == 0)
        {
            return false;
        }
        return inventory.getHotbar().addToConsumable(item, count, index);
    }

    public List<Item> GetItemFromHotBar(int index)
    {
        if (index == 0)
        {
            return inventory.getHotbar().getWeapon();
            player.setWeapon();
            player.calculateStrengthAndDefenseBonus();
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
    /// Saves the inventory to a file.
    /// </summary>
    public void SaveInventory()
    {
        inventory.SaveInventory();
    }

    /// <summary>
    /// Loads the inventory from a file.
    /// </summary>
    public void LoadInventory()
    {
        InventroyData inventroyData = SaveSystem.LoadInventory();
        if (inventroyData != null)
        {
            if (inventory.LoadInventory(inventroyData, AllItems))
            {
                updateInventoryUI();
            }
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
}
