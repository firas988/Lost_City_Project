using System.Collections;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;

    [Header("Keys Configuration")]
    private KeyCode forwardKey = KeyCode.W;
    private KeyCode backwardKey = KeyCode.S;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode leftKey = KeyCode.A;
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode interactKey = KeyCode.E;
    private KeyCode attackKey = KeyCode.Mouse0;
    private KeyCode toggleActivateAttackKey = KeyCode.LeftControl;
    private KeyCode takeOneItemKey = KeyCode.LeftAlt;
    private KeyCode pauseKey = KeyCode.Escape;
    private KeyCode inventoryKey = KeyCode.M;
    private KeyCode skillTreeKey = KeyCode.N;

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
        return Input.GetKey(forwardKey);
    }

    public bool isPressingBackward()
    {
        return Input.GetKey(backwardKey);
    }

    public bool isPressingRight()
    {
        return Input.GetKey(rightKey);
    }

    public bool isPressingLeft()
    {
        return Input.GetKey(leftKey);
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
    }

    void Update()
    {
        horizontal_input = 0f;
        vertical_input = 0f;

        if (Input.GetKey(forwardKey))
        {
            vertical_input += 1f;
        }
        if (Input.GetKey(backwardKey))
        {
            vertical_input -= 1f;
        }
        if (Input.GetKey(rightKey))
        {
            horizontal_input += 1f;
        }
        if (Input.GetKey(leftKey))
        {
            horizontal_input -= 1f;
        }

        if (Input.GetKeyUp(jumpKey))
        {
            jump_input = false;
        }
        if (Input.GetKeyDown(jumpKey))
        {
            jump_input = true;
        }
        if (Input.GetKeyUp(sprintKey))
        {
            sprint_input = false;
        }
        if (Input.GetKeyDown(sprintKey))
        {
            sprint_input = true;
        }

        if (Input.GetKey(interactKey))
        {
            interact_input = true;
        }
        else
        {
            interact_input = false;
        }

        if (Input.GetKeyDown(inventoryKey))
        {
            uiManager.toggleInventory();
        }
        if (Input.GetKeyDown(skillTreeKey))
        {
            uiManager.toggleSkillTreeMenu();
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
        return Input.GetKeyDown(attackKey) && canAttack;
    }

    public bool isToggleActivateAttack()
    {
        return Input.GetKeyDown(toggleActivateAttackKey) && canAttack;
    }

    public bool isTakingOneItem()
    {
        return Input.GetKey(takeOneItemKey);
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
