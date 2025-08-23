using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HideWhenTimelineEnds : MonoBehaviour
{
    private PlayableDirector director;

    [SerializeField]
    private List<GameObject> objectsToHide;

    private UIManager uiManager;

    private string GameManagerTag = "GameManager";

    void Start()
    {
        if (director == null)
            director = GetComponentInChildren<PlayableDirector>();

        director.stopped += OnTimelineStopped;

        uiManager = GameObject
            .FindGameObjectWithTag(GameManagerTag)
            .transform.parent.GetComponentInChildren<UIManager>();
    }

    private void Update()
    {
        if (uiManager == null)
        {
            uiManager = GameObject
                .FindGameObjectWithTag(GameManagerTag)
                .transform.parent.GetComponentInChildren<UIManager>();
            return;
        }
        if (uiManager.isMenuOpen())
        {
            objectsToHide.ForEach(obj => obj.SetActive(false));
        }
        else
        {
            objectsToHide.ForEach(obj => obj.SetActive(true));
        }
    }

    void OnTimelineStopped(PlayableDirector obj)
    {
        gameObject.SetActive(false);
    }
}
