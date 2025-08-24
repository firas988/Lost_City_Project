using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPlayer : MonoBehaviour
{
    private Player player;

    private string spawnPointTag = "Respawn";

    private bool isCutScenePart1Completed = false;
    private bool isCutScenePart2Completed = false;

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
            }
            catch (System.Exception)
            {
                Debug.Log("No spawn point found");
            }
        }
        else
        {
            gameObject.GetComponent<CharacterController>().enabled = false;

            gameObject.transform.position = new Vector3(
                playerData.Position[0],
                playerData.Position[1],
                playerData.Position[2]
            );
            gameObject.transform.rotation = new Quaternion(
                playerData.Rotation[0],
                playerData.Rotation[1],
                playerData.Rotation[2],
                playerData.Rotation[3]
            );

            gameObject.GetComponent<CharacterController>().enabled = true;
        }

        this.isCutScenePart1Completed =
            playerData != null ? playerData.IsCutScenePart1Completed : false;
        this.isCutScenePart2Completed =
            playerData != null ? playerData.IsCutScenePart2Completed : false;

        if (SceneManager.GetActiveScene().buildIndex == 1 && !isCutScenePart1Completed)
        {
            playerScript.setIsInCutscene(true);
        }

        if (SceneManager.GetActiveScene().buildIndex == 2 && !isCutScenePart2Completed)
        {
            playerScript.setIsInCutscene(true);
        }
    }

    public void setIsCutScenePart1Completed(bool isCutScenePart1Completed)
    {
        this.isCutScenePart1Completed = isCutScenePart1Completed;
    }

    public void setIsCutScenePart2Completed(bool isCutScenePart2Completed)
    {
        this.isCutScenePart2Completed = isCutScenePart2Completed;
    }

    public bool getIsCutScenePart1Completed()
    {
        return isCutScenePart1Completed;
    }

    public bool getIsCutScenePart2Completed()
    {
        return isCutScenePart2Completed;
    }
}
