using UnityEngine;

public class PlayerHealthUi : MonoBehaviour
{
    private ProgressBar progressBar;
    private Player player;

    void Start()
    {
        progressBar = GetComponent<ProgressBar>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>().getPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<StartPlayer>()
                .getPlayer();
        }
        else
        {
            progressBar.SetProgress(player.getHealth() / player.getMaxHealth());
        }
    }
}
