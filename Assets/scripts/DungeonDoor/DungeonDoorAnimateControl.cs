using UnityEngine;

public class DungeonDoorAnimateControl : MonoBehaviour
{
    [SerializeField]
    private Animator animatorLeft;

    [SerializeField]
    private Animator animatorRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatorLeft = transform.Find("door").transform.Find("door_left").GetComponent<Animator>();
        animatorRight = transform
            .Find("door")
            .transform.Find("door_right")
            .GetComponent<Animator>();
    }

    public void openLeftDoor()
    {
        animatorLeft.SetBool("Open", true);
    }

    public void openRightDoor()
    {
        animatorRight.SetBool("Open", true);
    }

    public void openBothDoors()
    {
        animatorLeft.SetBool("Open", true);
        animatorRight.SetBool("Open", true);
    }
}
