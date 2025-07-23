using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;


public class TrunOnFires : MonoBehaviour
{
    //private Transform[] braziers;
    private List<Transform> fires;

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

        List<Transform> sortedFires = fires.OrderBy(fire => int.Parse(fire.name)).ToList();

        StartCoroutine(playFires(sortedFires));


    }

    void Update()
    {
        
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
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

}
