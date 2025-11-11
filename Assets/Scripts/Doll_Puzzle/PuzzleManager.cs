using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Refs")]
    public Canvas canvasRoot;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    [Header("Prefabs")]
    public GameObject dollBoardPrefabUI; // DollBoard_UI
    public GameObject partPrefabUI;      // Part_UI

    [Header("Sprites")]
    public Sprite headSpr, torsoSpr, armLSpr, armRSpr, legLSpr, legRSpr;

    GameObject board;
    DollBoardUIController boardCtrl;

    int placedCount;
    const int TOTAL_PARTS = 6;

    bool wasCursorVisible;
    CursorLockMode prevLock;


    private bool hasHead, hasTorso, hasArmL, hasArmR, hasLegL, hasLegR;

    public bool AllPartsCollected =>
        hasHead && hasTorso && hasArmL && hasArmR && hasLegL && hasLegR;



    void Awake() => Instance = this;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (AllPartsCollected)
                StartPuzzle();
            else
                Debug.Log("Falta recoger piezas de la muñeca antes de iniciar el puzzle.");
        }
    }



    public void StartPuzzle()
    {
        if (board != null) return;
        placedCount = 0;

        wasCursorVisible = Cursor.visible;
        prevLock = Cursor.lockState;
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
        Destroy(board); board = null;
        Debug.Log("Puzzle UI Solved");
    }

    public void RegisterFoundPart(DollPartType part)
    {
        switch (part)
        {
            case DollPartType.Head: hasHead = true; break;
            case DollPartType.Torso: hasTorso = true; break;
            case DollPartType.ArmL: hasArmL = true; break;
            case DollPartType.ArmR: hasArmR = true; break;
            case DollPartType.LegL: hasLegL = true; break;
            case DollPartType.LegR: hasLegR = true; break;
        }

        Debug.Log($"[PuzzleManager] Parte registrada: {part}. All={AllPartsCollected}");

        // opcional: auto-abrir puzzle al completar
        // if (AllPartsCollected && board == null) StartPuzzle();
    }

}
