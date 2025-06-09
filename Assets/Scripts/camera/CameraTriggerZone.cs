using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerZone : MonoBehaviour

{
    public CameraManagerZ manager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        manager.SwitchCamera(manager.CamaraPoste2);
        /*
        CameraManager manager = FindObjectOfType<CameraManager>();
        if (manager != null && manager.IsInitialized())
            manager.SwitchToCamera(targetCamera);
        */
    }
    

}

