using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lockpick : MonoBehaviour
{
    int[] order = { 1, 2, 3, 4, 5 };
    int currentOrder = 0;
    int wrongAttempt = 0;

    [Header("Waypoints")]
    [SerializeField] private Transform[] horizontalPositions;
    [SerializeField] private Transform verticalPosition;

    [SerializeField] private float stepSpeedHorizontal = 20f;
    [SerializeField] private float stepSpeedVertical = 8f;

    private int currentHIndex = 0;
    private bool isVertical = false;
    private Vector3 targetPosition;

    private bool isVerticalCycleActive = false;
    [SerializeField] private float autoLowerDelay = 1f;

    private Tumbler[] allTumblers;
    [SerializeField] private Text text;

    [Header("Game Control")]
    [SerializeField] private LockInteraction lockInteractionController;
    void Start()
    {
        allTumblers = FindObjectsOfType<Tumbler>();
        order = GenerateOrder();

        if (horizontalPositions.Length > 0)
        {
            targetPosition = horizontalPositions[0].position;
            transform.position = targetPosition;
        }
    }

    void Update()
    {
        float currentSpeed;
        if (Mathf.Abs(transform.position.y - targetPosition.y) > 0.001f)
        {
            currentSpeed = stepSpeedVertical;
        }
        else
        {
            currentSpeed = stepSpeedHorizontal;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            HandleInput();
        }

        if (currentOrder == 5)
        {
            //Debug.Log("Puzzle completado");
            if (lockInteractionController != null)
            {
                lockInteractionController.CompletePuzzleSequence();
            }

            currentOrder++;
        }
    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Bloquea todo el movimiento horizontal y vertical si un ciclo vertical está activo
        if (isVerticalCycleActive) return;

        // MOVIMIENTO HORIZONTAL
        if (horizontalInput != 0)
        {
            int nextIndex = currentHIndex + (int)horizontalInput;

            if (nextIndex >= 0 && nextIndex < horizontalPositions.Length)
            {
                currentHIndex = nextIndex;
                UpdateTargetPosition();
                return;
            }
        }

        // MOVIMIENTO VERTICAL
        else if (verticalInput > 0)
        {
            if (!isVertical)
            {
                isVertical = true;
                UpdateTargetPosition();
                StartCoroutine(VerticalLockpickCycle());
            }
        }
    }

    private void UpdateTargetPosition()
    {
        Vector3 basePos = horizontalPositions[currentHIndex].position;

        if (isVertical)
        {
            targetPosition = new Vector3(basePos.x, verticalPosition.position.y, basePos.z);
        }
        else
        {
            targetPosition = basePos;
        }
    }

    IEnumerator VerticalLockpickCycle()
    {
        isVerticalCycleActive = true;

        yield return new WaitUntil(() => transform.position == targetPosition);
        yield return new WaitForSeconds(autoLowerDelay);

        isVertical = false;
        UpdateTargetPosition();

        yield return new WaitUntil(() => transform.position == targetPosition);

        isVerticalCycleActive = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Tumbler hitTumbler = other.gameObject.GetComponent<Tumbler>();

        if (hitTumbler != null)
        {
            int tumblerNumber = hitTumbler.TumblerNumber;

            if (CheckTumbler(tumblerNumber, order, currentOrder))
            {
                hitTumbler.LockTumbler();
                currentOrder++;
                //Debug.Log("Correcto, Current Order es:" + currentOrder);
            }
            else
            {
                //Debug.Log("Incorrecto, se reinicia el puzzle.");
                wrongAttempt++;
                StartCoroutine(FailAndResetSequence());
            }
        }
    }

    IEnumerator FailAndResetSequence()
    {
        isVerticalCycleActive = true;

        currentOrder = 0;
        foreach (Tumbler tumbler in allTumblers)
        {
            tumbler.ResetTumbler();
        }

        if (isVertical)
        {
            isVertical = false;
            UpdateTargetPosition();
            yield return new WaitUntil(() => transform.position == targetPosition);
        }

        isVerticalCycleActive = false;
    }

    private void ResetAllTumblers()
    {
        currentOrder = 0;
        foreach (Tumbler tumbler in allTumblers)
        {
            tumbler.ResetTumbler();
        }

        currentHIndex = 0;
        isVertical = false;
        UpdateTargetPosition();
    }

    int[] GenerateOrder()
    {
        int[] order = { 1, 2, 3, 4, 5 };
        for (int i = 0; i < order.Length; i++)
        {
            int rnd = Random.Range(i, order.Length);
            int temp = order[rnd];
            order[rnd] = order[i];
            order[i] = temp;
        }
        return order;
    }

    bool CheckTumbler(int tumblerNumber, int[] order, int currentOrder)
    {
        if (currentOrder >= 0 && currentOrder < order.Length && tumblerNumber == order[currentOrder])
        {
            return true;
        }
        return false;
    }
}