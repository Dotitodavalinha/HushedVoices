using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour, ICameraController
{
    public Camera outsideCamera;

    public void Activate()
    {
        outsideCamera.enabled = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        outsideCamera.enabled = false;
        gameObject.SetActive(false);
    }
}

