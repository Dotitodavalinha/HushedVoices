using System;
using UnityEngine;

public class ConcentrationManager : MonoBehaviour
{
    public static ConcentrationManager Instance { get; private set; }

    [Header("Config")]
    [Tooltip("Segundos que dura cada activación")]
    public float durationSeconds = 10f;
    [Tooltip("Cuántas veces podes usarla cada día")]
    public int maxUsesPerDay = 3;
    [Tooltip("Si true, se recarga automáticamente cuando DayManager dispara OnDayStart")]
    public bool refillOnDayStart = true;

    [Header("Runtime")]
    [SerializeField] private int usesRemaining = 0;
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool inputEnabled = true;

    public event Action OnConcentrationStarted;
    public event Action OnConcentrationEnded;
    public event Action<int> OnUsesChanged; // pasa usesRemaining

    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        usesRemaining = maxUsesPerDay;
    }

    private void OnEnable()
    {
        if (refillOnDayStart)
        {
            if (DaysManager.Instance != null)
                DaysManager.Instance.OnDayStart += HandleDayStart;
        }
    }

    private void OnDisable()
    {
        if (refillOnDayStart)
        {
            if (DaysManager.Instance != null)
                DaysManager.Instance.OnDayStart -= HandleDayStart;
        }
    }

    private void HandleDayStart()
    {
        RefillUses();
    }

    private void Update()
    {
        // activacion manual: R (solo si inputEnabled y hay usos)
        if (inputEnabled && !isActive && Input.GetKeyDown(KeyCode.Q))
        {
            TryActivate();
        }

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                EndConcentration();
        }
    }

    // intenta activar: devuelve true si se activó
    public bool TryActivate()
    {
        if (!inputEnabled) return false;
        if (isActive) return false;
        if (usesRemaining <= 0) return false;

        StartConcentration();
        return true;
    }

    // forzar activación (uso por scripts)
    public bool StartConcentration()
    {
        if (isActive || usesRemaining <= 0) return false;
        Debug.Log("Concentracion Activadaa");
        isActive = true;
        timer = Mathf.Max(0.1f, durationSeconds);
        usesRemaining--;
        OnUsesChanged?.Invoke(usesRemaining);

        // eventos para que otros subscriptores reaccionen (VFX, SFX, revealables)
        OnConcentrationStarted?.Invoke();

        // ejemplo: reproducir sonido o activar shader desde quien escucha OnConcentrationStarted
        return true;
    }

    public void EndConcentration()
    {
        if (!isActive) return;

        isActive = false;
        timer = 0f;
        OnConcentrationEnded?.Invoke();
    }

    public void ForceEnd() => EndConcentration();

    public void RefillUses()
    {
        usesRemaining = maxUsesPerDay;
        OnUsesChanged?.Invoke(usesRemaining);
    }

    public int GetUsesRemaining() => usesRemaining;
    public bool IsActive() => isActive;

    // bloquear entrada del jugador para la habilidad (cinemáticas, cutscenes)
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
}
