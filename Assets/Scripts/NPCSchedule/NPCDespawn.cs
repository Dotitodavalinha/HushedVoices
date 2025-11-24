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

    [Header("Auto-bind de ruta")]
    [Tooltip("Debe matchear con el npcId usado en el schedule y en NpcWaypointPath")]
    public string npcId;

    [Tooltip("Si está en true y no hay waypoints asignados en el prefab, busca un NpcWaypointPath en la escena con el mismo npcId.")]
    public bool autoBindPathById = true;

    private int currentPointIndex = 0;
    private bool isLeaving = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private bool isHidden = false;

    private LightingManager timeManager;

    public GameObject TriggerDialogue;

    public bool HasFinishedLeaving => isHidden;
    public bool HasWaypoints => waypoints != null && waypoints.Count > 0;

    public void StartLeavingFromSchedule()
    {
        StartLeaving();
    }

    private IEnumerator Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();


        timeManager = FindAnyObjectByType<LightingManager>();

        yield return null;

        // En este punto la escena ya está montada, podemos buscar la ruta
        TryAutoBindWaypoints();

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
        if (isLeaving && !isHidden && waypoints != null && waypoints.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void TryAutoBindWaypoints()
    {
        if (!autoBindPathById) return;
        if (waypoints != null && waypoints.Count > 0) return;

        var paths = FindObjectsOfType<NpcWaypointPath>();
        foreach (var p in paths)
        {
            if (p != null && p.npcId == npcId)
            {
                waypoints = new List<Transform>(p.points);
                Debug.Log($"NPCDespawn: bound {waypoints.Count} waypoints for npcId='{npcId}'.");
                return;
            }
        }

        Debug.LogWarning($"NPCDespawn: no NpcWaypointPath found for npcId='{npcId}' in scene '{gameObject.scene.name}'.");
    }

    private void CheckTimeAndAct()
    {
        if (timeManager == null) return;

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
        if (isHidden || isLeaving) return;

        
        if (TriggerDialogue != null)
            TriggerDialogue.SetActive(false);

        if (waypoints == null || waypoints.Count == 0)
        {
            SetGhostMode(true);
            // también podemos destruir directamente si querés:
            Destroy(gameObject);
            return;
        }

        isLeaving = true;
        currentPointIndex = 0;

        if (animator) animator.SetBool("isWalking", true);
    }

    private void MoveAlongPath()
    {
        if (currentPointIndex < 0 || currentPointIndex >= waypoints.Count)
            return;

        Transform target = waypoints[currentPointIndex];
        if (target == null)
        {
            // si un waypoint se borró, forzamos final
            currentPointIndex = waypoints.Count;
        }
        else
        {
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
        }

        if (currentPointIndex >= waypoints.Count)
        {
            // LLEGÓ AL ÚLTIMO WAYPOINT:
            SetGhostMode(true);
            isLeaving = false;

            // NUEVO: destruimos el NPC al terminar la ruta
            Destroy(gameObject);
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
