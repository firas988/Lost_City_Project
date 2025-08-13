using UnityEngine;

public class BossBarHandler : MonoBehaviour
{
    private ProgressBar progressBar;

    void Start()
    {
        progressBar = GetComponentInChildren<ProgressBar>();
    }

    public void TakeDamage(float health)
    {
        progressBar.SetProgress(health);
    }

    public void hideBar()
    {
        this.gameObject.SetActive(false);
    }

    public void showBar()
    {
        this.gameObject.SetActive(true);
    }
}
