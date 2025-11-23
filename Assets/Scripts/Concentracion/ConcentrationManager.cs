using System;
using System.Collections;
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

    [Header("Visual Feedback")]
    private GameObject activeOverlay;
    public int MaxUsesPerDay => maxUsesPerDay;
    public int UsesRemaining => usesRemaining;
    public float RemainingSeconds => Mathf.Max(0f, timer);
    public float DurationSeconds => durationSeconds;

    [Header("Fatiga")]
    [Tooltip("Cuánto más lento se mueve el jugador luego de usar Concentración (1 = sin cambio, 0.67 = 33% más lento)")]
    [Range(0f, 1f)] public float fatigueSpeedMultiplier = 0.67f;
    [Tooltip("Duración en segundos de la fatiga post-concentración")]
    public float fatigueDuration = 5f;
    private bool isFatigued = false;

    // holds de interacción: mientras sea >0, el temporizador se PAUSA
    private int holdCounter = 0;

    public event Action OnConcentrationStarted;
    public event Action OnConcentrationEnded;
    public event Action<int> OnUsesChanged;

    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        usesRemaining = maxUsesPerDay;
    }

    private Coroutine subscribeRoutine;

    private void OnEnable()
    {
        if (!refillOnDayStart) return;
        subscribeRoutine = StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null) StopCoroutine(subscribeRoutine);
        if (DaysManager.Instance != null)
            DaysManager.Instance.OnDayStart -= HandleDayStart;
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (DaysManager.Instance == null) yield return null;
        DaysManager.Instance.OnDayStart += HandleDayStart;
    }

    private void HandleDayStart()
    {
        RefillUses();
    }

    private void Update()
    {
        // activación manual: Q (solo si inputEnabled y hay usos)
        if (inputEnabled && !isActive && Input.GetKeyDown(KeyCode.Q))
        {
            TryActivate();
        }

        if (isActive)
        {
            // NUEVO COMPORTAMIENTO:
            // Si hay una interacción/pista abierta (holdCounter > 0),
            // la concentración SIGUE activa pero el timer se PAUSA.
            if (holdCounter > 0)
            {
                return; // no descontamos tiempo mientras haya una UI de pista abierta
            }

            // Sin holds, el timer avanza normal
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                EndConcentration();
            }
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
        Debug.Log("Concentracion Activada");
        isActive = true;
        timer = Mathf.Max(0.1f, durationSeconds);
        usesRemaining--;
        OnUsesChanged?.Invoke(usesRemaining);

        // eventos para que otros subscriptores reaccionen (VFX, SFX, revealables)
        OnConcentrationStarted?.Invoke();
        return true;
    }

    public void EndConcentration()
    {
        // NUEVO: ya no miramos holdCounter ni posponemos el End.
        // El timer NUNCA llega a 0 mientras holdCounter > 0, así que
        // EndConcentration solo se dispara cuando ya no hay pista abierta
        // (o si algún script llama ForceEnd).

        if (!isActive) return;

        isActive = false;
        timer = 0f;

        if (activeOverlay != null)
        {
            Destroy(activeOverlay);
            activeOverlay = null;
        }
        OnConcentrationEnded?.Invoke();

        // activa fatiga post concentracion
        if (!isFatigued)
            StartCoroutine(ApplyFatigue());
    }

    private IEnumerator ApplyFatigue()
    {
        isFatigued = true;
        inputEnabled = false; // bloquea nueva concentración
        var player = FindObjectOfType<Player_Movement>();
        float originalSpeed = 0f;

        if (player != null)
        {
            originalSpeed = player.moveSpeed;
            player.moveSpeed *= fatigueSpeedMultiplier;
        }

        yield return new WaitForSeconds(fatigueDuration);

        if (player != null)
            player.moveSpeed = originalSpeed;

        inputEnabled = true;
        isFatigued = false;
    }

    // llamado por los interactuables cuando abren una UI de pista
    public void AddInteractionHold()
    {
        holdCounter = Mathf.Max(0, holdCounter) + 1;
        Debug.Log($"Concentration hold added. Count: {holdCounter}");
    }

    // llamado cuando se cierra esa UI
    public void RemoveInteractionHold()
    {
        holdCounter = Mathf.Max(0, holdCounter - 1);
        Debug.Log($"Concentration hold removed. Count: {holdCounter}");
        // NO hay más lógica acá: si holdCounter vuelve a 0,
        // el Update retomará el conteo normal del timer.
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
