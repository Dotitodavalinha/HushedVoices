using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PersistentInput : MonoBehaviour
{
    public static PersistentInput Instance { get; private set; }

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool Run { get; private set; }

    // --- nuevo: guardamos los últimos valores válidos ---
    private float lastH, lastV;
    private bool lastRun;
    private int framesToHold = 2; // mantener por 2 frames tras un "reseteo"
    private int holdCounter = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // fallback si GetAxis se resetea a 0 justo en el cambio de escena
        if (Mathf.Approximately(h, 0f))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
        }
        if (Mathf.Approximately(v, 0f))
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
        }

        // si el eje se resetea completamente pero antes había input, mantenelo brevemente
        bool axesReset = Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f) && run && (lastH != 0f || lastV != 0f);
        if (axesReset && holdCounter == 0)
        {
            holdCounter = framesToHold;
        }

        if (holdCounter > 0)
        {
            h = lastH;
            v = lastV;
            run = lastRun;
            holdCounter--;
        }

        Horizontal = h;
        Vertical = v;
        Run = run;

        lastH = h;
        lastV = v;
        lastRun = run;
    }
}
