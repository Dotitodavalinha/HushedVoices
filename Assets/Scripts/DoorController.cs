using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Settings")]
    public float openAngle = 90f;
    public float smoothSpeed = 5f;
    public float interactionRange = 3f;

    public bool isLocked = true; // Empieza cerrada

    [Header("References")]
    public GameObject player;

    private bool isOpen = false;
    private Quaternion initialRot;

    void Start()
    {
        initialRot = transform.localRotation;
        if (player == null) player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        // Solo abre si NO está bloqueada
        if (!isLocked && Vector3.Distance(transform.position, player.transform.position) <= interactionRange)
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

    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void LockDoor()
    {
        isLocked = true;
        isOpen = false; // Se cierra automáticamente
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isLocked ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}