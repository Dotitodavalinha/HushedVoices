using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCStoreKeeper : MonoBehaviour
{
    [Header("Configuración")]
    public Animator animator;
    public float speed = 2.5f;
    public float turnSpeed = 5f;

    [Header("Activación y Tiempos")]
    public LightsOutGrid lightsOutGrid;
    public float delayBeforeLeaving = 3f;
    public float pauseAtDestination = 5f;

    [Header("Ruta de Salida")]
    public List<Transform> waypoints;


    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool isReturning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private List<Transform> originalWaypoints;
    private List<Transform> returnWaypoints;

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private bool isHidden = false;

    private LightingManager timeManager;
    [Tooltip("El objeto (Trigger/Collider) que permite iniciar el diálogo. Se desactiva cuando el NPC empieza a caminar para evitar interacciones durante el movimiento.")]
    public GameObject TriggerDialogue;

    public bool HasFinishedLeaving => isHidden;
    public bool HasWaypoints => waypoints != null && waypoints.Count > 0;

    private IEnumerator Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();

        if (lightsOutGrid == null)
        {
            lightsOutGrid = FindAnyObjectByType<LightsOutGrid>();
        }

        timeManager = FindAnyObjectByType<LightingManager>();

        yield return null;


        if (timeManager != null)
        {
            timeManager.OnDayStart += ResetNPC;
        }

        if (HasWaypoints)
        {
            originalWaypoints = new List<Transform>(waypoints);
            returnWaypoints = new List<Transform>(originalWaypoints);
            returnWaypoints.Reverse();
        }

        SetGhostMode(false);
        if (TriggerDialogue != null)
            TriggerDialogue.SetActive(true);
    }

    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayStart -= ResetNPC;
        }
    }

    private void Update()
    {
        if (isMoving && !isHidden && waypoints != null && waypoints.Count > 0)
        {
            MoveAlongPath();
        }
    }

    public void ActivateMovementAfterPuzzle()
    {
        if (isMoving || isHidden) return;
        StartCoroutine(DelayedLeavingCoroutine());
    }

    private IEnumerator DelayedLeavingCoroutine()
    {
        if (animator) animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(delayBeforeLeaving);

        if (!HasWaypoints)
        {
            yield break;
        }

        isMoving = true;
        isReturning = false;
        currentPointIndex = 0;
        waypoints = originalWaypoints;

        if (TriggerDialogue != null)
            TriggerDialogue.SetActive(false);

        if (animator) animator.SetBool("isWalking", true);
    }

    private void MoveAlongPath()
    {
        if (currentPointIndex < 0 || currentPointIndex >= waypoints.Count)
            return;

        Transform target = waypoints[currentPointIndex];

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex++;
        }

        if (currentPointIndex >= waypoints.Count)
        {
            isMoving = false;
            if (animator) animator.SetBool("isWalking", false);

            if (!isReturning)
            {
                StartCoroutine(PauseAndReturnCoroutine());
            }
            else
            {
                ResetNPC();
            }
        }
    }

    private IEnumerator PauseAndReturnCoroutine()
    {
        yield return new WaitForSeconds(pauseAtDestination);

        isMoving = true;
        isReturning = true;
        waypoints = returnWaypoints;
        currentPointIndex = 0;

        if (animator) animator.SetBool("isWalking", true);
    }

    private void ResetNPC()
    {
        isMoving = false;
        isReturning = false;
        currentPointIndex = 0;

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (animator) animator.SetBool("isWalking", false);
        SetGhostMode(false);

        if (TriggerDialogue != null)
            TriggerDialogue.SetActive(true);
    }

    private void SetGhostMode(bool active)
    {
        isHidden = active;

        if (allRenderers != null)
        {
            foreach (var r in allRenderers)
                if (r != null) r.enabled = !active;
        }

        if (allColliders != null)
        {
            foreach (var c in allColliders)
                if (c != null) c.enabled = !active;
        }

        if (animator != null)
        {
            animator.enabled = !active;
        }
    }
}