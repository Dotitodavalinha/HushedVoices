using UnityEngine;

public class RoomCameraController : MonoBehaviour, ICameraController
{
    public Camera roomCamera;

    public void Activate()
    {
        roomCamera.enabled = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        roomCamera.enabled = false;
        gameObject.SetActive(false);
    }
}
