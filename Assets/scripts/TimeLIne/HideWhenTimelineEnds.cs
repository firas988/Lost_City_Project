using UnityEngine;
using UnityEngine.Playables;

public class HideWhenTimelineEnds : MonoBehaviour
{
    private PlayableDirector director;

    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        director.stopped += OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector obj)
    {
        gameObject.SetActive(false);
    }
}
