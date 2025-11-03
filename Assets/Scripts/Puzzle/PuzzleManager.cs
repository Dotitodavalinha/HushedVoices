using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleManagerUI : MonoBehaviour
{
    public static PuzzleManagerUI Instance { get; private set; }

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
    int placedCount;
    bool wasCursorVisible;
    CursorLockMode prevLock;

    void Awake() => Instance = this;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            StartPuzzle();
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

        var dp = go.GetComponent<DraggablePartUI>();
        dp.targetId = id;
        dp.raycaster = raycaster;
        dp.SetInitialAnchoredPos(rt.anchoredPosition); // importante
    }

    public void NotifyPlaced()
    {
        placedCount++;
        if (placedCount >= 6) FinishPuzzle();
    }

    void FinishPuzzle()
    {
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = prevLock;
        Debug.Log("Puzzle UI Solved");
        // Si querés cerrar el board: Destroy(board); board = null;
    }
}
