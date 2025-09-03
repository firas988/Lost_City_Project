using UnityEngine;

/// <summary>
/// Triggers scene transition when the player enters the dungeon door area.
/// Checks if the player has the required quest before allowing dungeon access.
/// </summary>
public class DungeonDoorTrigger : MonoBehaviour
{
    #region Unity Event Methods

    /// <summary>
    /// Handles player entry into the dungeon door trigger area.
    /// Loads the dungeon scene if the player has the required quest.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            // Get the player component to check quest status
            Player player = other.GetComponent<StartPlayer>().getPlayer();

            // Check if the player has the required dungeon level quest
            if (player.getCurrentMainQuest() is DungeonLevel1)
            {
                // Load the dungeon scene (scene index 5)
                GameObject
                    .FindWithTag("GameManager")
                    .GetComponentInChildren<SceneHandler>()
                    .LoadScene(5);
            }
        }
    }

    #endregion
}
