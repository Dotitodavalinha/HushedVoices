using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public bool dollQuestActive = false;


    [Header("Refs")]
    public Canvas canvasRoot;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    [Header("Scene Refs (autobind)")]
    [SerializeField] bool autoBindSceneRefs = true; // NUEVO

    [Header("Doll drawing")]
    public GameObject dollDrawingPrefabUI;   //  prefab del dibujo de la nena
    GameObject drawingInstance;              // instancia actual del dibujo


    [Header("Prefabs")]
    public GameObject dollBoardPrefabUI; // DollBoard_UI
    public GameObject partPrefabUI;      // Part_UI

    [Header("Sprites")]
    public Sprite headSpr, torsoSpr, armLSpr, armRSpr, legLSpr, legRSpr;

    GameObject board;
    DollBoardUIController boardCtrl;

    [Header("Popup control")]
    [SerializeField] private bool openWhenPuzzleStarted = true; // abre auto tras diálogo
    [SerializeField] private bool closeWithE = true;             // cerrar con E
    bool pendingPopup;                                           // esperando a que cierre la UI
    public bool PuzzleStarted = false;

    int placedCount;
    const int TOTAL_PARTS = 6;

    bool wasCursorVisible;
    CursorLockMode prevLock;


    public bool hasHead, hasTorso, hasArmL, hasArmR, hasLegL, hasLegR;

    public bool AllPartsCollected =>
        hasHead && hasTorso && hasArmL && hasArmR && hasLegL && hasLegR;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (autoBindSceneRefs) BindSceneRefs(); // intenta enlazar refs de la escena actual
    }

    void Update()
    {
        // Debug / acceso manual al puzzle
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (AllPartsCollected)
                StartPuzzle();
            else
                Debug.Log("Falta recoger piezas de la muñeca antes de iniciar el puzzle.");
        }

        // Cerrar SOLO el dibujo con E
        if (closeWithE && drawingInstance != null && Input.GetKeyDown(KeyCode.E))
            CloseDollDrawing();
    }


    public void ShowDollDrawing()
    {
        if (drawingInstance != null) return;  // ya está abierto

        if (autoBindSceneRefs) BindSceneRefs();
        if (canvasRoot == null)
        {
            Debug.LogWarning("PuzzleManager: no hay Canvas para mostrar el dibujo.");
            return;
        }

        drawingInstance = Instantiate(dollDrawingPrefabUI, canvasRoot.transform);
        dollQuestActive = true;


    }
    void CloseDollDrawing()
    {
        if (drawingInstance == null) return;
        Destroy(drawingInstance);
        drawingInstance = null;
    }


    void BindSceneRefs()
    {
        // Si están asignadas en el inspector para cada escena, no tocamos.
        if (canvasRoot == null) canvasRoot = FindObjectOfType<Canvas>(true);
        if (raycaster == null) raycaster = FindObjectOfType<GraphicRaycaster>(true);
        if (eventSystem == null) eventSystem = FindObjectOfType<EventSystem>(true);
    }


    public void StartPuzzle()
    {
        if (board != null) return;

        if (autoBindSceneRefs) BindSceneRefs();
        if (canvasRoot == null || raycaster == null || eventSystem == null)
        {
            Debug.LogWarning("PuzzleManager: faltan refs de escena (Canvas/Raycaster/EventSystem).");
            return;
        }

        placedCount = 0;
        if (GameManager.Instance != null) GameManager.Instance.TryLockUI();

        wasCursorVisible = Cursor.visible;
        prevLock = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        board = Instantiate(dollBoardPrefabUI, canvasRoot.transform);
        boardCtrl = board.GetComponent<DollBoardUIController>();

        Vector2 basePos = new Vector2(420f, 200f);
        SpawnPart("Head", headSpr, basePos + new Vector2(0, 0));
        SpawnPart("Torso", torsoSpr, basePos + new Vector2(0, -90));
        SpawnPart("ArmL", armLSpr, basePos + new Vector2(0, -180));
        SpawnPart("ArmR", armRSpr, basePos + new Vector2(0, -270));
        SpawnPart("LegL", legLSpr, basePos + new Vector2(0, -360));
        SpawnPart("LegR", legRSpr, basePos + new Vector2(0, -450));
    }


    void SpawnPart(string id, Sprite spr, Vector2 anchoredPos)
    {
        var go = Instantiate(partPrefabUI, canvasRoot.transform);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;

        var img = go.GetComponent<Image>();
        img.sprite = spr;

        var dp = go.GetComponent<DraggablePart>();
        dp.targetId = id;
        dp.raycaster = raycaster;
        dp.SetInitialAnchoredPos(rt.anchoredPosition); // clave para evitar “salto”
    }

    // llamada desde DraggablePartUI cuando una pieza fue correcta
    public void CorrectPlacement(string id)
    {
        if (boardCtrl != null)
        {
            boardCtrl.RevealPart(id);
            placedCount++;
            if (boardCtrl.IsComplete(TOTAL_PARTS))
                boardCtrl.PlayCompleteAndClose(FinishPuzzle);
        }
    }

    // solo si alguna vez querés contar colocaciones “a la vieja”
    public void NotifyPlaced()
    {
        placedCount++;
        if (placedCount >= TOTAL_PARTS) FinishPuzzle();
    }

    void FinishPuzzle()
    {
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = prevLock;

        if (GameManager.Instance != null) GameManager.Instance.UnlockUI(); // :contentReference[oaicite:4]{index=4}

        if (board != null) { Destroy(board); board = null; }
        // si esto vino del flujo “PuzzleStarted tras diálogo”, ya consumimos ese estado
        PuzzleStarted = false;
        pendingPopup = false;

        Debug.Log("Puzzle UI Solved/Cerrado");
        ProgressManager.Instance.CambiarRootNPC("Niña", "DollFound");
    }


    public void RegisterFoundPart(DollPartType part)
    {
        Debug.Log("Se registra DollPart" + part);
        switch (part)
        {
            case DollPartType.Head: hasHead = true; break;
            case DollPartType.Torso: hasTorso = true; break;
            case DollPartType.ArmL: hasArmL = true; break;
            case DollPartType.ArmR: hasArmR = true; break;
            case DollPartType.LegL: hasLegL = true; break;
            case DollPartType.LegR: hasLegR = true; break;
        }
        if (AllPartsCollected && board == null)
        {
            Debug.Log("se encontraron todas las DollParts");
            StartPuzzle();  // se abre el puzzle automáticamente al tener todas
        }

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // NUEVO
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // NUEVO
    }
    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (autoBindSceneRefs) BindSceneRefs(); // NUEVO: re-enlaza refs por escena
    }


}
