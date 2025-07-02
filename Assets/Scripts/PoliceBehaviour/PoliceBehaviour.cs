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
    [SerializeField] private bool isWaiting = false;

    public Animator animator;

    [SerializeField] private bool patrolDuringDay = false;

    [SerializeField] private NightManager nightManager;
    [SerializeField] private JailManager jailManager;

    private bool isChasingPlayer = false;
    private GameObject player;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        nightManager = FindObjectOfType<NightManager>();
        jailManager = FindObjectOfType<JailManager>();

        player = GameObject.FindWithTag("Player");

    }

    private void Update()
    {
        if (!patrolDuringDay && !nightManager.IsNight)
        {
            animator.SetBool("IsWaiting", true);
            return;
        }

        if (patrolPoints.Count < 2 || isWaiting)
        {
            animator.SetBool("IsWaiting", true);
            return;
        }

        if (DialogueTrigger.playerInRange && !nightManager.IsNight)
        {
            animator.SetBool("IsWaiting", true);
            return;
        }


        animator.SetBool("IsWaiting", false);
        if (isChasingPlayer)
        {
            Vector3 toPlayer = (player.transform.position - transform.position);
            float distanceToPlayer = toPlayer.magnitude;
            float angle = Vector3.Angle(transform.forward, toPlayer.normalized);

            if (distanceToPlayer <= 5f && angle <= 22.5f)
            {
                animator.SetBool("IsWaiting", false);
                toPlayer.Normalize();
                transform.position += toPlayer * moveSpeed * Time.deltaTime;
                transform.forward = toPlayer;

                if (DialogueTrigger.playerInRange)
                {
                    jailManager.SetMaxValue();
                    Debug.Log("¡Jugador atrapado! Enviado a la cárcel.");
                }
            }
            else
            {
                // Lo perdió de vista
                isChasingPlayer = false;
                currentIndex = 0;
            }

            return;
        }

        else
        {
            // Patrullaje normal
            Transform target = patrolPoints[currentIndex];
            Vector3 direction = (target.position - transform.position);
            if (direction.magnitude > 0.01f)
            {
                direction.Normalize();
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.forward = direction;
            }

            if (Vector3.Distance(transform.position, target.position) < 0.2f)
            {
                StartCoroutine(WaitAndMove());
            }

            // Detección de visión
            if (nightManager.IsNight && player != null)
            {
                Vector3 toPlayer = (player.transform.position - transform.position);
                float distanceToPlayer = toPlayer.magnitude;
                float angle = Vector3.Angle(transform.forward, toPlayer.normalized);

                if (distanceToPlayer <= 5f && angle <= 22.5f)
                {
                    Debug.Log("viendo al jugador");
                    isChasingPlayer = true;
                }
            }
        }



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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Dirección frontal del policía
        Vector3 forward = transform.forward;
        float viewDistance = 5f;
        float viewAngle = 45f;

        // Ángulo izquierdo
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Gizmos.DrawRay(transform.position, leftDir * viewDistance);

        // Ángulo derecho
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;
        Gizmos.DrawRay(transform.position, rightDir * viewDistance);

        // Línea central
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, forward * viewDistance);
    }

}
