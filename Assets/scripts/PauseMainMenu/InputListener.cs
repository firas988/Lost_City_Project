using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages all input handling for player movement, actions, and menu interactions.
/// Handles keybind management, input state tracking, and input validation.
/// Provides centralized input processing for movement, actions, and UI interactions.
/// </summary>
public class InputListener : MonoBehaviour
{
    #region Serialized Fields
    [Header("Keybind Configuration")]
    /// <summary>
    /// ScriptableObject containing the current keybind configuration.
    /// Stores all key mappings for player actions.
    /// </summary>
    [SerializeField]
    private KeybindList keybindList;

    /// <summary>
    /// ScriptableObject containing the default keybind configuration.
    /// Used for resetting keybinds to their original values.
    /// </summary>
    [SerializeField]
    private KeybindList keybindListDefault;

    /// <summary>
    /// List of GameObjects representing key display canvases.
    /// Used to show current keybind assignments in the UI.
    /// </summary>
    [SerializeField]
    private List<GameObject> keyCanvas;

    [Header("UI Text References")]
    /// <summary>
    /// Text component for displaying movement key tips.
    /// Shows WASD or equivalent movement keys to the player.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keyMovementTipText;

    /// <summary>
    /// Text component for displaying attack key tips.
    /// Shows the key used to toggle activate attack.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keyAttackTipText;

    /// <summary>
    /// Text component for displaying pause key tip.
    /// Shows the key used to pause the game.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keyPauseTipText;

    /// <summary>
    /// Text component for displaying map key tip.
    /// Shows the key used to open the full map.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keyMapTipText;

    /// <summary>
    /// Text component for displaying inventory key tip.
    /// Shows the key used to open the inventory.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keyInventoryTipText;

    /// <summary>
    /// Text component for displaying skill tree key tip.
    /// Shows the key used to open the skill tree.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI keySkillTreeTipText;
    #endregion

    #region Static Fields
    /// <summary>
    /// Static reference to movement key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keyMovementTipTextStatic;

    /// <summary>
    /// Static reference to attack key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keyAttackTipTextStatic;

    /// <summary>
    /// Static reference to pause key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keyPauseTipTextStatic;

    /// <summary>
    /// Static reference to map key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keyMapTipTextStatic;

    /// <summary>
    /// Static reference to inventory key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keyInventoryTipTextStatic;

    /// <summary>
    /// Static reference to skill tree key tip text for global access.
    /// </summary>
    private static TextMeshProUGUI keySkillTreeTipTextStatic;

    /// <summary>
    /// Static reference to the keybind list for global access.
    /// </summary>
    private static KeybindList keybindListStatic;
    #endregion

    #region Private Fields
    [Header("Input State Variables")]
    /// <summary>
    /// Dictionary mapping action names to their corresponding KeyCode values.
    /// Stores the current keybind configuration for runtime access.
    /// </summary>
    private static Dictionary<string, KeyCode> keybinds;

    /// <summary>
    /// Current horizontal input value (-1 to 1).
    /// Negative for left, positive for right movement.
    /// </summary>
    private float horizontal_input = 0f;

    /// <summary>
    /// Current vertical input value (-1 to 1).
    /// Negative for backward, positive for forward movement.
    /// </summary>
    private float vertical_input = 0f;

    /// <summary>
    /// Current jump input state.
    /// True when jump key is pressed, false otherwise.
    /// </summary>
    private bool jump_input = false;

    /// <summary>
    /// Current sprint input state.
    /// True when sprint key is held, false otherwise.
    /// </summary>
    private bool sprint_input = false;

    /// <summary>
    /// Current interact input state.
    /// True when interact key is held, false otherwise.
    /// </summary>
    private bool interact_input = false;

    [Header("Input Permission Flags")]
    /// <summary>
    /// Whether the player is currently allowed to move.
    /// Can be disabled during cutscenes or other game states.
    /// </summary>
    private bool canMove = true;

    /// <summary>
    /// Whether the player is currently allowed to attack.
    /// Can be disabled during certain game states.
    /// </summary>
    private bool canAttack = true;

    /// <summary>
    /// Whether the player is currently allowed to jump.
    /// Can be disabled during certain game states.
    /// </summary>
    private bool canJump = true;

