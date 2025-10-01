using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManagerZ : MonoBehaviour
{
    public CinemachineVirtualCameraBase[] cameras;

    public CinemachineVirtualCameraBase startCamera;
    private CinemachineVirtualCameraBase currentCam;

    private void Start()
    {
        currentCam = startCamera;

        foreach (var cam in cameras)
        {
            cam.Priority = (cam == currentCam) ? 20 : 10;
        }
    }

    public void SwitchCamera(CinemachineVirtualCameraBase newCam)
    {
        if (currentCam != null)
            currentCam.Priority = 10;

        currentCam = newCam;

        if (currentCam != null)
            currentCam.Priority = 20;

        foreach (var cam in cameras)
        {
            if (cam != currentCam)
            {
                cam.Priority = 10;
            }
        }
    }

    public CinemachineVirtualCameraBase GetCurrentCamera()
    {
        return currentCam;
    }

    public void CambiarLookAt(Transform npcCurrent)
    {

        if (currentCam != null && currentCam.name == "Cafeteria")
            return;

        if (currentCam is CinemachineVirtualCamera virtualCam)
        {
            virtualCam.LookAt = npcCurrent;
        }
        else if (currentCam is CinemachineFreeLook freeLook)
        {
            freeLook.LookAt = npcCurrent;
        }
    }
}
