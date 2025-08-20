using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPlayer : MonoBehaviour
{
    private Player player;

    private string spawnPointTag = "Respawn";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = new Player();
    }

    public Player getPlayer()
    {
        return player;
    }

    public void loadPlayer(PlayerData playerData)
    {
        if (playerData == null || playerData.SceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            try
            {
                gameObject.transform.position = GameObject
                    .FindWithTag(spawnPointTag)
                    .transform.position;
                gameObject.transform.rotation = GameObject
                    .FindWithTag(spawnPointTag)
                    .transform.rotation;
                return;
            }
            catch (System.Exception)
            {
                Debug.Log("No spawn point found");
                return;
            }
        }

        gameObject.transform.position = new Vector3(
            playerData.Potions[0],
            playerData.Potions[1],
            playerData.Potions[2]
        );
        gameObject.transform.rotation = new Quaternion(
            playerData.Rotation[0],
            playerData.Rotation[1],
            playerData.Rotation[2],
            playerData.Rotation[3]
        );
    }
}
