using UnityEngine;
using UnityEngine.InputSystem;

// Ejecutar antes que la mayoría para que actualice valores antes del Update de otros scripts.
[DefaultExecutionOrder(-100)]
public class PersistentInput : MonoBehaviour
{
    public static PersistentInput Instance { get; private set; }

    // Valores que consume Player_Movement
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool Run { get; private set; }

    // InputActions en memoria
    private InputAction moveAction;
    private InputAction runAction;

    [Header("Debug")]
    public bool debugLogs = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // (re)enable si es necesario
        moveAction?.Enable();
        runAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        runAction?.Disable();
    }

    void OnDestroy()
    {
        // limpiar subscriptions
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            moveAction.Dispose();
        }

        if (runAction != null)
        {
            runAction.started -= OnRunStarted;
            runAction.canceled -= OnRunCanceled;
            runAction.Dispose();
        }

        if (Instance == this) Instance = null;
    }

    private void SetupActions()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
      
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");


        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        runAction = new InputAction("Run", InputActionType.Button);
        runAction.AddBinding("<Keyboard>/leftShift");
        runAction.AddBinding("<Keyboard>/rightShift");
        runAction.AddBinding("<Gamepad>/leftStickPress"); // opcional

        runAction.started += OnRunStarted;
        runAction.canceled += OnRunCanceled;

        moveAction.Enable();
        runAction.Enable();
    }

    // callbacks (se ejecutan en el input system update, antes de MonoBehaviour.Update)
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        Horizontal = v.x;
        Vertical = v.y;
        // debug
        if (debugLogs) Debug.Log($"[PersistentInput] Move performed H:{Horizontal} V:{Vertical}");
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        // cuando se cancela ponemos a 0 (pero Player puede mantener último valor si querés)
        Horizontal = 0f;
        Vertical = 0f;
        if (debugLogs) Debug.Log($"[PersistentInput] Move canceled -> H:0 V:0");
    }

    private void OnRunStarted(InputAction.CallbackContext ctx)
    {
        Run = true;
        if (debugLogs) Debug.Log("[PersistentInput] Run started");
    }

    private void OnRunCanceled(InputAction.CallbackContext ctx)
    {
        Run = false;
        if (debugLogs) Debug.Log("[PersistentInput] Run canceled");
    }

    // Safety fallback: en Update si por alguna razón las acciones no dieron valores,
    // rellenamos con GetKey (esto protege en edge-cases de plataforma)
    void Update()
    {
        if (moveAction == null || runAction == null) return;

        // fallback si los ejes son 0 pero realmente hay tecla física apretada (evita frame de "reseteo")
        if (Mathf.Approximately(Horizontal, 0f) && (Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed))
        {
            float h = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1f;
            Horizontal = h;
            if (debugLogs) Debug.Log($"[PersistentInput] Fallback H applied: {h}");
        }

        if (Mathf.Approximately(Vertical, 0f) && (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed))
        {
            float v = 0f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v = -1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v = 1f;
            Vertical = v;
            if (debugLogs) Debug.Log($"[PersistentInput] Fallback V applied: {v}");
        }

        // fallback run (redundante con started/canceled pero es seguro)
        bool runKb = (Keyboard.current != null) && (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
        // Priorizar action-system value (Run already set by callbacks), si difiere sincronizamos:
        if (runKb && !Run) Run = true;
        if (!runKb && Run && !runAction.triggered) { /* leave Run as is; runAction.canceled will flip it */ }
    }
}
