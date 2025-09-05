using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an NPC that can engage in dialogue with the player.
/// Extends the base NPC class to add conversation capabilities.
/// </summary>
public class TalkativeNpc : NPC
{
    #region Private Fields

    /// <summary>
    /// Dictionary containing all available dialogues for this NPC.
    /// </summary>
    protected Dictionary<string, Dialogue> dialogues;

    #endregion

    #region Public Fields

    /// <summary>
    /// The starting dialogue key for conversations with this NPC.
    /// </summary>
    public string start;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new TalkativeNpc with specified parameters.
    /// </summary>
    /// <param name="id">Unique identifier for the NPC.</param>
    /// <param name="name">Display name of the NPC.</param>
    /// <param name="type">Type/category of the NPC.</param>
    /// <param name="walkRadius">Radius within which the NPC can walk.</param>
    /// <param name="areaMask">Navigation area mask for the NPC.</param>
    /// <param name="waitTimeRange">Range of wait times between actions.</param>
    /// <param name="navMeshAreaName">Name of the navigation mesh area.</param>
    /// <param name="speed">Current movement speed.</param>
    /// <param name="maxSpeed">Maximum movement speed.</param>
    /// <param name="dialogues">Dictionary of available dialogues.</param>
    /// <param name="start">Starting dialogue key.</param>
    public TalkativeNpc(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float speed,
        float maxSpeed,
        Dictionary<string, Dialogue> dialogues,
        string start
    )
        : base(
            id,
            name,
            type,
            walkRadius,
            areaMask,
            waitTimeRange,
            navMeshAreaName,
            speed,
            maxSpeed
        )
    {
        // COMPLEXITY ANALYSIS: TalkativeNpc() - O(1)
        this.dialogues = dialogues;
        this.start = start;
    }

    #endregion

    #region Dialogue Management

    /// <summary>
    /// Responds to a dialogue interaction by providing the dialogue text and options.
    /// </summary>
    /// <param name="dialogue">The key identifying the dialogue to respond to.</param>
    /// <param name="options">The array of response options for the dialogue.</param>
    /// <param name="endDialogue">Whether the dialogue has ended.</param>
    /// <returns>The text content of the dialogue response.</returns>
    public string respodToDialogue(string dialogue, out string[] options, out bool endDialogue)
    {
        // COMPLEXITY ANALYSIS: respodToDialogue() - O(1)
        try
        {
            if (!this.dialogues.ContainsKey(dialogue))
            {
                options = null;
                endDialogue = true;
                return null;
            }
            else
            {
                options = this.dialogues[dialogue].GetOptions();
                endDialogue = false;
            }
            return this.dialogues[dialogue].GetText();
        }
        catch (System.Exception)
        {
            options = null;
            endDialogue = true;
            return null;
        }
    }

    #endregion

    #region Getters and Setters

    /// <summary>
    /// Gets all available dialogues for this NPC.
    /// </summary>
    /// <returns>Dictionary containing all dialogues.</returns>
    public Dictionary<string, Dialogue> getDialogues()
    {
        // COMPLEXITY ANALYSIS: getDialogues() - O(1)
        return this.dialogues;
    }

    /// <summary>
    /// Sets the dialogues for this NPC.
    /// </summary>
    /// <param name="dialogues">New dictionary of dialogues.</param>
    public void setDialogue(Dictionary<string, Dialogue> dialogues)
    {
        // COMPLEXITY ANALYSIS: setDialogue() - O(1)
        this.dialogues = dialogues;
    }

    #endregion
}
