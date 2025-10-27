using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Rendering;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class CameraConcentrationEffect : MonoBehaviour
{
    [Header("Config")]
    public GameObject overlayCameraPrefab;

    [Tooltip("Cuánto FOV se sumará a la cámara principal durante el efecto.")]
    public float fovIncrease = -5f;

    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    private GameObject instantiatedOverlayCamera;

    private float originalFOV;

    private Camera overlayCam;
    public Material overlayMaterial;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        cameraData = GetComponent<UniversalAdditionalCameraData>();

        originalFOV = mainCamera.fieldOfView;
    }

    void OnEnable()
    {
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted += ApplyEffects;
            ConcentrationManager.Instance.OnConcentrationEnded += RemoveEffects;
        }
    }

    void OnDisable()
    {
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted -= ApplyEffects;
            ConcentrationManager.Instance.OnConcentrationEnded -= RemoveEffects;
        }
    }

    void LateUpdate()
    {
        if (overlayCam != null && mainCamera != null)
        {
            if (overlayCam.fieldOfView != mainCamera.fieldOfView)
            {
                overlayCam.fieldOfView = mainCamera.fieldOfView;
            }
        }
    }

    private void ApplyEffects()
    {
        if (overlayCameraPrefab != null && cameraData != null)
        {
            overlayMaterial.SetFloat("_onOff", 1);
            // Si ya hay una subcamara, no agregamos otra
            if (instantiatedOverlayCamera != null)
            {
                Debug.LogWarning("Overlay camera ya existe, no se crea otra.");
                return;
            }

            // Guardamos FOV original
            originalFOV = mainCamera.fieldOfView;
            mainCamera.fieldOfView += fovIncrease;

            // Instanciamos
            instantiatedOverlayCamera = Instantiate(overlayCameraPrefab, transform);
            overlayCam = instantiatedOverlayCamera.GetComponent<Camera>();

            if (overlayCam != null)
            {
                overlayCam.clearFlags = CameraClearFlags.Depth;
                overlayCam.fieldOfView = mainCamera.fieldOfView;

                // Añadimos a la pila SOLO si no está ya agregado
                if (!cameraData.cameraStack.Contains(overlayCam))
                    cameraData.cameraStack.Add(overlayCam);
            }
            else
            {
                Debug.LogError("El prefab overlayCameraPrefab no tiene Camera!");
            }
        }
    }

    private void RemoveEffects()
    {
        if (instantiatedOverlayCamera != null)
        {
            overlayMaterial.SetFloat("_onOff", 0);

            mainCamera.fieldOfView = originalFOV;


            if (cameraData != null && overlayCam != null && cameraData.cameraStack.Contains(overlayCam))
                cameraData.cameraStack.Remove(overlayCam);

            Destroy(instantiatedOverlayCamera);
            instantiatedOverlayCamera = null;
            overlayCam = null;
        }
    }

}