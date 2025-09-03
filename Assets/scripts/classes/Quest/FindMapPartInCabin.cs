using UnityEngine;

/// <summary>
/// Story quest that requires finding a map part in a cabin.
/// Manages hint visibility and door unlocking upon completion.
/// </summary>
[CreateAssetMenu(fileName = "FindMapPartInCabin", menuName = "Quests/FindMapPartInCabin")]
public class FindMapPartInCabin : StoryQuest
{
    #region Serialized Fields

    /// <summary>
    /// Static reference to the hint GameObject for this quest.
    /// </summary>
    [SerializeField]
    private static GameObject hint;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new FindMapPartInCabin quest.
    /// </summary>
    /// <param name="quest">The base quest to copy properties from.</param>
    public FindMapPartInCabin(Quest quest)
        : base(quest)
    {
        hint = null;
    }

    #endregion

    #region Hint Management

    /// <summary>
    /// Sets the hint GameObject for this quest.
    /// </summary>
    /// <param name="hint">The hint GameObject to set.</param>
    public void setHint(GameObject hint)
    {
        FindMapPartInCabin.hint = hint;
    }

    /// <summary>
    /// Gets the hint GameObject for this quest.
    /// </summary>
    /// <returns>The hint GameObject, or null if not set.</returns>
    public GameObject getHint()
    {
        return FindMapPartInCabin.hint;
    }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Completes the quest by unlocking the cabin door and showing the hint.
    /// </summary>
    public override void CompleteQuest()
    {
        // Find and unlock the cabin door
        GameObject door = GameObject.Find("Cabin").transform.Find("Door").gameObject;

        // Show the hint
        hint.SetActive(true);

        // Open the door by setting the animator parameter
        door.GetComponent<Animator>().SetBool("IsClosed", false);

        // Complete the base quest
        base.CompleteQuest();
    }

    #endregion
}
