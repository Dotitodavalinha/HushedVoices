using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCutscene : MonoBehaviour
{
    private Animator animator;
    public GameObject inJail;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
            return;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (!inJail.activeSelf)
        {
            animator.Play("prisonCutscene", -1, 0f);
            
        }
    }
}
