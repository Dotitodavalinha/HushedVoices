using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoTest_Manager : MonoBehaviour
{
    [Header("Root UI")]
    [SerializeField] private GameObject memoRoot;          // panel negro / root del memotest
    [SerializeField] private RectTransform boardParent;    // contenedor de cartas
    [SerializeField] private GridLayoutGroup grid;

    [Header("Grid")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 2;

    [Header("Card Setup")]
    [SerializeField] private MemoCard cardPrefab;          // único prefab
    [SerializeField] private List<Sprite> cardSprites;     // sprites de frente

    [Header("Layout")]
    [SerializeField] private Vector2 cellSize = new Vector2(150f, 200f);
    [SerializeField] private Vector2 cellSpacing = new Vector2(10f, 10f);

    [Header("Debug")]
    [SerializeField] private bool spawnOnStart = false;

    // estado interno
    private MemoCard firstSelected;
    private MemoCard secondSelected;
    private bool isCheckingPair = false;

    private bool waitingToStartMemo = false;
    private bool memoOpen = false;

    private int totalPairs;
    private int matchedPairs;

    bool wasCursorVisible;
    CursorLockMode prevLock;

    private void Awake()
    {
        if (grid == null && boardParent != null)
            grid = boardParent.GetComponent<GridLayoutGroup>();

        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.cellSize = cellSize;
            grid.spacing = cellSpacing;
            grid.childAlignment = TextAnchor.MiddleCenter;
        }

        if (memoRoot != null)
            memoRoot.SetActive(false); // arranca cerrado
    }

    private void Start()
    {
        if (spawnOnStart)
            StartMemoTest();
    }

    private void Update()
    {
        // placeholder para probar
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartMemoTest();
        }
    }

    // ------------------ APERTURA / CIERRE CON GAME MANAGER ------------------

    public void StartMemoTest()
    {
        if (waitingToStartMemo || memoOpen)
            return;

        StartCoroutine(Co_StartMemoTest());
    }

    private IEnumerator Co_StartMemoTest()
    {
        waitingToStartMemo = true;

        // 1) Esperar a que NO haya ninguna otra UI abierta
        while (GameManager.Instance != null && GameManager.Instance.IsAnyUIOpen)
            yield return null;

        // 2) Lockear UI
        if (GameManager.Instance != null)
            GameManager.Instance.TryLockUI();

        // 3) Cursor
        wasCursorVisible = Cursor.visible;
        prevLock = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 4) Activar root del memotest y armar tablero
        if (memoRoot != null)
            memoRoot.SetActive(true);

        SetupBoard();

        memoOpen = true;
        waitingToStartMemo = false;
    }

    private void FinishMemoTest()
    {
        // limpiar cartas
        if (boardParent != null)
        {
            for (int i = boardParent.childCount - 1; i >= 0; i--)
            {
                Destroy(boardParent.GetChild(i).gameObject);
            }
        }

        // ocultar panel
        if (memoRoot != null)
            memoRoot.SetActive(false);

        // restaurar cursor
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = prevLock;

        // liberar UI
        if (GameManager.Instance != null)
            GameManager.Instance.UnlockUI();

        memoOpen = false;
    }

    // ------------------ LÓGICA DEL TABLERO / PAREJAS ------------------

    private void SetupBoard()
    {
        if (boardParent == null || cardPrefab == null)
        {
            Debug.LogError("[MemoTest] Falta boardParent o cardPrefab.");
            return;
        }

        int totalSlots = columns * rows;
        if (totalSlots % 2 != 0)
        {
            Debug.LogError("[MemoTest] La grilla debe tener cantidad PAR de casillas.");
            return;
        }

        int pairCount = totalSlots / 2;
        if (cardSprites.Count < pairCount)
        {
            Debug.LogError($"[MemoTest] Necesitás al menos {pairCount} sprites distintos (uno por pareja).");
            return;
        }

        // limpiar tablero anterior
        for (int i = boardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(boardParent.GetChild(i).gameObject);
        }

        firstSelected = null;
        secondSelected = null;
        isCheckingPair = false;
        matchedPairs = 0;
        totalPairs = pairCount;

        // Crear IDs de pareja (0,0,1,1,2,2...)
        List<int> pairIds = new List<int>(totalSlots);
        for (int i = 0; i < pairCount; i++)
        {
            pairIds.Add(i);
            pairIds.Add(i);
        }

        Shuffle(pairIds);

        // Instanciar cartas
        for (int i = 0; i < pairIds.Count; i++)
        {
            int pairId = pairIds[i];
            Sprite sprite = cardSprites[pairId];

            MemoCard card = Instantiate(cardPrefab, boardParent);
            card.Setup(sprite, this, pairId);
        }
    }

    public void OnCardClicked(MemoCard card)
    {
        if (!memoOpen) return;
        if (isCheckingPair) return;
        if (card.IsRevealed) return;

        if (firstSelected == null)
        {
            firstSelected = card;
            card.Reveal();
        }
        else if (secondSelected == null)
        {
            secondSelected = card;
            card.Reveal();
            StartCoroutine(CheckPair());
        }
    }

    private IEnumerator CheckPair()
    {
        isCheckingPair = true;
        yield return new WaitForSeconds(0.8f);

        if (firstSelected.PairId == secondSelected.PairId)
        {
            // son pareja  contabilizar
            matchedPairs++;

            // ¿ganó el memotest?
            if (matchedPairs >= totalPairs)
            {
                yield return new WaitForSeconds(0.3f);

                ProgressManager.Instance.CambiarRootNPC("Lola", "Root1"); //Root chismos actualizado

                FinishMemoTest();
            }

        }
        else
        {
            // no son pareja  se dan vuelta de nuevo
            firstSelected.ShowBack();
            secondSelected.ShowBack();
        }

        firstSelected = null;
        secondSelected = null;
        isCheckingPair = false;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
