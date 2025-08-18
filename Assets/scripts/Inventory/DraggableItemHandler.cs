using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the logic for dragging and interacting with inventory items.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DraggableItemHandler : MonoBehaviour, IPointerClickHandler
{
    /// ===== INSTANCE VARIABLES =====
    /// <summary>
    /// Reference to the input listener that tracks player's actions like taking an item.
    /// </summary>
    private InputListener inputListener;

    /// <summary>
    /// Reference to the InventoryManager responsible for modifying inventory state.
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// The actual inventory data being modified and displayed.
    /// </summary>
    private Inventory inventory;

    /// <summary>
    /// Reference to the InventoryAudioController responsible for playing audio.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the AudioSource component.
    /// </summary>
    private AudioSource audioSource;

    /// ===== UI ELEMENTS =====
    /// <summary>
    /// Prefab used to visually represent a draggable item while dragging.
    /// </summary>
    [SerializeField]
    private GameObject draggableItemPrefab;

    /// <summary>
    /// Reference to the slot currently being dragged.
    /// </summary>
    private Slot slotDraggableItem;

    /// <summary>
    /// ID information for the slot being interacted with.
    /// </summary>
    private SlotID slotID;

    /// <summary>
    /// The current GameObject under the mouse when clicking.
    /// </summary>
    private GameObject draggableItem;

    /// ===== BOOLEANS =====
    /// <summary>
    /// Indicates whether the draggable item has an active prefab assigned and in use.
    /// </summary>
    private bool isDraggableItemHaveAPrefab = false;

    /// ===== TAGS =====
    /// <summary>
    /// Tag used to identify UI elements that are inventory slots.
    /// </summary>
    private string slotTag = "Slot";

    /// <summary>
    /// Tag used to identify the delete area where items can be discarded.
    /// </summary>
    private string deleteItemTag = "DeleteItem";

    /// <summary>
    /// Tag used to identify the armor slot.
    /// </summary>
    private string armorSlotTag = "ArmorSlot";

    private string gameManagerTag = "GameManager";

    /// <summary>
    /// Initializes references to inventory manager, inventory and input listener.
    /// </summary>
    void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        inventory = inventoryManager.getInventory();
        inputListener = FindAnyObjectByType<InputListener>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates the position of the draggable item to follow the mouse.
    /// </summary>
    void Update()
    {
        if (draggableItem != null)
        {
            // Make the draggable item follow the mouse
            draggableItemPrefab.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Handles left click interactions for picking, dropping, or deleting items.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        // If no item is being dragged or player is taking one item
        if (draggableItem == null || inputListener.isTakingOneItem())
        {
            draggableItem = eventData.pointerCurrentRaycast.gameObject;
            if (draggableItem.CompareTag(slotTag))
            {
                takeItemFromSlot();
            }
            else if (draggableItem.CompareTag(armorSlotTag))
            {
                tryTakeItemFromArmorSlot();
            }
            else
            {
                TryDeleteItem();
            }
        }
        else
        {
            draggableItem = eventData.pointerCurrentRaycast.gameObject;
            if (draggableItem.CompareTag(deleteItemTag))
            {
                audioManager.playUI(audioSource, "DeleteItemFromInventory");
                TryDeleteItem();
            }
            else if (draggableItem.CompareTag(slotTag))
            {
                Slot slot = draggableItem.GetComponent<Slot>();
                if (draggableItem.GetComponent<Slot>().getIsEmpty())
                {
                    if (slot.getSlotID() != null)
                    {
                        TryMoveItemToEmptySlot(slot);
                    }
                    else
                    {
                        TryMoveItemToEmptyHotBar(slot);
                    }
                }
                else
                {
                    if (slot.getSlotID() != null)
                    {
                        TryMoveItemToNotEmptySlot(slot);
                    }
                    else
                    {
                        TryMoveItemToNotEmptyHotBar(slot);
                    }
                }
            }
            else if (draggableItem.CompareTag(armorSlotTag))
            {
                tryPutItemToArmorSlot();
            }
        }
    }

    private void TryMoveItemToNotEmptyHotBar(Slot slot)
    {
        if (slot.getSlotID() != null || slot.getHotBarIndex() == 0)
        {
            return;
        }

        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        if (!slot.getIsEmpty())
        {
            if (slot.getItem().id == slotDraggableItem.getItem().id)
            {
                // Check if full stack can be added
                if (slot.getCount() + slotDraggableItem.getCount() <= slot.getItem().maxStack)
                {
                    bool isSuccess = inventoryManager.AddItemToNotEmptyHotBar(
                        slotDraggableItem.getItem(),
                        slotDraggableItem.getCount(),
                        slot.getHotBarIndex()
                    );
                    if (isSuccess)
                    {
                        slot.addCount(slotDraggableItem.getCount());
                        audioManager.playUI(audioSource, "PutItemInInventory");
                        slotDraggableItem.ClearSlot();
                        TryDeleteItem();
                    }
                }
                else
                {
                    // Add only partial stack
                    int count = slot.getItem().maxStack - slot.getCount();

                    bool isSuccess = inventoryManager.AddItemToNotEmptyHotBar(
                        slotDraggableItem.getItem(),
                        count,
                        slot.getHotBarIndex()
                    );
                    if (isSuccess)
                    {
                        slot.addCount(count);
                        slotDraggableItem.removeCount(count);
                        audioManager.playUI(audioSource, "PutItemInInventory");
                    }
                }
            }
        }
    }

    private void TryMoveItemToEmptyHotBar(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        bool isSuccess = inventoryManager.TryMoveItemToEmptyHotBar(
            slotDraggableItem.getItem(),
            slotDraggableItem.getCount(),
            slot.getHotBarIndex()
        );
        if (isSuccess)
        {
            audioManager.playUI(audioSource, "PutItemInInventory");
            slot.SetItem(slotDraggableItem.getItem(), slotDraggableItem.getCount());
            slotDraggableItem.ClearSlot();
            TryDeleteItem();
        }
    }

    private void tryTakeItemFromArmorSlot()
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        if (!isDraggableItemHaveAPrefab && !draggableItem.GetComponent<ArmorSlot>().getIsEmpty())
        {
            Item item = draggableItem.GetComponent<ArmorSlot>().getItem();
            inventoryManager.TryRemoveItemFromArmorSlot(
                draggableItem.GetComponent<ArmorSlot>().getSlotArmorType().getArmorType()
            );
            draggableItemPrefab.SetActive(true);
            slotDraggableItem.SetItem(item, 1);
            isDraggableItemHaveAPrefab = true;
            // TODO: Play take item from armor slot audio   i have to change the audio controller
            audioManager.playUI(audioSource, "TakeItemFromInventory");
        }
        else
        {
            TryDeleteItem();
        }
    }

    private void tryPutItemToArmorSlot()
    {
        CosmeticType cosmeticType = draggableItem
            .GetComponent<ArmorSlot>()
            .getSlotArmorType()
            .getArmorType();
        bool isEmpty = draggableItem.GetComponent<ArmorSlot>().getIsEmpty();

        Item item = draggableItemPrefab.GetComponent<Slot>().getItem();
        if (item is CosmeticItem)
        {
            CosmeticItem cosmeticItem = (CosmeticItem)item;
            if (cosmeticItem.getCosmeticType() == cosmeticType && isEmpty)
            {
                if (inventoryManager.TryPutItemToArmorSlot(item, cosmeticType))
                {
                    audioManager.playUI(audioSource, "PutItemInInventory");
                    TryDeleteItem();
                }
            }
        }
    }

    private void takeItemFromSlot()
    {
        List<Item> items = null;
        slotID = draggableItem.GetComponent<Slot>().getSlotID();
        if (slotID != null)
        {
            items = inventoryManager.GetItemFromInventory(slotID.getRow(), slotID.getColumn());
        }
        else
        {
            items = inventoryManager.GetItemFromHotBar(
                draggableItem.GetComponent<Slot>().getHotBarIndex()
            );
        }
        if (inputListener.isTakingOneItem())
        {
            tryTakeOnItem(items);
        }
        else
        {
            TryTakeAllItem(items);
        }
    }

    /// <summary>
    /// Tries to move the dragged item into a non-empty slot. If same type and stackable, adds stack.
    /// </summary>
    /// <param name="slot">The target non-empty slot.</param>
    private void TryMoveItemToNotEmptySlot(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        if (!slot.getIsEmpty())
        {
            if (slot.getItem().id == slotDraggableItem.getItem().id)
            {
                // Check if full stack can be added
                if (slot.getCount() + slotDraggableItem.getCount() <= slot.getItem().maxStack)
                {
                    bool isSuccess = inventoryManager.AddItemToNotEmptySlot(
                        slotDraggableItem.getItem(),
                        slot.getRow(),
                        slot.getColumn(),
                        slotDraggableItem.getCount()
                    );
                    if (isSuccess)
                    {
                        slot.addCount(slotDraggableItem.getCount());

                        audioManager.playUI(audioSource, "PutItemInInventory");
                        slotDraggableItem.ClearSlot();
                        TryDeleteItem();
                    }
                }
                else
                {
                    // Add only partial stack
                    int count = slot.getItem().maxStack - slot.getCount();
                    slot.addCount(count);
                    slotDraggableItem.removeCount(count);
                    bool isSuccess = inventoryManager.AddItemToNotEmptySlot(
                        slotDraggableItem.getItem(),
                        slot.getRow(),
                        slot.getColumn(),
                        count
                    );
                    if (isSuccess)
                    {
                        audioManager.playUI(audioSource, "PutItemInInventory");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to move the dragged item into an empty slot.
    /// </summary>
    /// <param name="slot">The target empty slot.</param>
    private void TryMoveItemToEmptySlot(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (slot.getIsEmpty())
        {
            bool isSuccess = inventoryManager.AddItemToEmptySlot(
                slotDraggableItem.getItem(),
                slot.getRow(),
                slot.getColumn(),
                slotDraggableItem.getCount()
            );
            if (isSuccess)
            {
                audioManager.playUI(audioSource, "PutItemInInventory");
                slot.setEmpty(false);
                slot.SetItem(slotDraggableItem.getItem(), slotDraggableItem.getCount());
                TryDeleteItem();
            }
        }
    }

    /// <summary>
    /// Attempts to pick one item from the list if already dragging an item, or initialize dragging if not.
    /// </summary>
    /// <param name="items">List of items in the clicked slot.</param>
    private void tryTakeOnItem(List<Item> items)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (items != null && isDraggableItemHaveAPrefab && items.Count > 0)
        {
            int itemId = items[0].id;
            int draggableItemId = slotDraggableItem.getItem().id;
            if (itemId == draggableItemId && slotDraggableItem.getCount() < items[0].maxStack)
            {
                Item item = null;
                // Add one item to current dragging slot
                if (slotID != null)
                {
                    item = inventoryManager.TakeOnItem(slotID.getRow(), slotID.getColumn());
                }
                else
                {
                    item = inventoryManager.TakeOnItemFromHotBar(
                        draggableItem.GetComponent<Slot>().getHotBarIndex(),
                        draggableItem.GetComponent<Slot>()
                    );
                }
                if (item != null)
                {
                    slotDraggableItem.addCount(1);
                    audioManager.playUI(audioSource, "TakeItemFromInventory");
                }
            }
        }
        else if (items != null && !isDraggableItemHaveAPrefab && items.Count > 0)
        {
            Item item = null;
            // Start dragging item
            if (slotID != null)
            {
                item = inventoryManager.TakeOnItem(slotID.getRow(), slotID.getColumn());
            }
            else
            {
                item = inventoryManager.TakeOnItemFromHotBar(
                    draggableItem.GetComponent<Slot>().getHotBarIndex(),
                    draggableItem.GetComponent<Slot>()
                );
            }

            if (item != null)
            {
                draggableItemPrefab.SetActive(true);
                slotDraggableItem.SetItem(item, 1);
                isDraggableItemHaveAPrefab = true;
                audioManager.playUI(audioSource, "TakeItemFromInventory");
            }
        }
        else
        {
            TryDeleteItem();
        }
    }

    /// <summary>
    /// Takes all items from a slot and begins dragging them.
    /// </summary>
    /// <param name="items">List of items in the clicked slot.</param>
    private void TryTakeAllItem(List<Item> items)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        if (slotID != null)
        {
            inventoryManager.RemoveItemAndUpdateUI(slotID.getRow(), slotID.getColumn());
        }
        else
        {
            inventoryManager.RemoveItemFromHotBar(
                draggableItem.GetComponent<Slot>().getHotBarIndex()
            );
            draggableItem.GetComponent<Slot>().ClearSlot();
        }

        if (items != null && items.Count > 0)
        {
            draggableItemPrefab.SetActive(true);
            slotDraggableItem.SetItem(items[0], items.Count);
            isDraggableItemHaveAPrefab = true;
            audioManager.playUI(audioSource, "TakeItemFromInventory");
        }
        else
        {
            TryDeleteItem();
        }
    }

    /// <summary>
    /// Deletes the currently dragged item and clears its data.
    /// </summary>
    private void TryDeleteItem()
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        draggableItem = null;
        draggableItemPrefab.SetActive(false);
        slotDraggableItem.ClearSlot();
        isDraggableItemHaveAPrefab = false;
    }
}