    /// <summary>
    /// Whether the player is currently allowed to open menus.
    /// Can be disabled during cutscenes or other game states.
    /// </summary>
    private bool canOpenMenu = true;
    #endregion

    #region Input State Methods
    /// <summary>
    /// Gets whether the forward key is being pressed.
    /// </summary>
    /// <returns>True if forward key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingForward() - O(1)
    public bool isPressingForward()
    {
        return Input.GetKey(keybinds["Forward"]) && canMove;
    }

    /// <summary>
    /// Gets whether the backward key is being pressed.
    /// </summary>
    /// <returns>True if backward key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingBackward() - O(1)
    public bool isPressingBackward()
    {
        return Input.GetKey(keybinds["Backward"]) && canMove;
    }

    /// <summary>
    /// Gets whether the right key is being pressed.
    /// </summary>
    /// <returns>True if right key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingRight() - O(1)
    public bool isPressingRight()
    {
        return Input.GetKey(keybinds["Right"]) && canMove;
    }

    /// <summary>
    /// Gets whether the left key is being pressed.
    /// </summary>
    /// <returns>True if left key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingLeft() - O(1)
    public bool isPressingLeft()
    {
        return Input.GetKey(keybinds["Left"]) && canMove;
    }

    /// <summary>
    /// Gets the horizontal input value, respecting movement restrictions.
    /// </summary>
    /// <returns>The horizontal input value or 0 if movement is disabled.</returns>
    // COMPLEXITY ANALYSIS: horizontal() - O(1)
    public float horizontal()
    {
        return canMove ? horizontal_input : 0f;
    }

    /// <summary>
    /// Gets the vertical input value, respecting movement restrictions.
    /// </summary>
    /// <returns>The vertical input value or 0 if movement is disabled.</returns>
    // COMPLEXITY ANALYSIS: vertical() - O(1)
    public float vertical()
    {
        return canMove ? vertical_input : 0f;
    }

    /// <summary>
    /// Gets whether the jump input is active, respecting movement and jump restrictions.
    /// </summary>
    /// <returns>True if jump input is active and allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isJumping() - O(1)
    public bool isJumping()
    {
        return jump_input && canMove && canJump;
    }

    /// <summary>
    /// Gets whether the sprint input is active, respecting movement restrictions.
    /// </summary>
    /// <returns>True if sprint input is active and allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isSprinting() - O(1)
    public bool isSprinting()
    {
        return sprint_input && canMove;
    }

    /// <summary>
    /// Gets whether the interact input is active.
    /// </summary>
    /// <returns>True if interact input is active, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isInteracting() - O(1)
    public bool isInteracting()
    {
        return interact_input;
    }
    #endregion

    #region Menu Input Methods
    /// <summary>
    /// Gets whether the inventory key is pressed, respecting menu restrictions.
    /// </summary>
    /// <returns>True if inventory key is pressed and menus can be opened, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingInventory() - O(1)
    public bool isPressingInventory()
    {
        return Input.GetKeyDown(keybinds["Inventory"]) && canOpenMenu;
    }

    /// <summary>
    /// Gets whether the skill tree key is pressed, respecting menu restrictions.
    /// </summary>
    /// <returns>True if skill tree key is pressed and menus can be opened, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingSkillTree() - O(1)
    public bool isPressingSkillTree()
    {
        return Input.GetKeyDown(keybinds["SkillTree"]) && canOpenMenu;
    }

    /// <summary>
    /// Gets whether the full map key is pressed, respecting menu restrictions.
    /// </summary>
    /// <returns>True if full map key is pressed and menus can be opened, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingFullMap() - O(1)
    public bool isPressingFullMap()
    {
        return Input.GetKeyDown(keybinds["FullMap"]) && canOpenMenu;
    }

    /// <summary>
    /// Gets whether the pause key is pressed.
    /// </summary>
    /// <returns>True if pause key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingPause() - O(1)
    public bool isPressingPause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
    #endregion

    #region Action Input Methods
    /// <summary>
    /// Gets whether the P1 key is pressed.
    /// </summary>
    /// <returns>True if P1 key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingP1() - O(1)
    public bool isPressingP1()
    {
        return Input.GetKeyDown(keybinds["P1"]);
    }

