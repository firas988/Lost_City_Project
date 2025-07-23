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
}
