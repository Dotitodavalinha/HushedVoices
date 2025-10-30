using UnityEngine;
using System.Collections;

public class StationaryPolice : MonoBehaviour
{
    [Header("Configuración de Visión")]
    [Tooltip("El ángulo total del cono de visión (en grados).")]
    [SerializeField] private float viewAngle = 90f;
    [Tooltip("La distancia máxima que el policía puede ver.")]
    [SerializeField] private float viewDistance = 10f;

    [Header("Configuración del Temporizador")]
    [Tooltip("Segundos que el policía estará vigilando activamente.")]
    [SerializeField] private float timeOn = 4f;
    [Tooltip("Segundos que el policía estará 'descansando' (sin vigilar).")]
    [SerializeField] private float timeOff = 3f;

    [Header("Configuración de Objetivos")]
    [Tooltip("La etiqueta (Tag) del objeto del jugador.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Las capas (Layers) que bloquearán la visión del policía (ej. paredes).")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Visuales (Opcional)")]
    [Tooltip("Un objeto (ej. un cono de luz o malla) que se activa/desactiva con la visión.")]
    [SerializeField] private GameObject visionConeVisual;

    private Transform player;
    private float currentStateTimer;
    private bool isWatching;
    private bool hasCaughtPlayer = false;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se pudo encontrar al jugador. Asegúrate de que el jugador tenga el Tag: " + playerTag);
            this.enabled = false;
            return;
        }

        isWatching = true;
        currentStateTimer = timeOn;
        if (visionConeVisual != null)
        {
            visionConeVisual.SetActive(true);
        }
    }

    private void Update()
    {
        if (hasCaughtPlayer || player == null)
        {
            return;
        }

        HandleTimer();

        if (isWatching)
        {
            HandleVision();
        }
    }

    private void HandleTimer()
    {
        currentStateTimer -= Time.deltaTime;
        if (currentStateTimer <= 0)
        {
            isWatching = !isWatching;

            if (isWatching)
            {
                currentStateTimer = timeOn;

                animator.SetBool("isSleeping", false);

                if (visionConeVisual != null)
                {
                    visionConeVisual.SetActive(true);
                }
            }
            else
            {
                currentStateTimer = timeOff;

                animator.SetBool("isSleeping", true);

                if (visionConeVisual != null)
                {
                    visionConeVisual.SetActive(false);
                }
            }
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
        isWatching = false;
        if (visionConeVisual != null)
        {
            visionConeVisual.SetActive(false);
        }

        Debug.Log("¡Jugador atrapado! ¡Estás arrestado!");

         JailManager.Instance.SetMaxValue();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isWatching ? Color.yellow : Color.gray;
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
        }

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