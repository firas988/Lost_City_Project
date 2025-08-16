using UnityEngine;

public class IgnoreFreeze : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            GetComponent<Animator>().Update(Time.unscaledDeltaTime);
        }
    }
}
