using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    [SerializeField]
    private List<float> position;

    [SerializeField]
    private List<float> rotation;

    [SerializeField]
    private int sceneIndex;

    [SerializeField]
    private bool isCutScenePart1Completed;

    [SerializeField]
    private bool isCutScenePart2Completed;

    public PlayerData(StartPlayer startPlayer)
    {
        this.position = new List<float>();
        this.rotation = new List<float>();

        this.position.Add(startPlayer.gameObject.transform.position.x);
        this.position.Add(startPlayer.gameObject.transform.position.y);
        this.position.Add(startPlayer.gameObject.transform.position.z);

        this.rotation.Add(startPlayer.gameObject.transform.rotation.x);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.y);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.z);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.w);

        this.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        this.isCutScenePart1Completed = startPlayer.getIsCutScenePart1Completed();
        this.isCutScenePart2Completed = startPlayer.getIsCutScenePart2Completed();
    }

    public List<float> Position => position;
    public List<float> Rotation => rotation;
    public int SceneIndex => sceneIndex;
    public bool IsCutScenePart1Completed => isCutScenePart1Completed;
    public bool IsCutScenePart2Completed => isCutScenePart2Completed;
}
