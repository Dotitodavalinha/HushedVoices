using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;

public class DaysManager : MonoBehaviour
{
    public static DaysManager Instance { get; private set; }

    [SerializeField] private int currentDay = 0; // dia 0 = prólogo, visible en editor
    public int CurrentDay => currentDay;         // solo lectura desde afuera

    public event Action<int> OnDayChanged;           // int = nuevo día
    public event Action OnDayEnd;                    // trigger fin de día
    public event Action OnDayStart;                  // trigger inicio de día

    [SerializeField] private LightingManager timeSystem; // script de reloj

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
        if (timeSystem != null)
            timeSystem.OnDayFinished -= HandleDayFinished;
    }

    private void HandleDayFinished()
    {
        OnDayEnd?.Invoke();
        NextDay();
    }

    public void NextDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);
        Debug.LogWarning("Day Finished. Current day: " + currentDay);
        OnDayStart?.Invoke();
    }

    public void ForceSetDay(int day)
    {
        currentDay = Mathf.Max(0, day);
        OnDayChanged?.Invoke(currentDay);
    }


    // esta wea de abajo es para suscribirse al TimeSystem cuando cambio de escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryHookTimeSystem();
    }

    private void TryHookTimeSystem()
    {
        var found = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();
        if (found != null && found != timeSystem)
        {
            if (timeSystem != null)
                timeSystem.OnDayFinished -= HandleDayFinished; // me desuscribo del viejo

            timeSystem = found;
            timeSystem.OnDayFinished += HandleDayFinished; // me suscribo al nuevo
        }
    }

}
