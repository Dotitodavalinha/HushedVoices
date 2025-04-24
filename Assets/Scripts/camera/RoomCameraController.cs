using System.Collections;
using UnityEngine;

public class RoomCameraController : MonoBehaviour, ICameraController
{
    public Camera roomCamera;
    
    public void Activate()
    {

        Debug.Log("RoomCamera ACTIVATED!");
        roomCamera.enabled = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        roomCamera.enabled = false;
        gameObject.SetActive(false);
    }

}
