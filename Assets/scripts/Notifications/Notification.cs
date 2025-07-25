using System;
using TMPro;
using UniRx;
using UnityEngine;

public class Notification : MonoBehaviour
{   [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textSubtitle;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = this.GetComponent<Animator>();

    }


    public void SetTitle(string title) => textTitle.text = title;

    public void SetSubtitle(string subtitle) => textSubtitle.text = subtitle;

    public void Show()
    {
        animator.SetBool("isVisible", true);
    }

    public void Hide()
    {
        animator.SetBool("isVisible", false);
    }

    // Update is called once per frame
    void Update() { }
}
