using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private KeybindList keybindList;

    [Header("Keys Configuration")]
    private Dictionary<string, KeyCode> keybinds;
    private float horizontal_input = 0f;
    private float vertical_input = 0f;
    private bool jump_input = false;
    private bool sprint_input = false;
    private bool interact_input = false;

    private bool canMove = true;
    private bool canAttack = true;
    private bool canJump = true;

    public bool isPressingForward()
    {
        return Input.GetKey(keybinds["Forward"]);
    }

    public bool isPressingBackward()
    {
        return Input.GetKey(keybinds["Backward"]);
    }

    public bool isPressingRight()
    {
        return Input.GetKey(keybinds["Right"]);
    }

    public bool isPressingLeft()
    {
        return Input.GetKey(keybinds["Left"]);
    }

    public float horizontal()
    {
        return canMove ? horizontal_input : 0f;
    }

    public float vertical()
    {
        return canMove ? vertical_input : 0f;
    }

    public bool isJumping()
    {
        return jump_input && canMove && canJump;
    }

    public bool isSprinting()
    {
        return sprint_input && canMove;
    }

    public bool isInteracting()
    {
        return interact_input;
    }

    private void Awake()
    {
        uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        keybinds = new Dictionary<string, KeyCode>();
        foreach (var keybind in keybindList.Keybinds)
        {
            keybinds.Add(keybind.Key, keybind.Keycode);
        }
    }

    void Update()
    {
        horizontal_input = 0f;
        vertical_input = 0f;

        if (Input.GetKey(keybinds["Forward"]))
        {
            vertical_input += 1f;
        }
        if (Input.GetKey(keybinds["Backward"]))
        {
            vertical_input -= 1f;
        }
        if (Input.GetKey(keybinds["Right"]))
        {
            horizontal_input += 1f;
        }
        if (Input.GetKey(keybinds["Left"]))
        {
            horizontal_input -= 1f;
        }

        if (Input.GetKeyUp(keybinds["Jump"]))
        {
            jump_input = false;
        }
        if (Input.GetKeyDown(keybinds["Jump"]))
        {
            jump_input = true;
        }
        if (Input.GetKeyUp(keybinds["Sprint"]))
        {
            sprint_input = false;
        }
        if (Input.GetKeyDown(keybinds["Sprint"]))
        {
            sprint_input = true;
        }

        if (Input.GetKey(keybinds["Interact"]))
        {
            interact_input = true;
        }
        else
        {
            interact_input = false;
        }

        if (Input.GetKeyDown(keybinds["Inventory"]))
        {
            uiManager.toggleInventory();
        }
        if (Input.GetKeyDown(keybinds["SkillTree"]))
        {
            uiManager.toggleSkillTreeMenu();
        }
        if (Input.GetKeyDown(keybinds["FullMap"]))
        {
            uiManager.toggleFullMapMenu();
        }
    }

    void OnEnable()
    {
        jump_input = false;
        sprint_input = false;
        interact_input = false;
    }

    public bool isAttacking()
    {
        return Input.GetKeyDown(keybinds["Attack"]) && canAttack;
    }

    public bool isToggleActivateAttack()
    {
        return Input.GetKeyDown(keybinds["ToggleActivateAttack"]) && canAttack;
    }

    public bool isTakingOneItem()
    {
        return Input.GetKey(keybinds["TakeOneItem"]);
    }

    void OnDisable()
    {
        jump_input = false;
        sprint_input = false;
        interact_input = false;
        horizontal_input = 0f;
        vertical_input = 0f;
    }

    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public bool getCanMove()
    {
        return canMove;
    }

    public void setCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    public bool getCanAttack()
    {
        return canAttack;
    }

    public void setCanJump(bool canJump)
    {
        this.canJump = canJump;
    }

    public bool getCanJump()
    {
        return canJump;
    }
}
