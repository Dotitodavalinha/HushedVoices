using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClueBoardManager : MonoBehaviour
{
    [Header("Nodes")]
    public List<ClueNode> clueNodes;
    [SerializeField] private GameObject clueBoard;
    [SerializeField] private RectTransform playAreaObject;

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

    [Header("Cursor")]
    public CursorChange cursor;
    public Texture2D defaultCursor;
    public Texture2D grab;
    public Texture2D hover;
    public Texture2D connect;
    public Texture2D disconnect;
    public Texture2D zoomIn;
    public GameObject culpablesPanel;



    private void Awake()
    {
        Cursor.visible=false;

        foreach (var node in clueNodes)
        {
            node.BindBoard(this, playAreaObject);
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
            ChangeCursor(disconnect);
            qHeldTime += Time.unscaledDeltaTime;
            if (qHeldTime >= disconnectHoldSeconds)
            {
                ChangeCursor(hover);
                DisconnectAll();
                qHeldTime = 0f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            ChangeCursor(hover);
            qHeldTime = 0f;
        }
    }

    public void OpenBoard()
    {
        ChangeCursor(hover);
        clueBoard.SetActive(true);
        RefreshBoard();
    }

    public void CloseBoard()
    {
        cursor.CursorSpriteChange(defaultCursor, new Vector2(1,1));
        clueBoard.SetActive(false);
    }

    public void RefreshBoard()
    {
        foreach (var node in clueNodes)
        {
            if (PlayerPrefs.HasKey(node.data.clueID + "_parent"))
            {
                string parentName = PlayerPrefs.GetString(node.data.clueID + "_parent");
                Transform parent = GameObject.Find(parentName)?.transform;
                if (parent != null)
                    node.MoveToCorcho(parent as RectTransform);
            }

            if (PlayerPrefs.HasKey(node.data.clueID + "_x"))
            {
                float x = PlayerPrefs.GetFloat(node.data.clueID + "_x");
                float y = PlayerPrefs.GetFloat(node.data.clueID + "_y");
                node.RectTransform.anchoredPosition = new Vector2(x, y);
            }

            string key = "dyn_" + node.data.clueID;
            if (PlayerPrefs.HasKey(key))
            {
                string val = PlayerPrefs.GetString(key);
                node.data.connectedClues = new List<string>(val.Split(',', System.StringSplitOptions.RemoveEmptyEntries));
            }

            node.SetFound(PlayerClueTracker.Instance.HasClue(node.data.clueID));
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
        SaveDynamicConnections();
    }

    public void RemoveConnection(string fromID, string toID)
    {
        bool changed = false;

        if (dynamicConnections.ContainsKey(fromID) && dynamicConnections[fromID].Remove(toID)) changed = true;
        if (dynamicConnections.ContainsKey(toID) && dynamicConnections[toID].Remove(fromID)) changed = true;

        if (changed) RecalculateLines();
        SaveDynamicConnections();
    }
    private void SaveDynamicConnections()
    {
        foreach (var kvp in dynamicConnections)
        {
            string key = "dyn_" + kvp.Key;
            string val = string.Join(",", kvp.Value);
            PlayerPrefs.SetString(key, val);
        }
        PlayerPrefs.Save();
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
        if (string.CompareOrdinal(from.data.clueID, to.data.clueID) > 0) return;

        GameObject line = Instantiate(lineUIPrefab, clueBoard.transform);
        RectTransform rect = line.GetComponent<RectTransform>();

        Vector2 start = from.RectTransform.anchoredPosition;
        Vector2 end = to.RectTransform.anchoredPosition;

        float fromHeight = from.RectTransform.rect.height;
        float toHeight = to.RectTransform.rect.height;

        start.y += fromHeight / 2f;
        end.y += toHeight / 2f;

        ApplyLineGeometry(rect, start, end);
        rect.SetAsFirstSibling();
        lines.Add(line);

        var clueLine = line.AddComponent<ClueLine>();
        clueLine.Setup(this, from.data.clueID, to.data.clueID);
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
        cursor.CursorSpriteChange(connect, new Vector2(1, 1));
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
        cursor.CursorSpriteChange(hover, new Vector2(32, 32));
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

    public void ChangeCursor(Texture2D texture)
    {
        cursor.CursorSpriteChange(texture, new Vector2(32, 32));
    }
}
