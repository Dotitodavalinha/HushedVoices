using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoTest_Manager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 2;
    [SerializeField] private RectTransform boardParent;
    [SerializeField] private GridLayoutGroup grid;

    [Header("Card Setup")]
    [SerializeField] private MemoCard cardPrefab;          // único prefab
    [SerializeField] private List<Sprite> cardSprites;     // sprites de frente

    [Header("Layout")]
    [SerializeField] private Vector2 cellSize = new Vector2(150f, 200f);
    [SerializeField] private Vector2 cellSpacing = new Vector2(10f, 10f);

    [Header("Debug")]
    [SerializeField] private bool spawnOnStart = false;

    private MemoCard firstSelected;
    private MemoCard secondSelected;
    private bool isCheckingPair = false;

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
    }

    private void Start()
    {
        if (spawnOnStart)
            SetupBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetupBoard();
        }
    }

    public void SetupBoard()
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

        // Limpiar tablero anterior
        for (int i = boardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(boardParent.GetChild(i).gameObject);
        }

        firstSelected = null;
        secondSelected = null;
        isCheckingPair = false;

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
            // Son pareja: las dejamos dadas vuelta
            // Si querés deshabilitar el click, podrías quitar el raycast, etc.
        }
        else
        {
            // No son pareja, se dan vuelta de nuevo
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
