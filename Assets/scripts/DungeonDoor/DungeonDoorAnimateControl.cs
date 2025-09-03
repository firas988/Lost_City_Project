using UnityEngine;

/// <summary>
/// Controls the animation of dungeon doors, allowing individual or simultaneous door opening.
/// Manages left and right door animators for split-door dungeon entrances.
/// </summary>
public class DungeonDoorAnimateControl : MonoBehaviour
{
    #region Private Fields

    /// <summary>
    /// Animator component for the left door.
    /// </summary>
    private Animator animatorLeft;

    /// <summary>
    /// Animator component for the right door.
    /// </summary>
    private Animator animatorRight;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the door animators by finding them in the hierarchy.
    /// </summary>
    void Start()
    {
        // Find and assign the left door animator
        animatorLeft = transform.Find("door").transform.Find("door_left").GetComponent<Animator>();

        // Find and assign the right door animator
        animatorRight = transform
            .Find("door")
            .transform.Find("door_right")
            .GetComponent<Animator>();
    }

    #endregion

    #region Door Control Methods

    /// <summary>
    /// Opens the left door by setting its animator parameter.
    /// </summary>
    public void openLeftDoor()
    {
        animatorLeft.SetBool("Open", true);
    }

    /// <summary>
    /// Opens the right door by setting its animator parameter.
    /// </summary>
    public void openRightDoor()
    {
        animatorRight.SetBool("Open", true);
    }

    /// <summary>
    /// Opens both doors simultaneously by setting their animator parameters.
    /// </summary>
    public void openBothDoors()
    {
        animatorLeft.SetBool("Open", true);
        animatorRight.SetBool("Open", true);
    }

    #endregion
}
