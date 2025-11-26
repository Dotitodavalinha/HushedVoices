using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DaysManager : MonoBehaviour
{
    public static DaysManager Instance { get; private set; }

    [SerializeField] private int currentDay = 0;
    public int CurrentDay => currentDay;

    public event Action<int> OnDayChanged;
    public event Action OnDayStart;

    [SerializeField] private LightingManager timeSystem;

    // Flag para saber si el día ya fue avanzado automáticamente al cruzar medianoche
    private bool dayAdvancedThisCycle = false;

    private bool IsInRoomInicio()
    {
        return SceneManager.GetActiveScene().name == "RoomInicio";
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryHookTimeSystem();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void NextDay()
    {
        // Llamado por la cama u otros eventos que hacen "dormir"/pasar de día

        if (!dayAdvancedThisCycle)
        {
            currentDay++;
            OnDayChanged?.Invoke(currentDay);
            Debug.LogWarning("Iniciando siguiente día (manual): " + currentDay);
        }
        else
        {
            Debug.LogWarning("NextDay() llamado después de haber pasado medianoche. No se incrementa el día otra vez, solo se salta a la mañana.");
        }

        if (timeSystem != null)
        {
            timeSystem.StartNewDay(); // pone la hora en 5 y resetea flags internos
        }
        else
        {
            TryHookTimeSystem();
            if (timeSystem != null)
            {
                timeSystem.StartNewDay();
            }
        }

        OnDayStart?.Invoke();

        // Nuevo día “limpio”: reseteamos el flag hasta la próxima medianoche.
        dayAdvancedThisCycle = false;
    }

    /// <summary>
    /// Llamado automáticamente por el LightingManager cuando el tiempo cruza de 23:xx a 00:xx
    /// en escenas que NO son RoomInicio.
    /// </summary>
    public void AutoNextDayFromMidnight()
    {
        currentDay++;
        dayAdvancedThisCycle = true;
        OnDayChanged?.Invoke(currentDay);
        Debug.LogWarning("Día avanzado automáticamente al cruzar medianoche. Día actual: " + currentDay);
    }

    public void ForceSetDay(int day)
    {
        currentDay = Mathf.Max(0, day);
        OnDayChanged?.Invoke(currentDay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryHookTimeSystem();
    }

    private void TryHookTimeSystem()
    {
        var found = GameObject.FindAnyObjectByType<LightingManager>();

        if (found != null && found != timeSystem)
        {
            timeSystem = found;
        }
    }

    public int CurrentHour
    {
        get
        {
            return timeSystem != null ? Mathf.FloorToInt(timeSystem.TimeOfDay) : 0;
        }
    }
}
