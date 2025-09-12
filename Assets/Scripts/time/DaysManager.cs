using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DaysManager : MonoBehaviour
{
    public static DaysManager Instance { get; private set; }

    public int CurrentDay { get; private set; } = 0; // dia 0 = prólogo
    public event Action<int> OnDayChanged;           // int = nuevo día
    public event Action OnDayEnd;                    // trigger fin de día
    public event Action OnDayStart;                  // trigger inicio de día

    [SerializeField] private LightingManager timeSystem; // script de reloj (24h)

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (timeSystem != null)
            timeSystem.OnDayFinished += HandleDayFinished;
     
    }

    private void OnDisable()
    {
        if (timeSystem != null)
            timeSystem.OnDayFinished -= HandleDayFinished;
    }

    private void HandleDayFinished()
    {
        OnDayEnd?.Invoke();   // dispara eventos de fin de día (guardar, cerrar NPCs, etc.)
        NextDay();
    }

    public void NextDay()
    {
        CurrentDay++;
        OnDayChanged?.Invoke(CurrentDay);
        OnDayStart?.Invoke();

        // Si ya usás un EventManager global, acá podés loguear:
        // EventManager.Instance.LogEvent("DayStarted", CurrentDay);
    }

    public void ForceSetDay(int day) // útil para debug / cargar partidas
    {
        CurrentDay = Mathf.Max(0, day);
        OnDayChanged?.Invoke(CurrentDay);
    }
}
