using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Material Sky;
    [SerializeField] private Material ambientShader;

    [SerializeField] private float maxSunIntensity;
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 24)] public float TimeOfDay;

    [SerializeField] public float DaySpeed;
    [SerializeField] public bool IsNight => (TimeOfDay >= 20f || TimeOfDay < 5f);

    // Solo se usa para RoomInicio
    public float horaLimiteNoche = 22f;
    public bool tiempoPausado = false;

    // Eventos necesarios para los NPCs
    public event Action OnDayFinished; // Solo se usará en RoomInicio (cuando se frena a las 22)
    public event Action OnNightStart;  // Evento de las 20:00
    public event Action OnDayStart;    // Evento de las 05:00 (StartNewDay)

    private bool nightEventTriggered = false;

    private bool IsInRoomInicio()
    {
        return SceneManager.GetActiveScene().name == "RoomInicio";
    }

    public void StartNewDay()
    {
        TimeOfDay = 5f;
        tiempoPausado = false;
        nightEventTriggered = false;
        Debug.Log($"LightingManager: Nuevo día. Hora reseteada a {TimeOfDay}");

        // Avisar a los NPCs
        OnDayStart?.Invoke();
    }

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
            if (PlayerPrefs.HasKey("SavedTimeOfDay"))
                TimeOfDay = PlayerPrefs.GetFloat("SavedTimeOfDay");
        }
    }

    private void Update()
    {
        if (preset == null) return;

        if (Application.isPlaying)
        {
            bool inRoomInicio = IsInRoomInicio();

            // Avance de tiempo
            if (!(inRoomInicio && tiempoPausado))
            {
                float previousTime = TimeOfDay;

                TimeOfDay += Time.deltaTime / DaySpeed;
                bool wrappedMidnight = false;

                if (inRoomInicio)
                {
                    // En RoomInicio frenamos a las 22 y avisamos que terminó el día
                    if (TimeOfDay >= horaLimiteNoche && TimeOfDay < 24f)
                    {
                        tiempoPausado = true;
                        TimeOfDay = horaLimiteNoche;
                        OnDayFinished?.Invoke();
                    }
                }
                else
                {
                    // En el resto de escenas NO frenamos a las 22, el día sigue normal
                    if (TimeOfDay >= 24f)
                    {
                        TimeOfDay %= 24f;
                        wrappedMidnight = true;
                    }

                    // Evento de noche para NPCs (solo si aún no se disparó este día)
                    if (previousTime < 20f && TimeOfDay >= 20f && !nightEventTriggered)
                    {
                        nightEventTriggered = true;
                        OnNightStart?.Invoke();
                    }

                    // Si cruzamos medianoche, nuevo día automático
                    if (wrappedMidnight)
                    {
                        nightEventTriggered = false; // reseteamos para el nuevo día

                        if (DaysManager.Instance != null)
                        {
                            DaysManager.Instance.AutoNextDayFromMidnight();
                        }
                    }
                }
            }

            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

        Sky.SetFloat("_TimeOfDay", TimeOfDay);

        UpdateAmbientLight();
        UpdateSunLight();

        PlayerPrefs.SetFloat("SavedTimeOfDay", TimeOfDay);
    }

    public void ResetTime()
    {
        TimeOfDay = 5f;
        tiempoPausado = false;
        nightEventTriggered = false;

        PlayerPrefs.DeleteKey("SavedTimeOfDay");
        PlayerPrefs.DeleteKey("HasStartedBefore");

        OnDayStart?.Invoke();
    }

    private void UpdateLighting(float timePercent)
    {
        if (preset == null) return;
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * -360f) - 90f, 170, 0));
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null) return;
        if (RenderSettings.sun != null) DirectionalLight = RenderSettings.sun;
        else
        {
            var l = GameObject.FindAnyObjectByType<Light>();
            if (l && l.type == LightType.Directional) DirectionalLight = l;
        }
    }

    public void UpdateAmbientLight()
    {
        if (ambientShader != null)
        {
            float dayNight;
            if (TimeOfDay <= 15f) dayNight = 1f;
            else if (TimeOfDay >= 21f) dayNight = 0f;
            else dayNight = Mathf.InverseLerp(21f, 15f, TimeOfDay);

            ambientShader.SetFloat("_dayNight", dayNight);
        }
    }

    public void UpdateSunLight()
    {
        if (DirectionalLight != null)
        {
            float sunIntensity = maxSunIntensity;
            if (TimeOfDay <= 14f) sunIntensity = maxSunIntensity;
            else if (TimeOfDay >= 22f) sunIntensity = 0.1f;
            else sunIntensity = Mathf.Lerp(maxSunIntensity, 0.1f, Mathf.InverseLerp(14f, 22f, TimeOfDay));

            DirectionalLight.intensity = sunIntensity;
        }
    }
}