    /// <summary>
    /// Gets whether the P2 key is pressed.
    /// </summary>
    /// <returns>True if P2 key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingP2() - O(1)
    public bool isPressingP2()
    {
        return Input.GetKeyDown(keybinds["P2"]);
    }

    /// <summary>
    /// Gets whether the P3 key is pressed.
    /// </summary>
    /// <returns>True if P3 key is pressed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isPressingP3() - O(1)
    public bool isPressingP3()
    {
        return Input.GetKeyDown(keybinds["P3"]);
    }

    /// <summary>
    /// Gets whether the attack input is active, respecting attack restrictions.
    /// </summary>
    /// <returns>True if attack input is active and allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isAttacking() - O(1)
    public bool isAttacking()
    {
        return Input.GetKeyDown(keybinds["Attack"]) && canAttack;
    }

    /// <summary>
    /// Gets whether the toggle activate attack input is active, respecting attack restrictions.
    /// </summary>
    /// <returns>True if toggle activate attack input is active and allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isToggleActivateAttack() - O(1)
    public bool isToggleActivateAttack()
    {
        return Input.GetKeyDown(keybinds["ToggleActivateAttack"]) && canAttack;
    }

    /// <summary>
    /// Gets whether the take one item input is active.
    /// </summary>
    /// <returns>True if take one item input is active, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: isTakingOneItem() - O(1)
    public bool isTakingOneItem()
    {
        return Input.GetKey(keybinds["TakeOneItem"]);
    }
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the input listener and sets up keybinds.
    /// Sets up static references and initializes the keybind system.
    /// </summary>
    // COMPLEXITY ANALYSIS: Awake() - O(k) where k = number of keybinds
    private void Awake()
    {
        // Store static reference to keybind list for global access
        keybindListStatic = keybindList;

        // Initialize keybinds dictionary if not already done
        if (keybinds == null)
        {
            keybinds = new Dictionary<string, KeyCode>();
            foreach (var keybind in keybindList.Keybinds)
            {
                keybinds.Add(keybind.Key, keybind.Keycode);
            }
        }

        // Store static references to UI text components
        keyMovementTipTextStatic = keyMovementTipText;
        keyAttackTipTextStatic = keyAttackTipText;
        keyPauseTipTextStatic = keyPauseTipText;
        keyMapTipTextStatic = keyMapTipText;
        keyInventoryTipTextStatic = keyInventoryTipText;
        keySkillTreeTipTextStatic = keySkillTreeTipText;

        // Initialize UI elements
        setKeyCanvas();
        setKeyTips();
    }

    /// <summary>
    /// Updates input states each frame and handles continuous input processing.
    /// Processes all input types and updates internal state variables.
    /// </summary>
    // COMPLEXITY ANALYSIS: Update() - O(1)
    void Update()
    {
        // Rotate the input listener object for visual effect
        transform.Rotate(0f, 90 * Time.unscaledDeltaTime, 0f);

        // Reset input values each frame
        horizontal_input = 0f;
        vertical_input = 0f;

        // Process movement input (WASD or equivalent)
        if (Input.GetKey(keybinds["Forward"]))
        {
            vertical_input += 1f; // Add forward movement
        }
        if (Input.GetKey(keybinds["Backward"]))
        {
            vertical_input -= 1f; // Add backward movement
        }
        if (Input.GetKey(keybinds["Right"]))
        {
            horizontal_input += 1f; // Add right movement
        }
        if (Input.GetKey(keybinds["Left"]))
        {
            horizontal_input -= 1f; // Add left movement
        }

        // Process jump input (key up/down for single press)
        if (Input.GetKeyUp(keybinds["Jump"]))
        {
            jump_input = false; // Reset jump input when key is released
        }
        if (Input.GetKeyDown(keybinds["Jump"]))
        {
            jump_input = true; // Set jump input when key is pressed
        }

        // Process sprint input (key up/down for toggle behavior)
        if (Input.GetKeyUp(keybinds["Sprint"]))
        {
            sprint_input = false; // Reset sprint input when key is released
        }
        if (Input.GetKeyDown(keybinds["Sprint"]))
        {
            sprint_input = true; // Set sprint input when key is pressed
        }

        // Process interact input (continuous while key is held)
        if (Input.GetKey(keybinds["Interact"]))
        {
            interact_input = true; // Set interact input while key is held
        }
        else
        {
            interact_input = false; // Reset interact input when key is released
        }
    }

