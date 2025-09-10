using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClueBoardManager : MonoBehaviour
{
    [Header("Nodes")]
    [SerializeField] private List<ClueNode> clueNodes;   // nodos ya puestos en el editor
    [SerializeField] private GameObject clueBoard;

    [Header("Lines")]
    [SerializeField] private GameObject lineUIPrefab;
    [SerializeField] private float lineYOffset = 10f;
    [SerializeField] private float lineThickness = 8f;
    private List<GameObject> lines = new();

    private Dictionary<string, List<string>> dynamicConnections = new Dictionary<string, List<string>>();
    private GameObject previewLineGO;
    private RectTransform previewLineRect;

    [Header("Drag/Disconnect")]
    [SerializeField] private float disconnectHoldSeconds = 2f;
    private float qHeldTime = 0f;

    [SerializeField] private GameObject culpablesPanel;

    private void Awake()
    {
        foreach (var node in clueNodes)
        {
            node.BindBoard(this, clueBoard.GetComponent<RectTransform>());
            node.SetFound(PlayerClueTracker.Instance.HasClue(node.data.clueID));
        }

    }

    private void Update()
    {
        HandleDisconnectHold();
    }

    private void HandleDisconnectHold()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            qHeldTime += Time.unscaledDeltaTime;
            if (qHeldTime >= disconnectHoldSeconds)
            {
                DisconnectAll();
                qHeldTime = 0f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            qHeldTime = 0f;
        }
    }

    public void OpenBoard()
    {
        clueBoard.SetActive(true);
        RefreshBoard();
    }

    public void CloseBoard()
    {
        clueBoard.SetActive(false);
    }

    public void RefreshBoard()
    {
        foreach (var node in clueNodes)
        {
            bool found = PlayerClueTracker.Instance.HasClue(node.data.clueID);
            node.SetFound(found);
        }

        RecalculateLines();
    }

    public void AddConnection(string fromID, string toID)
    {
        if (string.IsNullOrEmpty(fromID) || string.IsNullOrEmpty(toID) || fromID == toID) return;

        if (!dynamicConnections.ContainsKey(fromID))
            dynamicConnections[fromID] = new List<string>();
        if (!dynamicConnections[fromID].Contains(toID))
            dynamicConnections[fromID].Add(toID);

        if (!dynamicConnections.ContainsKey(toID))
            dynamicConnections[toID] = new List<string>();
        if (!dynamicConnections[toID].Contains(fromID))
            dynamicConnections[toID].Add(fromID);

        RecalculateLines();
    }

    public void RemoveConnection(string fromID, string toID)
    {
        bool changed = false;

        if (dynamicConnections.ContainsKey(fromID) && dynamicConnections[fromID].Remove(toID)) changed = true;
        if (dynamicConnections.ContainsKey(toID) && dynamicConnections[toID].Remove(fromID)) changed = true;

        if (changed) RecalculateLines();
    }

    public void RecalculateLines()
    {
        foreach (var l in lines) Destroy(l);
        lines.Clear();

        foreach (var node in clueNodes)
        {
            foreach (var targetID in node.data.connectedClues)
            {
                ClueNode target = clueNodes.Find(n => n.data.clueID == targetID);
                if (target != null) DrawLine(node, target);
            }

            if (dynamicConnections.TryGetValue(node.data.clueID, out var dynTargets))
            {
                foreach (var targetID in dynTargets)
                {
                    ClueNode target = clueNodes.Find(n => n.data.clueID == targetID);
                    if (target != null) DrawLine(node, target);
                }
            }
        }
    }

    private void DrawLine(ClueNode from, ClueNode to)
    {
        // Evito duplicados
        if (string.CompareOrdinal(from.data.clueID, to.data.clueID) > 0) return;

        GameObject line = Instantiate(lineUIPrefab, clueBoard.transform);
        RectTransform rect = line.GetComponent<RectTransform>();

        Vector2 start = from.RectTransform.anchoredPosition;
        Vector2 end = to.RectTransform.anchoredPosition;

        // mover al pin de arriba de cada nodo
        float fromHeight = from.RectTransform.rect.height;
        float toHeight = to.RectTransform.rect.height;

        start.y += fromHeight / 2f;
        end.y += toHeight / 2f;

        ApplyLineGeometry(rect, start, end);
        rect.SetAsFirstSibling();
        lines.Add(line);
    }


    private void ApplyLineGeometry(RectTransform lineRect, Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        float dist = dir.magnitude;
        lineRect.sizeDelta = new Vector2(dist, lineThickness);
        lineRect.anchoredPosition = start + dir / 2f;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ShowPreviewFromNodeToScreen(ClueNode fromNode, Vector2 screenPos, Camera uiCam)
    {
        if (previewLineGO == null)
        {
            previewLineGO = Instantiate(lineUIPrefab, clueBoard.transform);
            previewLineRect = previewLineGO.GetComponent<RectTransform>();
        }

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(clueBoard.GetComponent<RectTransform>(), screenPos, uiCam, out localPoint);

        // La línea empieza desde la parte superior central del nodo
        Vector2 start = fromNode.RectTransform.anchoredPosition;
        Vector2 end = localPoint;

        start.y += fromNode.RectTransform.rect.height / 2 + lineYOffset;
        end.y += lineYOffset; // el punto de destino queda con offset solo

        ApplyLineGeometry(previewLineRect, start, end);
        previewLineRect.SetAsFirstSibling();
    }

    public void HidePreviewLine()
    {
        if (previewLineGO != null)
        {
            Destroy(previewLineGO);
            previewLineGO = null;
            previewLineRect = null;
        }
    }

    private void DisconnectAll()
    {
        dynamicConnections.Clear();
        RecalculateLines();
    }

    public void OpenCulpables()
    {
        culpablesPanel.SetActive(true);
        clueBoard.SetActive(false);
    }

    public void CloseCulpables()
    {
        culpablesPanel.SetActive(false);
        clueBoard.SetActive(true);
    }
}
