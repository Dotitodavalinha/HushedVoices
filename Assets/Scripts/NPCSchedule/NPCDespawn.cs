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

    [Header("Control de diálogo")]
    [SerializeField] private NPCDialogue npcDialogue;
    [SerializeField] private DialogueTrigger dialogueTrigger;

    private int currentPointIndex = 0;
    private bool isLeaving = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private bool isHidden = false;

    public bool HasFinishedLeaving => isHidden;
    public bool HasWaypoints => waypoints != null && waypoints.Count > 0;
    public bool IsLeaving => isLeaving;

    // Llamado desde el ScheduleManager cuando el bloque ya no corresponde
    public void StartLeavingFromSchedule()
    {
        StartLeaving();
    }

    private void Awake()
    {
        if (npcDialogue == null)
            npcDialogue = GetComponent<NPCDialogue>();

        if (dialogueTrigger == null)
            dialogueTrigger = GetComponentInChildren<DialogueTrigger>();
    }

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();

        // Solo para binding de la ruta, nada de horarios ni curfew acá
        TryAutoBindWaypoints();
    }

    private void Update()
    {
        if (isLeaving && !isHidden && HasWaypoints)
        {
            MoveAlongPath();
        }
    }

    private void TryAutoBindWaypoints()
    {
        if (!autoBindPathById) return;
        if (waypoints != null && waypoints.Count > 0) return;
        if (string.IsNullOrEmpty(npcId)) return;

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

    private void StartLeaving()
    {
        if (isHidden || isLeaving) return;

        if (!HasWaypoints)
        {
            // Sin ruta definida: desaparece en el lugar
            SetGhostMode(true);
            return;
        }

        isLeaving = true;
        currentPointIndex = 0;

        // Cortamos diálogo mientras se va
        if (npcDialogue != null)
            npcDialogue.enabled = false;

        if (dialogueTrigger != null)
            dialogueTrigger.enabled = false;

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
                // Llega al último waypoint -> queda en ghost mode
                // El ScheduleManager se encarga de destruirlo cuando refresque.
                SetGhostMode(true);
                isLeaving = false;
            }
        }
    }

    // Esto ya casi no lo va a llamar nadie, pero lo dejo por si en algún momento querés resetear desde otro lado
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
