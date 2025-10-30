using System.Collections;
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

    private float currentOnOff = 0f;
    private float targetOnOff = 0f;
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        cameraData = GetComponent<UniversalAdditionalCameraData>();
        originalFOV = mainCamera.fieldOfView;
    }
    private void Start()
    {
        if (ConcentrationManager.Instance != null)
        {
            if (ConcentrationManager.Instance.IsActive())
                ApplyEffects();
            else
                ForceResetEffect();
        }
    }

    void OnEnable()
{
    if (ConcentrationManager.Instance != null)
    {
        ConcentrationManager.Instance.OnConcentrationStarted += ApplyEffects;
        ConcentrationManager.Instance.OnConcentrationEnded += RemoveEffects;

        if (ConcentrationManager.Instance.IsActive())
        {
            ApplyEffects();
        }
        else
        {
            ForceResetEffect();
        }
    }
}
    private void ForceResetEffect()
    {
        if (overlayMaterial != null)
            overlayMaterial.SetFloat("_onOff", 0f);

        if (mainCamera != null)
            mainCamera.fieldOfView = originalFOV;

        if (cameraData != null && overlayCam != null && cameraData.cameraStack.Contains(overlayCam))
            cameraData.cameraStack.Remove(overlayCam);

        if (instantiatedOverlayCamera != null)
            Destroy(instantiatedOverlayCamera);

        overlayCam = null;
        instantiatedOverlayCamera = null;

        applyingEffects = false;
        removingEffects = false;
    }

    void OnDisable()
    {
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted -= ApplyEffects;
            ConcentrationManager.Instance.OnConcentrationEnded -= RemoveEffects;
        }

        if (overlayMaterial != null)
            overlayMaterial.SetFloat("_onOff", 0f);

        if (cameraData != null && overlayCam != null && cameraData.cameraStack.Contains(overlayCam))
            cameraData.cameraStack.Remove(overlayCam);

        if (instantiatedOverlayCamera != null)
            Destroy(instantiatedOverlayCamera);

        overlayCam = null;
        instantiatedOverlayCamera = null;
        applyingEffects = false;    
        removingEffects = false;

    }


    void Update()
    {
        if (overlayMaterial == null) return;

        // --- BLUR (onOff) ---
        currentOnOff = Mathf.MoveTowards(currentOnOff, targetOnOff, Time.deltaTime * transitionSpeed);
        overlayMaterial.SetFloat("_onOff", currentOnOff);

        // --- FOV ---
        float currentFovTarget = applyingEffects ? targetFOV : originalFOV;
        mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, currentFovTarget, Time.deltaTime * transitionSpeed);
        if (overlayCam != null)
            overlayCam.fieldOfView = mainCamera.fieldOfView;

        // --- LIMPIEZA DE ESTADOS ---
        if (applyingEffects && Mathf.Approximately(currentOnOff, 1f) && Mathf.Approximately(mainCamera.fieldOfView, targetFOV))
            applyingEffects = false;

        if (removingEffects && Mathf.Approximately(currentOnOff, 0f) && Mathf.Approximately(mainCamera.fieldOfView, originalFOV))
        {
            removingEffects = false;

            // Limpieza de overlay camera
            if (cameraData != null && overlayCam != null && cameraData.cameraStack.Contains(overlayCam))
                cameraData.cameraStack.Remove(overlayCam);

            if (instantiatedOverlayCamera != null)
                Destroy(instantiatedOverlayCamera);

            overlayCam = null;
            instantiatedOverlayCamera = null;
        }

        // --- FORZAR DESACTIVACIÓN si manager indica que terminó ---
        if (ConcentrationManager.Instance != null)
        {
            if (!ConcentrationManager.Instance.IsActive() && (applyingEffects || currentOnOff > 0f))
            {
                RemoveEffects();
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

        // Asegurarse de que originalFOV no se sobrescriba innecesariamente
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

        applyingEffects = true;
        removingEffects = false;
        targetOnOff = 1f;
    }
}

private void RemoveEffects()
{
    applyingEffects = false;
    removingEffects = true;
    targetOnOff = 0f;
    // Asegurarse de que el FOV vuelva al valor original
    targetFOV = originalFOV;
}


    private IEnumerator ForceDisableMaterial()
    {
        float value = overlayMaterial.GetFloat("_onOff");
        while (value > 0f)
        {
            value -= Time.deltaTime * transitionSpeed;
            overlayMaterial.SetFloat("_onOff", Mathf.Clamp01(value));
            yield return null;
        }
        overlayMaterial.SetFloat("_onOff", 0f);
    }

}
