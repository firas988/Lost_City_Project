using UnityEngine;

/// <summary>
/// Represents a single dialogue exchange containing text and response options.
/// Used to structure conversation data for NPC interactions.
/// </summary>
public class Dialogue
{
    /// <summary>
    /// The main dialogue text to be displayed to the player.
    /// </summary>
    private string text;

    /// <summary>
    /// Array of response options available to the player for this dialogue.
    /// </summary>
    private string[] options;

    /// <summary>
    /// Initializes a new dialogue with specified text and response options.
    /// </summary>
    /// <param name="text">The dialogue text to display to the player.</param>
    /// <param name="options">Array of response options for the player to choose from.</param>
    public Dialogue(string text, string[] options)
    {
        // COMPLEXITY ANALYSIS: Dialogue() - O(1)
        // Store the dialogue text
        this.text = text;
        // Store the response options
        this.options = options;
    }

    /// <summary>
    /// Gets the dialogue text content.
    /// </summary>
    /// <returns>The text content of this dialogue.</returns>
    public string GetText()
    {
        // COMPLEXITY ANALYSIS: GetText() - O(1)
        return text;
    }

    /// <summary>
    /// Gets the array of response options for this dialogue.
    /// </summary>
    /// <returns>Array of strings representing the available response options.</returns>
    public string[] GetOptions()
    {
        // COMPLEXITY ANALYSIS: GetOptions() - O(1)
        return options;
    }
}
