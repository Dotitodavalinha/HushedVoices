using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tumbler : MonoBehaviour
{
    private Vector3 initialPosition;
    [SerializeField]
    private int tumblerNumber;

    [SerializeField]
    private Vector3 lockedPosition = new Vector3(-2.2f, 2.0f, 0.0f);

    public int TumblerNumber => tumblerNumber;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void LockTumbler()
    {
        transform.position = lockedPosition;
    }

    public void ResetTumbler()
    {
        transform.position = initialPosition;
    }
}