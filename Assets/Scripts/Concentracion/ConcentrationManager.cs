using System;
using System.Collections;
using System.Collections.Generic;
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

    private int holdCounter = 0;
    private bool isPendingEnd = false;


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
        // activacion manual: R (solo si inputEnabled y hay usos)
        if (inputEnabled && !isActive && Input.GetKeyDown(KeyCode.Q))
        {
            TryActivate();
        }

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                if (holdCounter > 0)
                {
                    // hay interacciones en curso: posponemos el End hasta que se liberen
                    isPendingEnd = true;
                    timer = 0f; // seguro
                                // no llamamos EndConcentration() todavía
                }
                else
                {
                    EndConcentration();
                }
            }
        }

    }

    // intenta activar: devuelve true si se activo
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
        /*
        if (concentrationOverlayPrefab != null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                activeOverlay = Instantiate(concentrationOverlayPrefab, canvas.transform);
                activeOverlay.transform.SetAsLastSibling(); // se asegura que quede encima de todo
            }
        }
        */
        //reproducir sonido o activar shader desde quien escucha OnConcentrationStarted
        return true;
    }

    public void EndConcentration()
    {
        // si hay holds, no terminar directamente
        if (holdCounter > 0)
        {
            isPendingEnd = true;
            return;
        }

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
        inputEnabled = false; // bloquea nueva concentracion
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

    
    public void AddInteractionHold()
    {
        holdCounter = Mathf.Max(0, holdCounter) + 1;
        
         Debug.Log($"Concentration hold added. Count: {holdCounter}");
    }

    public void RemoveInteractionHold()
    {
        holdCounter = Mathf.Max(0, holdCounter - 1);
        // si ya no quedan holds y había un End pendiente, lo ejecutamos ahora
        if (holdCounter == 0 && isPendingEnd)
        {
            isPendingEnd = false;
            
            EndConcentration();
        }
        // Debug.Log($"Concentration hold removed. Count: {holdCounter}");
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
