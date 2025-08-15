using System;
using UnityEngine;

public class ColiderCutScene : MonoBehaviour
{
    private Action onTriggerEnter;

    public void subscribeToOnTriggerEnter(Action onTriggerEnter)
    {
        this.onTriggerEnter += onTriggerEnter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter?.Invoke();
        }
    }
}
