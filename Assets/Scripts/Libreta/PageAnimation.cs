using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageAnimation : MonoBehaviour
{
    private Animator animator;
    private bool pageChanged = false;

    private TabsLogic tabsLogic;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (tabsLogic == null)
            tabsLogic = FindObjectOfType<TabsLogic>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("ChangePage") && state.normalizedTime >= 0.5f && !pageChanged)
        {
            pageChanged = true;
            tabsLogic?.ActivateNextSection();
        }

        if (state.normalizedTime >= 1f)
        {
            pageChanged = false;
            gameObject.SetActive(false);
        }
    }

    public void PlayAnimation()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        animator.Play("ChangePage", -1, 0f);
        pageChanged = false;
    }

   
}
