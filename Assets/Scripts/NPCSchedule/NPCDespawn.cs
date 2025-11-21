using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDespawn : MonoBehaviour
{
    [Header("Configuración")]
    public Animator animator;
    public float speed = 2.5f;
    public float turnSpeed = 5f;

    [Header("Ruta de Salida")]
    public List<Transform> waypoints;

    private int currentPointIndex = 0;
    private bool isLeaving = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private bool isHidden = false;

    private LightingManager timeManager;

    private IEnumerator Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();

        timeManager = FindAnyObjectByType<LightingManager>();

        yield return null;

        if (timeManager != null)
        {
            timeManager.OnNightStart += StartLeaving;
            timeManager.OnDayStart += ResetNPC;

            CheckTimeAndAct();
        }
    }

    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnNightStart -= StartLeaving;
            timeManager.OnDayStart -= ResetNPC;
        }
    }

    private void Update()
    {
        if (isLeaving && !isHidden && waypoints.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void CheckTimeAndAct()
    {
        float h = timeManager.TimeOfDay;

        if (h >= 20f || h < 5f)
        {
            SetGhostMode(true);
        }
        else
        {
            SetGhostMode(false);
        }
    }

    private void StartLeaving()
    {
        if (isHidden) return;

        if (waypoints.Count == 0)
        {
            SetGhostMode(true);
            return;
        }

        isLeaving = true;
        currentPointIndex = 0;

        if (animator) animator.SetBool("isWalking", true);
    }

    private void MoveAlongPath()
    {
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
            if (currentPointIndex >= waypoints.Count)
            {
                SetGhostMode(true);
                isLeaving = false;
            }
        }
    }

    private void ResetNPC()
    {
        isLeaving = false;
        currentPointIndex = 0;

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (animator) animator.SetBool("isWalking", false);
        SetGhostMode(false);
    }

    private void SetGhostMode(bool active)
    {
        isHidden = active;

        foreach (var r in allRenderers) r.enabled = !active;
        foreach (var c in allColliders) c.enabled = !active;

        if (active && animator) animator.enabled = false;
        else if (!active && animator) animator.enabled = true;
    }
}