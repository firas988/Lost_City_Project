using System;
using UnityEngine;

public class CastelDoorHandler : MonoBehaviour
{
    private Animator animator;

    private Action onTriggerEnter;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update() { }

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

    public void openTheDoor()
    {
        animator.SetTrigger("open");
    }
}
