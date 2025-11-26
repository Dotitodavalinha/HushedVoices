using UnityEngine;
using UnityEngine.UI;

public class LightsOutGrid : MonoBehaviour
{
    public GameObject cellPrefab;
    public int rows = 3;
    public int cols = 3;
    public Transform gridParent;

    [Header("Activación NPC")]
    public NPCStoreKeeper targetNPC;

    private PuzzleActivator puzzleActivator;
    private LightsOutCell[,] grid;

    void Start()
    {
        puzzleActivator = FindObjectOfType<PuzzleActivator>();

        if (targetNPC == null)
        {
            targetNPC = FindObjectOfType<NPCStoreKeeper>();
        }

        InitializeGrid();
    }

    void Update()
    {
        // Truco para ganar rápido con la C
        if (Input.GetKeyDown(KeyCode.C))
        {
            ForceWin();
        }
    }

    private void InitializeGrid()
    {
        // 1. Limpieza de hijos antiguos (evita que se acumulen cuadros vacíos)
        if (gridParent.childCount > 0)
        {
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
        }

        grid = new LightsOutCell[rows, cols];

        // 2. Crear celdas nuevas
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                LightsOutCell cell = cellObj.GetComponent<LightsOutCell>();

                if (cell != null)
                {
                    cell.Initialize(r, c, this);
                    grid[r, c] = cell;
                    cell.SetLightState(false);
                }
            }
        }

        // 3. Mezclar el puzzle
        for (int i = 0; i < 15; i++)
        {
            int randomRow = Random.Range(0, rows);
            int randomCol = Random.Range(0, cols);

            ToggleCellAndCheckBounds(randomRow, randomCol);
            ToggleCellAndCheckBounds(randomRow + 1, randomCol);
            ToggleCellAndCheckBounds(randomRow - 1, randomCol);
            ToggleCellAndCheckBounds(randomRow, randomCol + 1);
            ToggleCellAndCheckBounds(randomRow, randomCol - 1);
        }
    }
    public void ResetPuzzleLogic()
    {
        gameObject.SetActive(true);

        InitializeGrid();

        if (puzzleActivator != null)
        {
            puzzleActivator.gameObject.SetActive(true);
        }
    }

    public void CellClicked(int r, int c)
    {
        ToggleCellAndCheckBounds(r, c);
        ToggleCellAndCheckBounds(r + 1, c);
        ToggleCellAndCheckBounds(r - 1, c);
        ToggleCellAndCheckBounds(r, c + 1);
        ToggleCellAndCheckBounds(r, c - 1);
        CheckWinCondition();
    }

    private void ToggleCellAndCheckBounds(int r, int c)
    {
        if (r >= 0 && r < rows && c >= 0 && c < cols)
        {
            grid[r, c].ToggleState();
        }
    }

    private void CheckWinCondition()
    {
        bool allOff = true;
        foreach (var cell in grid)
        {
            if (cell.IsLightOn())
            {
                allOff = false;
                break;
            }
        }

        if (allOff)
        {
            // ganaste
            if (targetNPC != null)
            {
                if (targetNPC.objectToToggle != null)
                    targetNPC.objectToToggle.SetActive(false);

                targetNPC.ActivateMovementAfterPuzzle();
            }

            if (puzzleActivator != null)
            {
                puzzleActivator.DeactivatePuzzleAndActivator();
            }

            transform.gameObject.SetActive(false);
        }
    }

    private void ForceWin()
    {
        foreach (var cell in grid)
        {
            cell.SetLightState(false);
        }
        CheckWinCondition();
    }
}