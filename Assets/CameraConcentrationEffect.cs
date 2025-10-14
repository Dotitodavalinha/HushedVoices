using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class CameraConcentrationEffect : MonoBehaviour
{
    [Header("Config")]
    public GameObject overlayCameraPrefab;
    public VolumeProfile concentrationVolumeProfile;

    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    private GameObject instantiatedOverlayCamera;
    private Volume globalVolume;

    private FloatParameter originalSaturation = new FloatParameter(0f);

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        cameraData = GetComponent<UniversalAdditionalCameraData>();
        globalVolume = FindObjectsOfType<Volume>().FirstOrDefault(v => v.isGlobal);

        if (globalVolume != null)
        {
            ColorAdjustments colorAdjustments;
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                originalSaturation.value = colorAdjustments.saturation.value;
            }
        }
    }

    void OnEnable()
    {
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted += ApplyEffects;
            ConcentrationManager.Instance.OnConcentrationEnded += RemoveEffects;
        }
        else
        {
            //Debug.LogError("ConcentrationManager.Instance es NULL en OnEnable");
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

    private void ApplyEffects()
    {
        if (overlayCameraPrefab != null && cameraData != null && instantiatedOverlayCamera == null)
        {
            instantiatedOverlayCamera = Instantiate(overlayCameraPrefab, transform);
            Camera overlayCam = instantiatedOverlayCamera.GetComponent<Camera>();

            if (overlayCam != null)
            {
                overlayCam.fieldOfView = mainCamera.fieldOfView;

                cameraData.cameraStack.Add(overlayCam);
            }
        }

        if (globalVolume != null)
        {
            ColorAdjustments colorAdjustments;
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments.saturation.overrideState = true;
                colorAdjustments.saturation.value = -100f;
            }
        }
    }

    private void RemoveEffects()
    {
        if (instantiatedOverlayCamera != null)
        {
            Camera overlayCam = instantiatedOverlayCamera.GetComponent<Camera>();
            if (cameraData != null && overlayCam != null)
            {
                cameraData.cameraStack.Remove(overlayCam);
            }
            Destroy(instantiatedOverlayCamera);
            instantiatedOverlayCamera = null;
        }

        if (globalVolume != null)
        {
            ColorAdjustments colorAdjustments;
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments.saturation.value = originalSaturation.value;
            }
        }
    }
}
