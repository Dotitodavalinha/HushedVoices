using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        animator.Play("ChangePage", -1, 0f);
    }

    public void OnAnimationEnd()
    {
        Debug.Log("Animación terminada, desactivando objeto");

        gameObject.SetActive(false);
    }

    public void StopAndHide()
    {
        animator.Rebind();
        gameObject.SetActive(false);
    }
}
