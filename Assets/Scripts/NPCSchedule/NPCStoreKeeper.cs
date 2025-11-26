using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class NPCStoreKeeper : MonoBehaviour
{
    [Header("Objeto a Controlar (Luces)")]
    public GameObject objectToToggle;

    [Header("Configuración de Movimiento")]
    public Animator animator;
    public float speed = 2.5f;
    public float turnSpeed = 5f;

    [Header("Configuración de Visión")]
    public float viewAngle = 55f;
    public float viewDistance = 10f;
    public float proximitySense = 2.0f;
    public float catchDistance = 1.2f;

    [Tooltip("Selecciona AQUÍ las capas que son PAREDES u OBSTÁCULOS.")]
    public LayerMask obstacleMask = 1;

    [Tooltip("Altura de los ojos")]
    public float eyeHeight = 1.6f;

    [Header("Comportamiento")]
    public bool arrestOnlyAtNight = true;
    public NightManager nightManager;

    [Header("Referencias de Sistema")]
    public CameraManagerZ camManager;
    public CinemachineFreeLook arrestCamera;

    private GameObject player;
    private PlayerMovementLocker movementLocker;
    private bool isChasing = false;
    private bool isArresting = false;

    public LightsOutGrid lightsOutGrid;
    public float delayBeforeLeaving = 3f;
    public float pauseAtDestination = 5f;
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
    public GameObject TriggerDialogue;

    public bool HasFinishedLeaving => isHidden;
    public bool HasWaypoints => waypoints != null && waypoints.Count > 0;

    private IEnumerator Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();

        player = GameObject.FindWithTag("Player");
        movementLocker = FindAnyObjectByType<PlayerMovementLocker>();
        if (lightsOutGrid == null) lightsOutGrid = FindAnyObjectByType<LightsOutGrid>();
        timeManager = FindAnyObjectByType<LightingManager>();

        if (nightManager == null)
            nightManager = FindObjectOfType<NightManager>();

        yield return null;

        if (timeManager != null) timeManager.OnDayStart += ResetNPC;

        if (HasWaypoints)
        {
            originalWaypoints = new List<Transform>(waypoints);
            returnWaypoints = new List<Transform>(originalWaypoints);
            returnWaypoints.Reverse();
        }

        SetGhostMode(false);
        if (TriggerDialogue != null) TriggerDialogue.SetActive(true);
    }

    private void OnDestroy()
    {
        if (timeManager != null) timeManager.OnDayStart -= ResetNPC;
    }

    private void Update()
    {
        if (isArresting) return;

        if (isMoving && !isReturning)
        {
            CheckVisionAndChase();
        }
        else if (isChasing)
        {
            CheckVisionAndChase();
        }

        if (isChasing)
        {
            HandleChaseMovement();
            return;
        }

        if (isMoving && !isHidden && waypoints != null && waypoints.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void CheckVisionAndChase()
    {
        if (player == null) return;

        if (arrestOnlyAtNight && nightManager != null)
        {
            if (!nightManager.IsNight)
            {
                isChasing = false;
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        bool canSee = false;

        if (distanceToPlayer <= proximitySense)
        {
            canSee = true;
        }
        else if (distanceToPlayer <= viewDistance)
        {
            Vector3 npcEyes = transform.position + Vector3.up * eyeHeight;
            Vector3 playerTarget = player.transform.position + Vector3.up * (eyeHeight * 0.8f);

            Vector3 dirToPlayer = (playerTarget - npcEyes).normalized;
            Vector3 flatDir = dirToPlayer; flatDir.y = 0;
            Vector3 flatFwd = transform.forward; flatFwd.y = 0;

            if (Vector3.Angle(flatFwd, flatDir) <= viewAngle / 2f)
            {
                if (Physics.Raycast(npcEyes, dirToPlayer, out RaycastHit hit, distanceToPlayer, obstacleMask))
                {
                    canSee = false;
                    Debug.DrawLine(npcEyes, hit.point, Color.red);
                }
                else
                {
                    canSee = true;
                    Debug.DrawLine(npcEyes, playerTarget, Color.green);
                }
            }
        }

        if (canSee) isChasing = true;
        else if (distanceToPlayer > viewDistance * 1.5f) isChasing = false;
    }

    private void HandleChaseMovement()
    {
        if (player == null) return;

        Vector3 targetPos = player.transform.position;
        Vector3 myPos = transform.position;

        Vector3 movementTarget = new Vector3(targetPos.x, myPos.y, targetPos.z);
        transform.position = Vector3.MoveTowards(myPos, movementTarget, speed * Time.deltaTime);

        Vector3 direction = (movementTarget - myPos).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
        }

        if (animator) animator.SetBool("isWalking", true);
        if (TriggerDialogue != null) TriggerDialogue.SetActive(false);

        float dist2D = Vector3.Distance(new Vector3(myPos.x, 0, myPos.z), new Vector3(targetPos.x, 0, targetPos.z));

        if (dist2D < catchDistance) StartCoroutine(PerformArrestSequence());
    }

    private IEnumerator PerformArrestSequence()
    {
        isArresting = true;
        isChasing = false;

        if (animator) animator.SetBool("isWalking", false);
        if (movementLocker != null) movementLocker.LockMovement();

        if (camManager != null && arrestCamera != null)
        {
            camManager.SwitchCamera(arrestCamera);
            camManager.CambiarLookAt(transform);
        }

        yield return new WaitForSeconds(1.5f);

        if (JailManager.Instance != null) JailManager.Instance.SetMaxValue();
    }

    public void ActivateMovementAfterPuzzle()
    {
        if (isMoving || isHidden || isArresting) return;
        StartCoroutine(DelayedLeavingCoroutine());
    }

    private IEnumerator DelayedLeavingCoroutine()
    {
        if (animator) animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(delayBeforeLeaving);
        if (!HasWaypoints) yield break;

        isMoving = true;
        isReturning = false;
        currentPointIndex = 0;
        waypoints = originalWaypoints;

        if (TriggerDialogue != null) TriggerDialogue.SetActive(false);
        if (animator) animator.SetBool("isWalking", true);
    }

    private void MoveAlongPath()
    {
        if (currentPointIndex < 0 || currentPointIndex >= waypoints.Count) return;
        Transform target = waypoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, target.position) < 0.1f) currentPointIndex++;
        if (currentPointIndex >= waypoints.Count)
        {
            isMoving = false;
            if (animator) animator.SetBool("isWalking", false);
            if (!isReturning) StartCoroutine(PauseAndReturnCoroutine());
            else ResetNPC();
        }
    }

    private IEnumerator PauseAndReturnCoroutine()
    {
        yield return new WaitForSeconds(pauseAtDestination);

        if (objectToToggle != null) objectToToggle.SetActive(true);
        if (lightsOutGrid != null) lightsOutGrid.ResetPuzzleLogic();

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
        isChasing = false;
        isArresting = false;
        currentPointIndex = 0;
        transform.position = startPosition;
        transform.rotation = startRotation;
        if (animator) animator.SetBool("isWalking", false);
        SetGhostMode(false);
        if (TriggerDialogue != null) TriggerDialogue.SetActive(true);

        if (objectToToggle != null) objectToToggle.SetActive(true);
    }

    private void SetGhostMode(bool active)
    {
        isHidden = active;
        if (allRenderers != null) foreach (var r in allRenderers) if (r != null) r.enabled = !active;
        if (allColliders != null) foreach (var c in allColliders) if (c != null) c.enabled = !active;
        if (animator != null) animator.enabled = !active;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 eyeOrigin = transform.position + Vector3.up * eyeHeight;
        Vector3 forward = transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;
        Gizmos.DrawRay(eyeOrigin, leftDir * viewDistance);
        Gizmos.DrawRay(eyeOrigin, rightDir * viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(eyeOrigin, forward * viewDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
        Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, proximitySense);
    }
}