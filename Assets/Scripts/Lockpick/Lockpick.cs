using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lockpick : MonoBehaviour
{
    int[] order = { 1, 2, 3, 4, 5 };
    int currentOrder = 0;

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

    private Coroutine verticalCycleCoroutine;

    private bool isWaitingForSecondPress = false;
    private Tumbler pendingTumbler = null;

    private bool justMovedHorizontally = true;

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
            if (lockInteractionController != null)
            {
                lockInteractionController.CompletePuzzleSequence();
            }
            currentOrder++;
        }
    }

    private void HandleInput()
    {
        if (isWaitingForSecondPress && Input.GetKeyDown(KeyCode.W) && !isVerticalCycleActive)
        {
            isWaitingForSecondPress = false;
            justMovedHorizontally = false;

            if (verticalCycleCoroutine != null)
            {
                StopCoroutine(verticalCycleCoroutine);
                verticalCycleCoroutine = null;
            }
            isVerticalCycleActive = false;

            if (pendingTumbler != null)
            {
                if (CheckTumbler(pendingTumbler.TumblerNumber, order, currentOrder))
                {
                    StartCoroutine(LockAndReturnSequence(pendingTumbler));
                    currentOrder++;
                }
                else
                {
                    FailSequence();
                }
                pendingTumbler = null;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && !isVerticalCycleActive)
        {
            MoveHorizontal(1);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.A) && !isVerticalCycleActive)
        {
            MoveHorizontal(-1);
            return;
        }

        if (justMovedHorizontally && !isVertical && !isVerticalCycleActive && !isWaitingForSecondPress)
        {
            Tumbler tumblerToTanto = GetCurrentTumbler();
            if (tumblerToTanto != null && tumblerToTanto.IsLocked)
            {
                justMovedHorizontally = false;
                return;
            }

            justMovedHorizontally = false;
            isVertical = true;
            UpdateTargetPosition();
            verticalCycleCoroutine = StartCoroutine(VerticalLockpickCycle());
            return;
        }
    }

    IEnumerator LockAndReturnSequence(Tumbler tumblerToLock)
    {
        isVerticalCycleActive = true;

        isVertical = true;
        UpdateTargetPosition();
        yield return new WaitUntil(() => transform.position == targetPosition);

        tumblerToLock.LockTumbler();

        yield return new WaitForSeconds(0.2f);

        isVertical = false;
        UpdateTargetPosition();
        yield return new WaitUntil(() => transform.position == targetPosition);

        isVerticalCycleActive = false;
        justMovedHorizontally = true;
    }

    private void MoveHorizontal(int direction)
    {
        if (verticalCycleCoroutine != null)
        {
            StopCoroutine(verticalCycleCoroutine);
            verticalCycleCoroutine = null;
            isVerticalCycleActive = false;
            isVertical = false;
        }

        isWaitingForSecondPress = false;
        pendingTumbler = null;

        int nextIndex = currentHIndex + direction;
        if (nextIndex >= 0 && nextIndex < horizontalPositions.Length)
        {
            if (currentHIndex != nextIndex)
            {
                justMovedHorizontally = true;
            }
            currentHIndex = nextIndex;
            UpdateTargetPosition();
        }
    }

    private void FailSequence()
    {
        isVerticalCycleActive = false;
        if (verticalCycleCoroutine != null) StopCoroutine(verticalCycleCoroutine);
        verticalCycleCoroutine = null;

        justMovedHorizontally = false;
        currentOrder = 0;

        foreach (Tumbler tumbler in allTumblers)
        {
            tumbler.ResetTumbler();
        }

        isWaitingForSecondPress = false;
        pendingTumbler = null;

        if (isVertical)
        {
            isVertical = false;
            UpdateTargetPosition();
            transform.position = targetPosition;
        }

        justMovedHorizontally = true;
    }

    IEnumerator FailAndResetSequence()
    {
        isVerticalCycleActive = true;
        justMovedHorizontally = false;
        currentOrder = 0;

        foreach (Tumbler tumbler in allTumblers)
        {
            tumbler.ResetTumbler();
        }

        isWaitingForSecondPress = false;
        pendingTumbler = null;

        if (isVertical)
        {
            isVertical = false;
            UpdateTargetPosition();
            yield return new WaitUntil(() => transform.position == targetPosition);
        }

        isVerticalCycleActive = false;
        verticalCycleCoroutine = null;
        justMovedHorizontally = true;
    }

    private Tumbler GetCurrentTumbler()
    {
        int targetTumblerNumber = currentHIndex + 1;
        foreach (Tumbler t in allTumblers)
        {
            if (t.TumblerNumber == targetTumblerNumber)
            {
                return t;
            }
        }
        return null;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Tumbler hitTumbler = other.gameObject.GetComponent<Tumbler>();
        if (hitTumbler == null || hitTumbler.IsLocked) return;
        if (!isVertical) return;
        if (isWaitingForSecondPress) return;

        pendingTumbler = hitTumbler;
        isWaitingForSecondPress = true;

        bool correct = CheckTumbler(hitTumbler.TumblerNumber, order, currentOrder);

        if (correct)
        {
            // Tiembla como pista
            StartCoroutine(hitTumbler.CorrectTumbler());
        }

        SoundManager.instance.PlaySound(correct ? SoundID.click : SoundID.clack);
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
        verticalCycleCoroutine = null;
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
        justMovedHorizontally = true;
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