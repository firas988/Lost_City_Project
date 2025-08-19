using UnityEngine;

public class RebertRoomCutSceneHandler : MonoBehaviour
{
    private SceneHandler sceneHandler;

    private string gameManagerTag = "GameManager";

    void Start()
    {
        sceneHandler = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<SceneHandler>();
    }

    public void loadScene()
    {
        sceneHandler.LoadScene(2);
    }
}
