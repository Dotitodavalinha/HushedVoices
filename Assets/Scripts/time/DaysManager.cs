using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;

public class DaysManager : MonoBehaviour
{
    public static DaysManager Instance { get; private set; }

    [SerializeField] private int currentDay = 0;
    public int CurrentDay => currentDay;

    public event Action<int> OnDayChanged;
    public event Action OnDayEnd;
    public event Action OnDayStart;

    [SerializeField] private LightingManager timeSystem;

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

        if (timeSystem != null)
        {
            timeSystem.StartNewDay();
            Debug.Log("DaysManager: Se ha llamado a timeSystem.StartNewDay()");
        }
        else
        {
            TryHookTimeSystem();
            if (timeSystem != null)
            {
                timeSystem.StartNewDay();
            }
            else
            {
                Debug.LogError("DAYSMANAGER: No se pudo encontrar el LightingManager (timeSystem) para resetear la hora.");
            }
        }

        OnDayStart?.Invoke();
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
        var found = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();
        if (found != null && found != timeSystem)
        {
            if (timeSystem != null)
                timeSystem.OnDayFinished -= HandleDayFinished;

            timeSystem = found;
            timeSystem.OnDayFinished += HandleDayFinished;
        }
    }
}