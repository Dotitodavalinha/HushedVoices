using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerZone : MonoBehaviour
{
    public RoomCameraController targetCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CameraManager manager = FindObjectOfType<CameraManager>();
        if (manager != null && manager.IsInitialized())
            manager.SwitchToCamera(targetCamera);
    }


}

