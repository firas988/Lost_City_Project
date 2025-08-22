using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneHandlerDone : MonoBehaviour
{
    private PlayableDirector director;

    private StartPlayer startPlayer;

    void Start()
    {
        if (director == null)
            director = GetComponentInChildren<PlayableDirector>();

        director.stopped += OnTimelineStopped;

        startPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<StartPlayer>();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 1)
        {
            gameObject.SetActive(!startPlayer.getIsCutScenePart1Completed());
        }
        else if (sceneIndex == 2)
        {
            gameObject.SetActive(!startPlayer.getIsCutScenePart2Completed());
        }
    }

    void OnTimelineStopped(PlayableDirector obj)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 1)
        {
            startPlayer.setIsCutScenePart1Completed(true);
        }
        else if (sceneIndex == 2)
        {
            startPlayer.setIsCutScenePart2Completed(true);
        }
    }
}
