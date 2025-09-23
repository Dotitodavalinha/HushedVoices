using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraTriggerZoneOld : MonoBehaviour
{
    [Header("Cámara y Luces de esta habitación")]
    public RoomCameraController roomCamera;
    public GameObject roomLights;

    private CameraManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager != null && manager.IsInitialized())
            manager.RegisterZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager != null && manager.IsInitialized())
            manager.UnregisterZone(this);
    }

    // Centro del collider (para calcular cercanía del jugador)
    public Vector3 GetCenter()
    {
        Collider col = GetComponent<Collider>();
        return col.bounds.center;
    }
}