    /// <summary>
    /// Resets input states when the component is enabled.
    /// Ensures clean input state when re-enabling the component.
    /// </summary>
    // COMPLEXITY ANALYSIS: OnEnable() - O(1)
    void OnEnable()
    {
        // Reset all input states to prevent stuck inputs
        jump_input = false;
        sprint_input = false;
        interact_input = false;
    }

    /// <summary>
    /// Resets input states when the component is disabled.
    /// Ensures clean input state when disabling the component.
    /// </summary>
    // COMPLEXITY ANALYSIS: OnDisable() - O(1)
    void OnDisable()
    {
        // Reset all input states to prevent stuck inputs
        jump_input = false;
        sprint_input = false;
        interact_input = false;
        horizontal_input = 0f;
        vertical_input = 0f;
    }
    #endregion

    #region State Management Methods
    /// <summary>
    /// Sets whether the player can move.
    /// </summary>
    /// <param name="canMove">Whether movement is allowed.</param>
    // COMPLEXITY ANALYSIS: setCanMove() - O(1)
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    /// <summary>
    /// Gets whether the player can move.
    /// </summary>
    /// <returns>True if movement is allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getCanMove() - O(1)
    public bool getCanMove()
    {
        return canMove;
    }

    /// <summary>
    /// Sets whether the player can attack.
    /// </summary>
    /// <param name="canAttack">Whether attacking is allowed.</param>
    // COMPLEXITY ANALYSIS: setCanAttack() - O(1)
    public void setCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    /// <summary>
    /// Gets whether the player can attack.
    /// </summary>
    /// <returns>True if attacking is allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getCanAttack() - O(1)
    public bool getCanAttack()
    {
        return canAttack;
    }

    /// <summary>
    /// Sets whether the player can jump.
    /// </summary>
    /// <param name="canJump">Whether jumping is allowed.</param>
    // COMPLEXITY ANALYSIS: setCanJump() - O(1)
    public void setCanJump(bool canJump)
    {
        this.canJump = canJump;
    }

    /// <summary>
    /// Gets whether the player can jump.
    /// </summary>
    /// <returns>True if jumping is allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getCanJump() - O(1)
    public bool getCanJump()
    {
        return canJump;
    }

    /// <summary>
    /// Sets whether the player can open menus.
    /// </summary>
    /// <param name="canOpenMenu">Whether menu opening is allowed.</param>
    // COMPLEXITY ANALYSIS: setCanOpenMenu() - O(1)
    public void setCanOpenMenu(bool canOpenMenu)
    {
        this.canOpenMenu = canOpenMenu;
    }

    /// <summary>
    /// Gets whether the player can open menus.
    /// </summary>
    /// <returns>True if menu opening is allowed, false otherwise.</returns>
    // COMPLEXITY ANALYSIS: getCanOpenMenu() - O(1)
    public bool getCanOpenMenu()
    {
        return canOpenMenu;
    }
    #endregion

    #region Keybind Management Methods
    /// <summary>
    /// Sets a keybind for a specific action, checking for conflicts.
    /// </summary>
    /// <param name="key">The action name to bind.</param>
    /// <param name="keycode">The key code to bind to the action.</param>
    /// <returns>True if the keybind was set successfully, false if there was a conflict.</returns>
    // COMPLEXITY ANALYSIS: setKeybind() - O(k) where k = number of keybinds
    public static bool setKeybind(string key, KeyCode keycode)
    {
        // Check for conflicts with existing keybinds
        foreach (string keybind in keybinds.Keys)
        {
            if (keybind != key && keybinds[keybind] == keycode)
            {
                return false; // Conflict detected
            }
        }

        // Set the new keybind
        keybinds[key] = keycode;

        // Update the keybind list and UI
        updateKeybinds(key, keycode);
        setKeyTips();

        return true; // Successfully set
    }

    /// <summary>
    /// Updates the keybind list with a new key code for a specific action.
    /// </summary>
    /// <param name="key">The action name to update.</param>
    /// <param name="keycode">The new key code for the action.</param>
    // COMPLEXITY ANALYSIS: updateKeybinds() - O(k) where k = number of keybinds
    public static void updateKeybinds(string key, KeyCode keycode)
    {
        // Find and update the corresponding keybind in the list
        foreach (Keybind keybind in keybindListStatic.Keybinds)
        {
            if (keybind.Key == key)
            {
                keybind.SetKeycode(keycode);
                break;
            }
        }
    }

