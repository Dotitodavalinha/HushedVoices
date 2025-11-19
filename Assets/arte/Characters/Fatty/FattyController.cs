using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FattyController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float destroyAfterSeconds = 0.5f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Alarmed", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Alarmed", false);
        }
    }
    /*
    private IEnumerator Transition()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        animator.SetBool("InTransition", false);
        bool current = animator.GetBool("Alarmed");
        animator.SetBool("Alarmed", !current);
    }
    */
}
