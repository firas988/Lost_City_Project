using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    [SerializeField]
    private List<float> potions;

    [SerializeField]
    private List<float> rotation;

    [SerializeField]
    private int sceneIndex;

    public PlayerData(StartPlayer startPlayer)
    {
        this.potions = new List<float>();
        this.rotation = new List<float>();

        this.potions.Add(startPlayer.gameObject.transform.position.x);
        this.potions.Add(startPlayer.gameObject.transform.position.y);
        this.potions.Add(startPlayer.gameObject.transform.position.z);

        this.rotation.Add(startPlayer.gameObject.transform.rotation.x);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.y);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.z);
        this.rotation.Add(startPlayer.gameObject.transform.rotation.w);

        this.sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public List<float> Potions => potions;
    public List<float> Rotation => rotation;
    public int SceneIndex => sceneIndex;
}
