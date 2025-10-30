using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraConcentrationEffect : MonoBehaviour
{
    [Header("Config")]
    public GameObject overlayCameraPrefab;

    [Tooltip("Cuánto FOV se sumará a la cámara principal durante el efecto.")]
    public float fovIncrease = -5f;

    [Tooltip("Velocidad de transición del efecto y del FOV.")]
    public float transitionSpeed = 1f;

    public Material overlayMaterial;

    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    private GameObject instantiatedOverlayCamera;
    private Camera overlayCam;

    private float originalFOV;
    private float targetFOV;

    private bool applyingEffects = false;
    private bool removingEffects = false;

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

    void Update()
    {
        if (applyingEffects)
        {
            // --- Efecto blur ---
            float current = overlayMaterial.GetFloat("_onOff");
            current += Time.deltaTime * transitionSpeed;
            overlayMaterial.SetFloat("_onOff", Mathf.Clamp01(current));

            // --- FOV ---
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * transitionSpeed);
            if (overlayCam != null)
                overlayCam.fieldOfView = mainCamera.fieldOfView;

            if (current >= 1f && Mathf.Abs(mainCamera.fieldOfView - targetFOV) < 0.01f)
                applyingEffects = false;
        }

        if (removingEffects)
        {
            // --- Efecto blur ---
            float current = overlayMaterial.GetFloat("_onOff");
            current -= Time.deltaTime * transitionSpeed;
            overlayMaterial.SetFloat("_onOff", Mathf.Clamp01(current));

            // --- FOV ---
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV, Time.deltaTime * transitionSpeed);
            if (overlayCam != null)
                overlayCam.fieldOfView = mainCamera.fieldOfView;

            if (current <= 0f && Mathf.Abs(mainCamera.fieldOfView - originalFOV) < 0.01f)
            {
                removingEffects = false;

                if (cameraData != null && overlayCam != null && cameraData.cameraStack.Contains(overlayCam))
                    cameraData.cameraStack.Remove(overlayCam);

                if (instantiatedOverlayCamera != null)
                    Destroy(instantiatedOverlayCamera);

                overlayCam = null;
                instantiatedOverlayCamera = null;
            }
        }
    }

    private void ApplyEffects()
    {
        if (overlayCameraPrefab != null && cameraData != null)
        {
            if (instantiatedOverlayCamera != null)
            {
                Debug.LogWarning("Overlay camera ya existe, no se crea otra.");
                return;
            }

            originalFOV = mainCamera.fieldOfView;
            targetFOV = originalFOV + fovIncrease;

            instantiatedOverlayCamera = Instantiate(overlayCameraPrefab, transform);
            overlayCam = instantiatedOverlayCamera.GetComponent<Camera>();

            if (overlayCam != null)
            {
                overlayCam.clearFlags = CameraClearFlags.Depth;
                overlayCam.fieldOfView = mainCamera.fieldOfView;

                if (!cameraData.cameraStack.Contains(overlayCam))
                    cameraData.cameraStack.Add(overlayCam);
            }
            else
            {
                Debug.LogError("El prefab overlayCameraPrefab no tiene Camera!");
            }

            applyingEffects = true;
            removingEffects = false;
        }
    }

    private void RemoveEffects()
    {
        if (instantiatedOverlayCamera != null)
        {
            applyingEffects = false;
            removingEffects = true;
        }
    }
}
