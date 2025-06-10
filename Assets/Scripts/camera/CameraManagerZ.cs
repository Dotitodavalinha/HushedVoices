using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManagerZ : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;

    public CinemachineVirtualCamera CamaraPoste1;
    public CinemachineVirtualCamera CamaraPoste2;
    public CinemachineVirtualCamera CamaraCafetería;


    public CinemachineVirtualCamera startCamera;
    private CinemachineVirtualCamera currentCam;

    private void Start()
    {
        currentCam = startCamera;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == currentCam)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }

        }
    }
    public void SwitchCamera(CinemachineVirtualCamera newCam)
    {
        currentCam.Priority = 10;

        currentCam = newCam;

        currentCam.Priority = 20;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != currentCam)
            {
                cameras[i].Priority = 10;
            }
        }
    }

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return currentCam;
    }
}
