using UnityEngine;

public class StartPlayer : MonoBehaviour
{
    private Player player;

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
