using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Material Sky;

    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    private void Start()
    {
        RenderSettings.skybox = Sky;
    }
    private void Update()
    {
        if (preset == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime/4;
            TimeOfDay %= 24;
            UpdateLighting(TimeOfDay / 24);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24);
        }
        Sky.SetFloat("_TimeOfDay", TimeOfDay);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
        }
    }
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;
        if (RenderSettings.sun != null)
            DirectionalLight = RenderSettings.sun;
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type== LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
