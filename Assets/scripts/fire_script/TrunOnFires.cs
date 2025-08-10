using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class TrunOnFires : MonoBehaviour
{
    private List<Transform> fires;
    private List<Transform> sortedFires;
    [SerializeField]
    private PlayableDirector playableDirector;

    void Start()
    {
        fires = new List<Transform>();
        Transform[] braziers = transform.GetComponentsInChildren<Transform>();
        foreach(Transform brazier in braziers)
        {
            if(int.TryParse(brazier.name, out int result))
            {
                fires.Add(brazier);
            }
        }

        sortedFires = fires.OrderBy(fire => int.Parse(fire.name)).ToList();


    }

    public void TurnOnFires()
    {
        playableDirector.Pause();
        StartCoroutine(playFires(sortedFires));
    }

    private IEnumerator playFires(List<Transform> sortedFires)
    {
        if (sortedFires != null)
        {
            for (int i=0;i<sortedFires.Count;i+=2)
            {
                ParticleSystem fireParticleSystem1 = sortedFires[i].GetComponentInChildren<ParticleSystem>();
                ParticleSystem fireParticleSystem2 = sortedFires[i+1].GetComponentInChildren<ParticleSystem>();

                if (fireParticleSystem1 != null && fireParticleSystem2 != null)
                {
                    fireParticleSystem1.Play();
                    fireParticleSystem2.Play();

                }
                yield return new WaitForSeconds(0.4f);
            }
        }
        playableDirector.Resume();
    }

}
