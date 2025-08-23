using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerZoneOld : MonoBehaviour
{
    public RoomCameraController targetCamera;
    public GameObject newLights;
    public GameObject oldLights;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        newLights.SetActive(true);
        oldLights.SetActive(false);

        CameraManager manager = FindObjectOfType<CameraManager>();
        if (manager != null && manager.IsInitialized())
            manager.SwitchToCamera(targetCamera);
        
    }

}
