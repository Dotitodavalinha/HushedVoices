using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionesPantallas : MonoBehaviour
{
    [SerializeField] Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            animator.SetTrigger("Tecla");
        }
    }
}
