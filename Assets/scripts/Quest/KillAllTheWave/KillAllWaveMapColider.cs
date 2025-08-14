using System;
using UnityEngine;

public class KillAllWaveMapColider : MonoBehaviour
{
    private Action onEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnter?.Invoke();
        }
    }

    public void unsubscribeToOnEnter(Action onEnter)
    {
        this.onEnter -= onEnter;
    }

    public void subscribeToOnEnter(Action onEnter)
    {
        this.onEnter += onEnter;
    }
}
