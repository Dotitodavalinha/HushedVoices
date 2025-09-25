using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FolioAnimation : MonoBehaviour
{
    public Animator folioAnimator;
    public Button toggleButton;
    private bool isOpen;

    public void ToggleAnimationState()
    {
        if (isOpen)
        {
            folioAnimator.SetBool("isOpen", false);
        }
        else
        {
            folioAnimator.SetBool("isOpen", true);
        }

        isOpen = !isOpen;
    }
    public void OnUIReopened()
    {
        folioAnimator.SetBool("isOpen", false);
        isOpen = false;
        folioAnimator.Play("NormalState", 0, 0f);
    }

}
