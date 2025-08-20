using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DissolvingController : MonoBehaviour
{
    private Material[] skinnesMaterials;

    [SerializeField]
    private float dissolveRate = 0.0125f;

    [SerializeField]
    private float refreshRate = 0.025f;

    private AudioSource audioSource;

    private AudioManager audioManager;

    /// <summary>Tag for the GameManager object.</summary>
    private string gameManagerTag = "GameManager";

    void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        var mats = new System.Collections.Generic.List<Material>();
        foreach (Renderer rend in renderers)
        {
            mats.AddRange(rend.materials);
        }

        skinnesMaterials = mats.ToArray();

        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject
            .FindGameObjectWithTag(gameManagerTag)
            .GetComponentInChildren<AudioManager>();
    }

    public void StartDissolve()
    {
        StartCoroutine(Dissolve());
    }

    IEnumerator Dissolve()
    {
        if (skinnesMaterials.Length > 0)
        {
            audioManager.playSFX(audioSource, "Dissolving");
            float counter = 0f;
            while (counter < 1f)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnesMaterials.Length; i++)
                {
                    skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }

        Destroy(gameObject);
    }

    public void StartDeDissolve()
    {
        StartCoroutine(DeDissolve());
    }

    public void setDissolveAmount(float dissolveAmount = 1f)
    {
        if (skinnesMaterials == null)
        {
            return;
        }
        for (int i = 0; i < skinnesMaterials.Length; i++)
        {
            if (skinnesMaterials[i] != null)
            {
                skinnesMaterials[i].SetFloat("_DissolveAmount", dissolveAmount);
            }
        }
    }

    IEnumerator DeDissolve()
    {
        if (skinnesMaterials.Length > 0)
        {
            float counter = 1f;
            for (int i = 0; i < skinnesMaterials.Length; i++)
            {
                skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
            }

            while (counter > 0f)
            {
                counter -= dissolveRate;
                for (int i = 0; i < skinnesMaterials.Length; i++)
                {
                    skinnesMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
