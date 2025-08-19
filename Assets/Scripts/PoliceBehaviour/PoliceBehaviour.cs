using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private DialogueTrigger DialogueTrigger;

    [SerializeField] private bool patrolDuringDay = false;
    [SerializeField] private NightManager nightManager;
    

    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private float viewDistance = 5f;

    private int currentIndex = 0;
    private bool goingForward = true;
    [SerializeField] private bool isWaiting = false;

    private bool isChasingPlayer = false;
    private bool canChase = false;
    private bool hasCaughtPlayer = false;


    private GameObject player;
    private Animator animator;
    private bool wasNightLastFrame = false;



    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        nightManager = FindObjectOfType<NightManager>();

        player = GameObject.FindWithTag("Player");

       
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null) return;
        }

        if (!patrolDuringDay && !nightManager.IsNight)
        {
            animator.SetBool("IsWaiting", true);

            // Ir hacia el waypoint 0 si no está ahí
            Transform target = patrolPoints[0];
            Vector3 toStart = target.position - transform.position;

            if (toStart.magnitude > 0.1f)
            {
                Vector3 dir = toStart.normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                transform.forward = dir;
            }

            canChase = false;
            wasNightLastFrame = false;
            return;
        }

        // Si acaba de hacerse de noche, esperar 2 segundos antes de permitir persecución
        if (!wasNightLastFrame && nightManager.IsNight)
        {
            canChase = false;
            Invoke(nameof(EnableChase), 2f);
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

        Vector3 toPlayer = (player.transform.position - transform.position);
        float distanceToPlayer = toPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);

        bool seesPlayer = false;

        // chequeo de ángulo, distancia y raycast
        if (canChase && nightManager.IsNight && player != null &&
            distanceToPlayer <= viewDistance && angle <= viewAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, toPlayer.normalized, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    seesPlayer = true;
                }
            }
        }

        if (seesPlayer)
        {
            isChasingPlayer = true;
        }

        if (isChasingPlayer)
        {
            if (seesPlayer)
            {
                toPlayer.Normalize();
                transform.position += toPlayer * moveSpeed * Time.deltaTime;
                transform.forward = toPlayer;

                if (!hasCaughtPlayer && DialogueTrigger.playerInRange)
                {
                    hasCaughtPlayer = true;
                    JailManager.Instance.SetMaxValue();
                    Debug.Log("¡Jugador atrapado! Enviado a la cárcel.");
                }
            }
            else
            {
                // Lo perdió de vista, vuelve al patrullaje
                isChasingPlayer = false;
                currentIndex = 0;
            }

            return;
        }

        // Patrullaje normal
        Transform patrolTarget = patrolPoints[currentIndex];
        Vector3 patrolDir = (patrolTarget.position - transform.position);
        if (patrolDir.magnitude > 0.01f)
        {
            patrolDir.Normalize();
            transform.position += patrolDir * moveSpeed * Time.deltaTime;
            transform.forward = patrolDir;
        }

        if (Vector3.Distance(transform.position, patrolTarget.position) < 0.2f)
        {
            StartCoroutine(WaitAndMove());
        }

        wasNightLastFrame = nightManager.IsNight;
    }



    private void EnableChase()
    {
        canChase = true;
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

        Vector3 forward = transform.forward;

        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Gizmos.DrawRay(transform.position, leftDir * viewDistance);

        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;
        Gizmos.DrawRay(transform.position, rightDir * viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, forward * viewDistance);
    }

    

}
