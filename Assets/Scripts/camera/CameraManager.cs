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

    private void Awake()
    { 
        initialCamera = initialCameraBehaviour as ICameraController;

        //apagamos todas las camaras
        foreach (var cam in FindObjectsOfType<Camera>())
        {
            if (cam != (initialCameraBehaviour as MonoBehaviour).GetComponent<Camera>())
                cam.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        
        StartCoroutine(InitCamera());
    }

    private IEnumerator InitCamera()
    {
        yield return null; // espera un frame 

        if (initialCamera != null)
        {
            Debug.Log($"Initial camera: {initialCamera}, Behaviour: {initialCameraBehaviour}");
            SwitchToCamera(initialCamera);
        }
        else
        {
            Debug.LogWarning("Cámara inicial no válida.");
        }

        isInitialized = true;
    }

    public void SwitchToCamera(ICameraController newCamera)
    {
        if (!isInitialized) return;

        if (currentCamera != null)
            currentCamera.Deactivate();

        currentCamera = newCamera;
        currentCamera.Activate();
    }

    public bool IsInitialized() => isInitialized;

    public MonoBehaviour GetCurrentCameraBehaviour()
    {
        Debug.Log("Current camera is: " + (currentCamera != null ? currentCamera.ToString() : "null"));
        return currentCamera as MonoBehaviour;
    }

}


