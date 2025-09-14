using UnityEngine;
using UnityEngine.UI;

public class ClockRotationUi : MonoBehaviour
{
    [SerializeField] private LightingManager lightingManager; 
    [SerializeField] private RectTransform reloj;

    [SerializeField] private float rotationOffset = 45f;

    private void Start()
    {
        lightingManager = FindObjectOfType<LightingManager>();
        
    }
    private void LateUpdate()
    {
        if (lightingManager == null || reloj == null) return;

        // misma referencia que el sol en LightingManager: (timePercent*360) - 90
        float sunAngle = (lightingManager.TimeOfDay / 24f) * 360f + rotationOffset;
        reloj.localEulerAngles = new Vector3(0, 0, -sunAngle);
    }
}
