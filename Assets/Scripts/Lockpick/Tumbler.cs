using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tumbler : MonoBehaviour
{
    private Vector3 initialPosition;

    [SerializeField]
    private int tumblerNumber;

    [SerializeField]
    private Vector3 lockedOffset = new Vector3(0, 1.0f, 0.0f);

    public int TumblerNumber => tumblerNumber;

    private bool isLocked = false;
    public bool IsLocked => isLocked;

    void Start()
    {
        initialPosition = transform.position;
        isLocked = false;
    }

    public void LockTumbler()
    {
        transform.position = initialPosition + lockedOffset;
        isLocked = true;
    }

    public void ResetTumbler()
    {
        transform.position = initialPosition;
        isLocked = false;
    }
}