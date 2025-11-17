using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClueBoardManager : MonoBehaviour
{
    [Header("Nodes")]
    public List<ClueNode> clueNodes;
    [SerializeField] private GameObject clueBoard;

    // ESTO ES NUEVO: Referencias a los paneles
    [Header("Zoom Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject case1Panel;
    [SerializeField] private GameObject case2Panel;
    [SerializeField] private GameObject case3Panel;
    [SerializeField] private GameObject case4Panel;
    private GameObject activePanel;

    // ESTO CAMBIÓ: Ya no es una variable, se asigna dinámicamente
    private Transform currentLinesContainer;
    private RectTransform currentPlayArea;

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
    public Texture2D deleteClues;
    public GameObject culpablesPanel;


    [Header("Panel Containers")]
    [SerializeField] private RectTransform case1PlayArea;
    [SerializeField] private Transform case1Lines;
    [SerializeField] private RectTransform case2PlayArea;
    [SerializeField] private Transform case2Lines;
    [SerializeField] private RectTransform case3PlayArea;
    [SerializeField] private Transform case3Lines;
    [SerializeField] private RectTransform case4PlayArea;
    [SerializeField] private Transform case4Lines;

    [Header("Mini Corcho Containers")]
    [SerializeField] private RectTransform miniCorcho1Area;
    [SerializeField] private Transform miniCorcho1Lines;
    [SerializeField] private RectTransform miniCorcho2Area;
    [SerializeField] private Transform miniCorcho2Lines;
    [SerializeField] private RectTransform miniCorcho3Area;
    [SerializeField] private Transform miniCorcho3Lines;
    [SerializeField] private RectTransform miniCorcho4Area;
    [SerializeField] private Transform miniCorcho4Lines;

    private void Awake()
    {
        // ESTO CAMBIÓ: Ya no busca 'playAreaObject' o 'linesContainer'
        // Se asignarán cuando se abra un panel.

        Cursor.visible = false;

        foreach (var node in clueNodes)
        {
            // ESTO CAMBIÓ: BindBoard ahora es más simple
            node.BindBoard(this);
            node.SetFound(PlayerClueTracker.Instance.HasClue(node.data.clueID));
        }
    }

    private void Update()
    {
        if (clueBoard.activeSelf)
        {
            HandleDisconnectHold();
        }
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

        // ESTO ES NUEVO: Al abrir, mostramos el menú principal y refrescamos.
        // RefreshBoard pondrá todo en su mini-corcho guardado.
        ShowPanel(mainMenuPanel);
        RefreshBoard();
    }

    public void CloseBoard()
    {
        cursor.CursorSpriteChange(defaultCursor, new Vector2(1, 1));
        clueBoard.SetActive(false);
    }

    public void RefreshBoard()
    {
        dynamicConnections.Clear();

        foreach (var node in clueNodes)
        {
            if (PlayerPrefs.HasKey(node.data.clueID + "_parent"))
            {
                string parentName = PlayerPrefs.GetString(node.data.clueID + "_parent");
                Transform parent = GameObject.Find(parentName)?.transform;
                if (parent != null)
                {
                    // ESTO CAMBIÓ: Usa la nueva función MoveToCorcho
                    node.MoveToCorcho(parent as RectTransform, this);
                }
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
                List<string> targets = new List<string>(val.Split(',', System.StringSplitOptions.RemoveEmptyEntries));
                dynamicConnections[node.data.clueID] = targets;
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

        // ESTO ES NUEVO: Si no hay un contenedor de líneas activo, no dibujes.
        if (currentLinesContainer == null) return;

        foreach (var node in clueNodes)
        {
            // ESTO ES NUEVO: Solo dibuja líneas para nodos en el área activa
            if (node.transform.parent != currentPlayArea) continue;

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
        // ESTO ES NUEVO: Asegurarse que ambos nodos están en el área activa
        if (from.transform.parent != currentPlayArea || to.transform.parent != currentPlayArea)
            return;

        if (string.CompareOrdinal(from.data.clueID, to.data.clueID) > 0) return;

        // ESTO CAMBIÓ: Usa el 'currentLinesContainer'
        GameObject line = Instantiate(lineUIPrefab, currentLinesContainer);
        RectTransform rect = line.GetComponent<RectTransform>();

        Vector2 start = from.RectTransform.anchoredPosition;
        Vector2 end = to.RectTransform.anchoredPosition;

        float fromHeight = from.RectTransform.rect.height;
        float toHeight = to.RectTransform.rect.height;

        start.y += fromHeight / 2f;
        end.y += toHeight / 2f;

        ApplyLineGeometry(rect, start, end);
        rect.SetAsLastSibling();
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
        // ESTO ES NUEVO: Chequeo de seguridad
        if (currentLinesContainer == null) return;

        cursor.CursorSpriteChange(connect, new Vector2(1, 1));
        if (previewLineGO == null)
        {
            previewLineGO = Instantiate(lineUIPrefab, currentLinesContainer);
            previewLineRect = previewLineGO.GetComponent<RectTransform>();
        }

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(clueBoard.GetComponent<RectTransform>(), screenPos, uiCam, out localPoint);

        Vector2 start = fromNode.RectTransform.anchoredPosition;
        Vector2 end = localPoint;

        start.y += fromNode.RectTransform.rect.height / 2 + lineYOffset;
        end.y += lineYOffset;

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

        foreach (var node in clueNodes)
        {
            node.data.connectedClues.Clear();

            string key = "dyn_" + node.data.clueID;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        PlayerPrefs.Save();
        RecalculateLines();
    }

    public void OpenCulpables()
    {
        culpablesPanel.SetActive(true);
        clueBoard.SetActive(false);
        GameManager.Instance.TryLockUI();
        GameManager.Instance.SetBlockEInput(true);
        FindObjectOfType<CorchoInteract>()?.SetUIState(true);
    }

    public void CloseCulpables()
    {
        culpablesPanel.SetActive(false);
        clueBoard.SetActive(true);
        GameManager.Instance.SetBlockEInput(false);
    }


    public void ChangeCursor(Texture2D texture)
    {
        cursor.CursorSpriteChange(texture, new Vector2(32, 32));
    }


    // --- TODA ESTA SECCIÓN ES NUEVA ---
    // --- LÓGICA DE ZOOM Y NAVEGACIÓN ---

    // Esta función reemplaza al MenuManager
    private void ShowPanel(GameObject panelToShow)
    {
        mainMenuPanel.SetActive(false);
        case1Panel.SetActive(false);
        case2Panel.SetActive(false);
        case3Panel.SetActive(false);
        case4Panel.SetActive(false);

        panelToShow.SetActive(true);
        activePanel = panelToShow;
    }

    private void SetActiveContainers(RectTransform playArea, Transform linesContainer)
    {
        currentPlayArea = playArea;
        currentLinesContainer = linesContainer;
    }

    // (DENTRO DE ClueBoardManager.cs)

    private void TransferChildren(Transform fromParent, Transform toParent)
    {
        if (fromParent == null || toParent == null) return;

        for (int i = fromParent.childCount - 1; i >= 0; i--)
        {
            Transform child = fromParent.GetChild(i);

            // --- ESTE ES EL NUEVO CÓDIGO ---
            // Revisa si el hijo es una pista (ClueNode) o una línea (ClueLine)
            // (Necesitás tener un script 'ClueLine' en tu prefab de línea)
            bool isClue = child.GetComponent<ClueNode>() != null;
            bool isLine = child.GetComponent<ClueLine>() != null;

            // Si NO es una pista Y NO es una línea, se saltea este hijo.
            // Esto evita que mueva tu botón "Volver".
            if (!isClue && !isLine)
            {
                continue; // Saltar al siguiente hijo
            }
            // --- FIN DEL NUEVO CÓDIGO ---

            Vector2 savedPos = Vector2.zero;
            Vector3 savedScale = Vector3.one;
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                savedPos = rt.anchoredPosition;
                savedScale = rt.localScale;
            }

            child.SetParent(toParent, false);

            if (rt != null)
            {
                rt.anchoredPosition = savedPos;
                rt.localScale = savedScale;
            }

            if (isClue) // Solo guarda el estado si era una pista
            {
                child.GetComponent<ClueNode>().SaveState();
            }
        }
    }

    public void ZoomToPanel(int panelIndex)
    {
        GameObject panelToShow = null;
        RectTransform targetPlayArea = null;
        Transform targetLines = null;
        RectTransform sourceMiniArea = null;
        Transform sourceMiniLines = null;

        switch (panelIndex)
        {
            case 1:
                panelToShow = case1Panel;
                targetPlayArea = case1PlayArea;
                targetLines = case1Lines;
                sourceMiniArea = miniCorcho1Area;
                sourceMiniLines = miniCorcho1Lines;
                break;
            case 2:
                panelToShow = case2Panel;
                targetPlayArea = case2PlayArea;
                targetLines = case2Lines;
                sourceMiniArea = miniCorcho2Area;
                sourceMiniLines = miniCorcho2Lines;
                break;
            case 3:
                panelToShow = case3Panel;
                targetPlayArea = case3PlayArea;
                targetLines = case3Lines;
                sourceMiniArea = miniCorcho3Area;
                sourceMiniLines = miniCorcho3Lines;
                break;
            case 4:
                panelToShow = case4Panel;
                targetPlayArea = case4PlayArea;
                targetLines = case4Lines;
                sourceMiniArea = miniCorcho4Area;
                sourceMiniLines = miniCorcho4Lines;
                break;
        }

        if (panelToShow == null) return;

        TransferChildren(sourceMiniArea, targetPlayArea);
        TransferChildren(sourceMiniLines, targetLines);
        SetActiveContainers(targetPlayArea, targetLines);
        ShowPanel(panelToShow);
        RecalculateLines();
    }

    public void ZoomToMainMenu()
    {
        if (activePanel == null || activePanel == mainMenuPanel) return;

        RectTransform sourcePlayArea = null;
        Transform sourceLines = null;
        RectTransform targetMiniArea = null;
        Transform targetMiniLines = null;

        if (activePanel == case1Panel) { sourcePlayArea = case1PlayArea; sourceLines = case1Lines; targetMiniArea = miniCorcho1Area; targetMiniLines = miniCorcho1Lines; }
        else if (activePanel == case2Panel) { sourcePlayArea = case2PlayArea; sourceLines = case2Lines; targetMiniArea = miniCorcho2Area; targetMiniLines = miniCorcho2Lines; }
        else if (activePanel == case3Panel) { sourcePlayArea = case3PlayArea; sourceLines = case3Lines; targetMiniArea = miniCorcho3Area; targetMiniLines = miniCorcho3Lines; }
        else if (activePanel == case4Panel) { sourcePlayArea = case4PlayArea; sourceLines = case4Lines; targetMiniArea = miniCorcho4Area; targetMiniLines = miniCorcho4Lines; }

        if (sourcePlayArea == null) return;

        TransferChildren(sourcePlayArea, targetMiniArea);
        TransferChildren(sourceLines, targetMiniLines);
        SetActiveContainers(targetMiniArea, targetMiniLines);
        ShowPanel(mainMenuPanel);
        RecalculateLines();
    }
}