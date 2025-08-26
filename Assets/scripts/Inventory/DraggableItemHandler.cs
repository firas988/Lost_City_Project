using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the logic for dragging and interacting with inventory items, providing a comprehensive
/// drag-and-drop system for inventory management. Manages item movement between slots, hotbar,
/// and armor slots with proper validation and audio feedback.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DraggableItemHandler : MonoBehaviour, IPointerClickHandler
{
    #region Core Components
    /// <summary>Reference to the input listener that tracks player's actions like taking an item.</summary>
    private InputListener inputListener;

    /// <summary>Reference to the InventoryManager responsible for modifying inventory state.</summary>
    private InventoryManager inventoryManager;

    /// <summary>The actual inventory data being modified and displayed.</summary>
    private Inventory inventory;

    /// <summary>Reference to the AudioManager responsible for playing inventory audio effects.</summary>
    private AudioManager audioManager;

    /// <summary>Reference to the AudioSource component for playing audio.</summary>
    private AudioSource audioSource;
    #endregion

    #region UI Elements
    /// <summary>Prefab used to visually represent a draggable item while dragging.</summary>
    [SerializeField]
    private GameObject draggableItemPrefab;

    /// <summary>Reference to the slot currently being dragged.</summary>
    private Slot slotDraggableItem;

    /// <summary>ID information for the slot being interacted with.</summary>
    private SlotID slotID;

    /// <summary>The current GameObject under the mouse when clicking.</summary>
    private GameObject draggableItem;
    #endregion

    #region State Variables
    /// <summary>Indicates whether the draggable item has an active prefab assigned and in use.</summary>
    private bool isDraggableItemHaveAPrefab = false;
    #endregion

    #region Configuration Tags
    /// <summary>Tag used to identify UI elements that are inventory slots.</summary>
    private string slotTag = "Slot";

    /// <summary>Tag used to identify the delete area where items can be discarded.</summary>
    private string deleteItemTag = "DeleteItem";

    /// <summary>Tag used to identify the armor slot.</summary>
    private string armorSlotTag = "ArmorSlot";

    /// <summary>Tag used to identify the GameManager for finding audio components.</summary>
    private string gameManagerTag = "GameManager";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes references to inventory manager, inventory, input listener, and audio components.
    /// Sets up the foundation for inventory interaction and audio feedback.
    /// </summary>
    void Start()
    {
        // Get required component references
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        inventory = inventoryManager.getInventory();
        inputListener = FindAnyObjectByType<InputListener>();

        // Get audio components
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates the position of the draggable item to follow the mouse cursor.
    /// Ensures the dragged item stays visually connected to the mouse position.
    /// </summary>
    void Update()
    {
        if (draggableItem != null)
        {
            // Make the draggable item follow the mouse
            draggableItemPrefab.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    #endregion

    #region Input Handling
    /// <summary>
    /// Handles left click interactions for picking, dropping, or deleting items.
    /// Manages the complete drag-and-drop workflow including slot validation and item movement.
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity's event system.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Only handle left mouse button clicks
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // If no item is being dragged or player is taking one item
        if (draggableItem == null || inputListener.isTakingOneItem())
        {
            // Start dragging or take item
            draggableItem = eventData.pointerCurrentRaycast.gameObject;

            if (draggableItem.CompareTag(slotTag))
            {
                // Handle slot interaction
                takeItemFromSlot();
            }
            else if (draggableItem.CompareTag(armorSlotTag))
            {
                // Handle armor slot interaction
                tryTakeItemFromArmorSlot();
            }
            else
            {
                // Handle delete area interaction
                TryDeleteItem();
            }
        }
        else
        {
            // Item is being dragged - handle drop logic
            draggableItem = eventData.pointerCurrentRaycast.gameObject;

            if (draggableItem.CompareTag(deleteItemTag))
            {
                // Drop item in delete area
                audioManager.playUI(audioSource, "DeleteItemFromInventory");
                TryDeleteItem();
            }
            else if (draggableItem.CompareTag(slotTag))
            {
                // Handle slot drop logic
                Slot slot = draggableItem.GetComponent<Slot>();

                if (draggableItem.GetComponent<Slot>().getIsEmpty())
                {
                    // Drop in empty slot
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
                    // Drop in non-empty slot
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
                // Handle armor slot drop
                tryPutItemToArmorSlot();
            }
        }
    }
    #endregion

    #region Hotbar Item Movement
    /// <summary>
    /// Attempts to move a dragged item to a non-empty hotbar slot.
    /// Handles stacking logic for consumable items with proper validation.
    /// </summary>
    /// <param name="slot">The target hotbar slot to move the item to.</param>
    private void TryMoveItemToNotEmptyHotBar(Slot slot)
    {
        // Validate slot type and weapon slot restrictions
        if (slot.getSlotID() != null || slot.getHotBarIndex() == 0)
        {
            return;
        }

        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (!slot.getIsEmpty())
        {
            // Check if items can be stacked
            if (slot.getItem().id == slotDraggableItem.getItem().id)
            {
                // Check if full stack can be added
                if (slot.getCount() + slotDraggableItem.getCount() <= slot.getItem().maxStack)
                {
                    // Add entire stack
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
                    // Add only partial stack due to max stack limit
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

    /// <summary>
    /// Attempts to move a dragged item to an empty hotbar slot.
    /// Validates item type compatibility and updates the UI accordingly.
    /// </summary>
    /// <param name="slot">The target empty hotbar slot.</param>
    private void TryMoveItemToEmptyHotBar(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        // Attempt to move item to hotbar
        bool isSuccess = inventoryManager.TryMoveItemToEmptyHotBar(
            slotDraggableItem.getItem(),
            slotDraggableItem.getCount(),
            slot.getHotBarIndex()
        );

        if (isSuccess)
        {
            // Successfully moved - update UI and clear dragged item
            audioManager.playUI(audioSource, "PutItemInInventory");
            slot.SetItem(slotDraggableItem.getItem(), slotDraggableItem.getCount());
            slotDraggableItem.ClearSlot();
            TryDeleteItem();
        }
    }
    #endregion

    #region Armor Slot Management
    /// <summary>
    /// Attempts to take an item from an armor slot and begin dragging it.
    /// Handles the removal of equipped armor items with proper validation.
    /// </summary>
    private void tryTakeItemFromArmorSlot()
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (!isDraggableItemHaveAPrefab && !draggableItem.GetComponent<ArmorSlot>().getIsEmpty())
        {
            // Get item from armor slot and start dragging
            Item item = draggableItem.GetComponent<ArmorSlot>().getItem();

            // Remove item from armor slot
            inventoryManager.TryRemoveItemFromArmorSlot(
                draggableItem.GetComponent<ArmorSlot>().getSlotArmorType().getArmorType()
            );

            // Set up dragged item
            draggableItemPrefab.SetActive(true);
            slotDraggableItem.SetItem(item, 1);
            isDraggableItemHaveAPrefab = true;

            // Play audio feedback
            audioManager.playUI(audioSource, "TakeItemFromInventory");
        }
        else
        {
            // Clear dragged item if already dragging or slot is empty
            TryDeleteItem();
        }
    }

    /// <summary>
    /// Attempts to place a dragged item into an armor slot.
    /// Validates item type compatibility and armor slot requirements.
    /// </summary>
    private void tryPutItemToArmorSlot()
    {
        // Get armor slot type and empty status
        CosmeticType cosmeticType = draggableItem
            .GetComponent<ArmorSlot>()
            .getSlotArmorType()
            .getArmorType();
        bool isEmpty = draggableItem.GetComponent<ArmorSlot>().getIsEmpty();

        // Get dragged item and validate it's a cosmetic item
        Item item = draggableItemPrefab.GetComponent<Slot>().getItem();
        if (item is CosmeticItem)
        {
            CosmeticItem cosmeticItem = (CosmeticItem)item;

            // Check if item type matches slot type and slot is empty
            if (cosmeticItem.getCosmeticType() == cosmeticType && isEmpty)
            {
                if (inventoryManager.TryPutItemToArmorSlot(item, cosmeticType))
                {
                    // Successfully equipped - play audio and clear dragged item
                    audioManager.playUI(audioSource, "PutItemInInventory");
                    TryDeleteItem();
                }
            }
        }
    }
    #endregion

    #region Slot Item Management
    /// <summary>
    /// Handles taking items from inventory or hotbar slots.
    /// Manages both single item and full stack removal based on input state.
    /// </summary>
    private void takeItemFromSlot()
    {
        List<Item> items = null;
        slotID = draggableItem.GetComponent<Slot>().getSlotID();

        // Get items from appropriate source (inventory or hotbar)
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

        // Handle item removal based on input mode
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
    /// Tries to move the dragged item into a non-empty slot. If same type and stackable, adds to stack.
    /// Handles partial stacking when max stack size is reached.
    /// </summary>
    /// <param name="slot">The target non-empty slot.</param>
    private void TryMoveItemToNotEmptySlot(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (!slot.getIsEmpty())
        {
            // Check if items can be stacked (same ID)
            if (slot.getItem().id == slotDraggableItem.getItem().id)
            {
                // Check if full stack can be added
                if (slot.getCount() + slotDraggableItem.getCount() <= slot.getItem().maxStack)
                {
                    // Add entire stack
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
                    // Add only partial stack due to max stack limit
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
    /// Updates the slot UI and clears the dragged item on success.
    /// </summary>
    /// <param name="slot">The target empty slot.</param>
    private void TryMoveItemToEmptySlot(Slot slot)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (slot.getIsEmpty())
        {
            // Attempt to add item to empty slot
            bool isSuccess = inventoryManager.AddItemToEmptySlot(
                slotDraggableItem.getItem(),
                slot.getRow(),
                slot.getColumn(),
                slotDraggableItem.getCount()
            );

            if (isSuccess)
            {
                // Successfully added - update UI and clear dragged item
                audioManager.playUI(audioSource, "PutItemInInventory");
                slot.setEmpty(false);
                slot.SetItem(slotDraggableItem.getItem(), slotDraggableItem.getCount());
                TryDeleteItem();
            }
        }
    }
    #endregion

    #region Item Removal Logic
    /// <summary>
    /// Attempts to pick one item from the list if already dragging an item, or initialize dragging if not.
    /// Handles both inventory and hotbar item removal with proper stacking logic.
    /// </summary>
    /// <param name="items">List of items in the clicked slot.</param>
    private void tryTakeOnItem(List<Item> items)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        if (items != null && isDraggableItemHaveAPrefab && items.Count > 0)
        {
            // Already dragging - try to add one more item if stackable
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
            // Start dragging item
            Item item = null;

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
                // Set up dragged item
                draggableItemPrefab.SetActive(true);
                slotDraggableItem.SetItem(item, 1);
                isDraggableItemHaveAPrefab = true;
                audioManager.playUI(audioSource, "TakeItemFromInventory");
            }
        }
        else
        {
            // Clear dragged item if no valid operation
            TryDeleteItem();
        }
    }

    /// <summary>
    /// Takes all items from a slot and begins dragging them.
    /// Removes items from source and sets up the dragged item representation.
    /// </summary>
    /// <param name="items">List of items in the clicked slot.</param>
    private void TryTakeAllItem(List<Item> items)
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();

        // Remove items from source
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
            // Set up dragged item with all items from source
            draggableItemPrefab.SetActive(true);
            slotDraggableItem.SetItem(items[0], items.Count);
            isDraggableItemHaveAPrefab = true;
            audioManager.playUI(audioSource, "TakeItemFromInventory");
        }
        else
        {
            // Clear dragged item if no items were taken
            TryDeleteItem();
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Deletes the currently dragged item and clears its data.
    /// Resets the dragging state and hides the dragged item prefab.
    /// </summary>
    private void TryDeleteItem()
    {
        slotDraggableItem = draggableItemPrefab.GetComponent<Slot>();
        draggableItem = null;
        draggableItemPrefab.SetActive(false);
        slotDraggableItem.ClearSlot();
        isDraggableItemHaveAPrefab = false;
    }
    #endregion
}
