using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private DialogueTrigger DialogueTrigger;

    private int currentIndex = 0;
    private bool goingForward = true;
    private bool isWaiting = false;

    private void Update()
    {
        if (patrolPoints.Count < 2 || isWaiting || DialogueTrigger.playerInRange) return;

        Transform target = patrolPoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.forward = direction;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            StartCoroutine(WaitAndMove());
    }


    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        if (goingForward)
        {
            currentIndex++;
            if (currentIndex >= patrolPoints.Count)
            {
                currentIndex = patrolPoints.Count - 2;
                goingForward = false;
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 1;
                goingForward = true;
            }
        }

        isWaiting = false;
    }
}
