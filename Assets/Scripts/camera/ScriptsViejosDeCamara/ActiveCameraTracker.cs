using UnityEngine;

public class ActiveCameraTracker : MonoBehaviour
{
    public static Camera ActiveCamera { get; private set; }

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (cam != null)
        {
            ActiveCamera = cam;
        }
    }

    void OnDisable()
    {
        // Si esta cámara era la activa, la borramos
        if (ActiveCamera == cam)
        {
            ActiveCamera = null;
        }
    }
}
