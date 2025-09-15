using System;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Material Sky; 
    [SerializeField] private Material ambientShader;


    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 24)] public float TimeOfDay;

    [SerializeField] public float DaySpeed;
    [SerializeField] public bool IsNight => (TimeOfDay >= 20f || TimeOfDay < 5f);


    public float horaLimiteNoche = 22f;
    public bool tiempoPausado = false;

    public event Action OnDayFinished;

    private void Start()
    {
        RenderSettings.skybox = Sky;

        if (!PlayerPrefs.HasKey("HasStartedBefore"))
        {
            TimeOfDay = 22f;
            PlayerPrefs.SetFloat("SavedTimeOfDay", TimeOfDay);
            PlayerPrefs.SetInt("HasStartedBefore", 1);
        }
        else
        {
            // Ya se jugó antes, cargamos el tiempo guardado
            if (PlayerPrefs.HasKey("SavedTimeOfDay"))
                TimeOfDay = PlayerPrefs.GetFloat("SavedTimeOfDay");
        }
    }
    private void Update()
    {
        if (preset == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            if (TimeOfDay >= 5f && tiempoPausado)
            {
                tiempoPausado = false;
            }

            if (!tiempoPausado)
            {
                TimeOfDay += Time.deltaTime / DaySpeed;

                // si llego a la hora limite deja de avanzar el clockPhite
                if (TimeOfDay >= horaLimiteNoche && TimeOfDay < 24f)
                {
                    tiempoPausado = true;
                    TimeOfDay = horaLimiteNoche;
                }

                /* 
                // FUTURO: cuando se habilite el juego nocturno,
                // este bloque permitirá invocar el fin de día automático a las 00
                if (TimeOfDay >= 24f)
                {
                    TimeOfDay = 0f;
                    OnDayFinished?.Invoke();
                }
                */

                TimeOfDay %= 24;
            }


            UpdateLighting(TimeOfDay / 24);
        }

        else
        {
            UpdateLighting(TimeOfDay / 24);
        }
        Sky.SetFloat("_TimeOfDay", TimeOfDay);

        UpdateAmbientLight();
        UpdateSunLight();


        PlayerPrefs.SetFloat("SavedTimeOfDay", TimeOfDay); //guardo el tiempo en cada frame 
    }

    public void ResetTime()
    {
        TimeOfDay = 5f;
        tiempoPausado = false;

        PlayerPrefs.DeleteKey("SavedTimeOfDay");
        PlayerPrefs.DeleteKey("HasStartedBefore");
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
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    public void UpdateAmbientLight()
    {
        if (ambientShader != null)
        {
            float dayNight;

            if (TimeOfDay <= 5f)
                dayNight = 1f;
            else if (TimeOfDay >= 21f)
                dayNight = 0f;
            else
                dayNight = Mathf.InverseLerp(21f, 5f, TimeOfDay);

            ambientShader.SetFloat("_dayNight", dayNight);
        }
    }

    public void UpdateSunLight()
    {
        if (DirectionalLight != null)
        {
            float sunIntensity;
            if (TimeOfDay <= 14f)
                sunIntensity = 1f;
            else if (TimeOfDay >= 22f)
                sunIntensity = 0.1f;
            else
                sunIntensity = Mathf.Lerp(1f, 0.1f, Mathf.InverseLerp(14f, 22f, TimeOfDay));

            DirectionalLight.intensity = sunIntensity;
        }
    }

}
