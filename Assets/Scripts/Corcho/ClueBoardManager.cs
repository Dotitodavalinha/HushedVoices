using UnityEngine;
using System.Collections.Generic;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] private List<ClueData> allClues;
    [SerializeField] private ClueNode cluePrefab;
    [SerializeField] private RectTransform boardParent;
    [SerializeField] private GameObject Corchoculpable;

    private List<GameObject> nodosInstanciados = new();

    [SerializeField] private GameObject lineUIPrefab;
    [SerializeField] private float lineYOffset = 10f;
    [SerializeField] private float lineThickness = 3f;

    private Dictionary<string, ClueNode> spawnedNodes = new();
    private List<GameObject> lineasInstanciadas = new();

    [SerializeField] private RectTransform playArea;

    private Dictionary<string, List<string>> dynamicConnections = new Dictionary<string, List<string>>();

    private GameObject previewLineGO;
    private RectTransform previewLineRect;

    private float qHeldTime = 0f;
    [SerializeField] private float disconnectHoldSeconds = 2f;

    public enum ClueMode { Normal, Connect, DeleteConnections }
    public ClueMode currentMode = ClueMode.Normal;
    public ClueNode selectedNode; // ya no se usa

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            qHeldTime += Time.unscaledDeltaTime;
            if (qHeldTime >= disconnectHoldSeconds)
            {
                DesconectarTodasLasConexiones();
                qHeldTime = 0f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            qHeldTime = 0f;
        }
    }

    public void AbrirCorcho()
    {
        boardParent.gameObject.SetActive(true);
        RegenerarBoard();
    }

    public void CerrarCorcho()
    {
        boardParent.gameObject.SetActive(false);
        LimpiarBoard();
    }

    public void OpenCulpables()
    {
        Corchoculpable.gameObject.SetActive(true);
        boardParent.gameObject.SetActive(false);
    }

    public void ClosedCulpables()
    {
        Corchoculpable.gameObject.SetActive(false);
        boardParent.gameObject.SetActive(true);
        RegenerarBoard();
    }

    private void RegenerarBoard()
    {

        LimpiarBoard();

        PlayerClueTracker.Instance.clues = new HashSet<string>(PlayerClueTracker.Instance.cluesList);

        bool hayPistas = false;
        foreach (var clue in allClues)
        {
            if (PlayerClueTracker.Instance.HasClue(clue.clueID))
            {
                hayPistas = true;
                break;
            }
        }
        if (!hayPistas) return;

        spawnedNodes.Clear();

        foreach (var clue in allClues)
        {
            bool isFound = PlayerClueTracker.Instance.HasClue(clue.clueID);

            float x = PlayerPrefs.GetFloat(clue.clueID + "_x", clue.boardPosition.x);
            float y = PlayerPrefs.GetFloat(clue.clueID + "_y", clue.boardPosition.y);
            Vector2 startPos = new Vector2(x, y);

            var node = Instantiate(cluePrefab, boardParent);
            node.Init(clue, isFound, playArea, startPos);
            node.BindBoard(this);

            spawnedNodes[clue.clueID] = node;
            nodosInstanciados.Add(node.gameObject);
        }

        RecalcularLineas();
    }

    public void AddConnection(string fromID, string toID)
    {
        if (string.IsNullOrEmpty(fromID) || string.IsNullOrEmpty(toID) || fromID == toID)
            return;

        if (!dynamicConnections.ContainsKey(fromID))
            dynamicConnections[fromID] = new List<string>();
        if (!dynamicConnections[fromID].Contains(toID))
            dynamicConnections[fromID].Add(toID);

        if (!dynamicConnections.ContainsKey(toID))
            dynamicConnections[toID] = new List<string>();
        if (!dynamicConnections[toID].Contains(fromID))
            dynamicConnections[toID].Add(fromID);

        RecalcularLineas();
    }

    public void RemoveConnection(string fromID, string toID)
    {
        bool changed = false;

        if (dynamicConnections.ContainsKey(fromID) && dynamicConnections[fromID].Remove(toID)) changed = true;
        if (dynamicConnections.ContainsKey(toID) && dynamicConnections[toID].Remove(fromID)) changed = true;

        if (spawnedNodes.TryGetValue(fromID, out var fromNode) &&
            fromNode.data.connectedClues.Remove(toID)) changed = true;

        if (spawnedNodes.TryGetValue(toID, out var toNode) &&
            toNode.data.connectedClues.Remove(fromID)) changed = true;

        if (changed) RecalcularLineas();
    }

    public void RecalcularLineas()
    {
        foreach (var linea in lineasInstanciadas) Destroy(linea);
        lineasInstanciadas.Clear();

        foreach (var clue in allClues)
        {
            if (!spawnedNodes.ContainsKey(clue.clueID)) continue;

            var fromNode = spawnedNodes[clue.clueID];

            if (clue.connectedClues != null)
            {
                foreach (var targetID in clue.connectedClues)
                {
                    TryDrawConnection(fromNode, targetID);
                }
            }

            if (dynamicConnections.TryGetValue(clue.clueID, out var dynTargets))
            {
                foreach (var targetID in dynTargets)
                {
                    TryDrawConnection(fromNode, targetID);
                }
            }
        }
    }

    private void TryDrawConnection(ClueNode fromNode, string targetID)
    {
        if (!spawnedNodes.ContainsKey(targetID)) return;
        var toNode = spawnedNodes[targetID];

        // Evito duplicados: solo dibujo si el ID de origen es "menor" que el destino
        if (string.CompareOrdinal(fromNode.data.clueID, targetID) > 0) return;

        GameObject linea = Instantiate(lineUIPrefab, boardParent);
        RectTransform lineRect = linea.GetComponent<RectTransform>();

        Vector2 start = fromNode.RectTransform.anchoredPosition;
        Vector2 end = toNode.RectTransform.anchoredPosition;

        start.y += lineYOffset;
        end.y += lineYOffset;

        ApplyLineGeometry(lineRect, start, end);
        lineRect.SetAsFirstSibling();
        lineasInstanciadas.Add(linea);
    }

    private void ApplyLineGeometry(RectTransform lineRect, Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        lineRect.sizeDelta = new Vector2(distance, lineThickness);
        lineRect.anchoredPosition = start + direction / 2f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void LimpiarBoard()
    {
        foreach (var obj in nodosInstanciados) Destroy(obj);
        nodosInstanciados.Clear();

        foreach (var linea in lineasInstanciadas) Destroy(linea);
        lineasInstanciadas.Clear();

        // oculto preview si quedó viva
        HidePreviewLine();
    }

    public ClueNode GetNodeByID(string clueID)
    {
        spawnedNodes.TryGetValue(clueID, out ClueNode node);
        return node;
    }


    public void ShowPreviewFromNodeToScreen(ClueNode fromNode, Vector2 screenPos, Camera uiCam)
    {
        if (previewLineGO == null)
        {
            previewLineGO = Instantiate(lineUIPrefab, boardParent);
            previewLineRect = previewLineGO.GetComponent<RectTransform>();
            //previewVisible = true;
        }

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardParent, screenPos, uiCam, out localPoint);

        Vector2 start = fromNode.RectTransform.anchoredPosition;
        Vector2 end = localPoint;

        start.y += lineYOffset;
        end.y += lineYOffset;

        ApplyLineGeometry(previewLineRect, start, end);
        previewLineRect.SetAsFirstSibling();
    }

    public void HidePreviewLine()
    {
        //previewVisible = false;
        if (previewLineGO != null)
        {
            Destroy(previewLineGO);
            previewLineGO = null;
            previewLineRect = null;
        }
    }

    private void DesconectarTodasLasConexiones()
    {
        dynamicConnections.Clear();

        foreach (var kv in spawnedNodes)
        {
            kv.Value.data.connectedClues.Clear();
        }

        RecalcularLineas();
    }
}