    /// <summary>
    /// Gets the key code bound to a specific action.
    /// </summary>
    /// <param name="key">The action name to get the key code for.</param>
    /// <returns>The key code bound to the action, or KeyCode.None if not found.</returns>
    // COMPLEXITY ANALYSIS: getKeybind() - O(1)
    public KeyCode getKeybind(string key)
    {
        if (keybinds.ContainsKey(key))
        {
            return keybinds[key];
        }
        else
        {
            return KeyCode.None;
        }
    }

    /// <summary>
    /// Resets all keybinds to their default values.
    /// Restores the original keybind configuration.
    /// </summary>
    // COMPLEXITY ANALYSIS: resetAllKeybinds() - O(k) where k = number of keybinds
    public void resetAllKeybinds()
    {
        // Reset all keybinds to their default values
        foreach (Keybind keybind in keybindListDefault.Keybinds)
        {
            keybinds[keybind.Key] = keybind.Keycode;
            updateKeybinds(keybind.Key, keybind.Keycode);
        }
        setKeyCanvas();
        setKeyTips(); // Update the UI to reflect changes
    }

    /// <summary>
    /// Sets the key canvas text to display the current keybinds.
    /// Updates UI elements to show current key assignments.
    /// </summary>
    // COMPLEXITY ANALYSIS: setKeyCanvas() - O(c) where c = number of key canvases
    private void setKeyCanvas()
    {
        foreach (GameObject keyCanvas in keyCanvas)
        {
            try
            {
                // Find the corresponding keybind and update the canvas text
                keyCanvas.GetComponent<TextMeshPro>().text = keybindList
                    .Keybinds.Find(x => x.Key == keyCanvas.name)
                    .Keycode.ToString();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error setting key canvas: " + e.Message);
                // Silently handle any errors in key canvas setup
            }
        }
    }

    /// <summary>
    /// Updates all key tip text components with current keybind information.
    /// Refreshes the UI to show current key assignments.
    /// </summary>
    // COMPLEXITY ANALYSIS: setKeyTips() - O(k) where k = number of keybinds
    private static void setKeyTips()
    {
        try
        {
            // Build movement key tip string (WASD format)
            string keyMovementTip =
                keybindListStatic.Keybinds.Find(x => x.Key == "Forward").Keycode.ToString()
                + ","
                + keybindListStatic.Keybinds.Find(x => x.Key == "Left").Keycode.ToString()
                + ","
                + keybindListStatic.Keybinds.Find(x => x.Key == "Backward").Keycode.ToString()
                + ","
                + keybindListStatic.Keybinds.Find(x => x.Key == "Right").Keycode.ToString();
            keyMovementTipTextStatic.text = keyMovementTip;

            // Update attack key tip
            string keyAttackTip = keybindListStatic
                .Keybinds.Find(x => x.Key == "ToggleActivateAttack")
                .Keycode.ToString();
            keyAttackTipTextStatic.text = keyAttackTip;

            // Update pause key tip
            string keyPauseTip = keybindListStatic
                .Keybinds.Find(x => x.Key == "Pause")
                .Keycode.ToString();
            keyPauseTipTextStatic.text = keyPauseTip;

            // Update map key tip
            string keyMapTip = keybindListStatic
                .Keybinds.Find(x => x.Key == "FullMap")
                .Keycode.ToString();
            keyMapTipTextStatic.text = keyMapTip;

            // Update inventory key tip
            string keyInventoryTip = keybindListStatic
                .Keybinds.Find(x => x.Key == "Inventory")
                .Keycode.ToString();
            keyInventoryTipTextStatic.text = keyInventoryTip;

            // Update skill tree key tip
            string keySkillTreeTip = keybindListStatic
                .Keybinds.Find(x => x.Key == "SkillTree")
                .Keycode.ToString();
            keySkillTreeTipTextStatic.text = keySkillTreeTip;
        }
        catch (System.Exception e)
        {
            // Silently handle any errors in key tip setup
        }
    }
    #endregion
}
