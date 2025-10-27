using UnityEngine;

public class PatrollingPolice : MonoBehaviour
{
    [Header("Configuración de Patrulla")]
    [Tooltip("El primer punto de la patrulla.")]
    [SerializeField] private Transform patrolPointA;
    [Tooltip("El segundo punto de la patrulla.")]
    [SerializeField] private Transform patrolPointB;
    [Tooltip("La velocidad de movimiento del policía.")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Configuración de Visión")]
    [Tooltip("El ángulo total del cono de visión (en grados).")]
    [SerializeField] private float viewAngle = 90f;
    [Tooltip("La distancia máxima que el policía puede ver.")]
    [SerializeField] private float viewDistance = 10f;

    [Header("Configuración de Objetivos")]
    [Tooltip("La etiqueta (Tag) del objeto del jugador.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Las capas (Layers) que bloquearán la visión del policía (ej. paredes).")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Visuales (Opcional)")]
    [Tooltip("Un objeto (ej. un cono de luz o malla) que representa la visión.")]
    [SerializeField] private GameObject visionConeVisual;

    [SerializeField] private Animator animator;
    private string walkingBoolName = "IsWalking";

    private Transform player;
    private Transform currentTarget;
    private bool hasCaughtPlayer = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("PatrollingPolice: No se pudo encontrar al jugador con Tag: " + playerTag);
            this.enabled = false;
            return;
        }

        if (patrolPointA == null || patrolPointB == null)
        {
            Debug.LogError("PatrollingPolice: Faltan asignar los puntos de patrulla (A o B) en el inspector.");
            this.enabled = false;
            return;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        currentTarget = patrolPointA;
        if (visionConeVisual != null)
        {
            visionConeVisual.SetActive(true);
        }
    }

    private void Update()
    {
        if (hasCaughtPlayer || player == null)
        {
            if (hasCaughtPlayer && animator != null)
            {
                animator.SetBool(walkingBoolName, false);
            }
            return;
        }

        HandleVision();

        if (!hasCaughtPlayer)
        {
            HandlePatrol();
        }
    }
    private void HandlePatrol()
    {
        Vector3 posA = transform.position;
        Vector3 posB = currentTarget.position;
        posA.y = 0;
        posB.y = 0;

        if (Vector3.Distance(posA, posB) < 0.2f)
        {
            currentTarget = (currentTarget == patrolPointA) ? patrolPointB : patrolPointA;
        }

        Vector3 directionToTarget = (currentTarget.position - transform.position);
        directionToTarget.y = 0;
        directionToTarget.Normalize();

        if (directionToTarget != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

        transform.position += directionToTarget * moveSpeed * Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool(walkingBoolName, true);
        }
    }

    private void HandleVision()
    {
        Vector3 directionToPlayer = (player.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= viewDistance)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);

            if (angle <= viewAngle / 2f)
            {
                Vector3 eyePosition = transform.position + Vector3.up * 0.5f;

                if (!Physics.Raycast(eyePosition, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
                {
                    ArrestPlayer();
                }
            }
        }
    }

    private void ArrestPlayer()
    {
        hasCaughtPlayer = true;
        Debug.Log("¡Jugador atrapado por patrullero! ¡Estás arrestado!");

        if (animator != null)
        {
            animator.SetBool(walkingBoolName, false);
        }
        JailManager.Instance.SetMaxValue();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;

        Gizmos.DrawRay(transform.position, leftDir * viewDistance);
        Gizmos.DrawRay(transform.position, rightDir * viewDistance);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawWireArc(transform.position, Vector3.up, leftDir, viewAngle, viewDistance);
#endif
    }
}