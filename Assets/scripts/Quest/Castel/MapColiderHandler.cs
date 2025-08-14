using System;
using UnityEngine;

public class MapColiderHandler : MonoBehaviour
{
    private Action onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter?.Invoke();
        }
    }

    public void subscribeToOnTriggerEnter(Action onTriggerEnter)
    {
        this.onTriggerEnter += onTriggerEnter;
    }
}
