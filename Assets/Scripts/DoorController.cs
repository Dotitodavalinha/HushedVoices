using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Settings")]
    public float openAngle = 90f;
    public float smoothSpeed = 5f;
    public float interactionRange = 3f;

    [Header("References")]
    public GameObject player;

    private bool isOpen = false;
    private Quaternion initialRot;

    void Start()
    {
        initialRot = transform.localRotation;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= interactionRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isOpen = !isOpen;
            }
        }

        float targetY = isOpen ? openAngle : 0f;
        Quaternion targetRot = Quaternion.Euler(0, targetY, 0) * initialRot;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * smoothSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}