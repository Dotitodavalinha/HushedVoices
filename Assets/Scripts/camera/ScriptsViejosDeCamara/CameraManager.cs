using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private ICameraController currentCamera;

    [Header("Cámara inicial")]
    [SerializeField] private MonoBehaviour initialCameraBehaviour;
    private ICameraController initialCamera;

    private bool isInitialized = false;

    // === NUEVO ===
    private List<CameraTriggerZoneOld> activeZones = new List<CameraTriggerZoneOld>();
    private Transform player;

    private void Awake()
    {
        initialCamera = initialCameraBehaviour as ICameraController;

        // todavía no se hace Switch real, se hace en InitCamera
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        StartCoroutine(InitCamera());
    }

    private IEnumerator InitCamera()
    {
        yield return null; // espera un frame

        if (initialCamera != null)
            SwitchToCamera(initialCamera);
        else
            Debug.LogWarning("Cámara inicial no válida.");

        isInitialized = true;
    }

    public void SwitchToCamera(ICameraController newCamera, GameObject newLights = null)
    {
        if (!isInitialized) return;

        if (currentCamera != null)
            currentCamera.Deactivate();

        // Apagar TODAS las luces antes de prender la nueva
        foreach (var zone in FindObjectsOfType<CameraTriggerZoneOld>())
        {
            if (zone.roomLights != null)
                zone.roomLights.SetActive(false);
        }

        currentCamera = newCamera;
        currentCamera.Activate();

        if (newLights != null)
            newLights.SetActive(true);
    }

    public bool IsInitialized() => isInitialized;

    public MonoBehaviour GetCurrentCameraBehaviour()
    {
        Debug.Log("Current camera is: " + (currentCamera != null ? currentCamera.ToString() : "null"));
        return currentCamera as MonoBehaviour;
    }

    // === NUEVO ===
    public void RegisterZone(CameraTriggerZoneOld zone)
    {
        if (!isInitialized) return;
        if (!activeZones.Contains(zone))
            activeZones.Add(zone);
        UpdateBestZone();
    }

    public void UnregisterZone(CameraTriggerZoneOld zone)
    {
        if (!isInitialized) return;
        if (activeZones.Contains(zone))
            activeZones.Remove(zone);
        UpdateBestZone();
    }

    private void UpdateBestZone()
    {
        if (activeZones.Count == 0 || player == null) return;

        CameraTriggerZoneOld bestZone = null;
        float bestDist = float.MaxValue;

        foreach (var zone in activeZones)
        {
            float dist = Vector3.Distance(player.position, zone.GetCenter());
            if (dist < bestDist)
            {
                bestDist = dist;
                bestZone = zone;
            }
        }

        if (bestZone != null)
            SwitchToCamera(bestZone.roomCamera, bestZone.roomLights);
    }
}
