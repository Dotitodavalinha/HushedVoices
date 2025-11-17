using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClueBoardManager : MonoBehaviour
{
    [System.Serializable]
    public class CaseContainers
    {
        public string caseName = "Case";
        public GameObject panel;
        public RectTransform playArea;
        public Transform linesContainer;
        public RectTransform miniCorchoArea;
        public Transform miniCorchoLines;
    }

    [Header("Configuración General")]
    public List<ClueNode> clueNodes;
    [SerializeField] private GameObject clueBoard;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private FolioManager folioManager;

    [Header("Configuración de Casos")]
    [Tooltip("Aquí se configuran todos los paneles de casos y sus mini-corchos")]
    [SerializeField] private List<CaseContainers> caseData = new List<CaseContainers>();

    [Header("Basura")]
    [SerializeField] private List<BrokenClueCleaner> brokenClues = new List<BrokenClueCleaner>();

    [Header("Líneas")]
    [SerializeField] private GameObject lineUIPrefab;
    [SerializeField] private float lineThickness = 8f;
    private List<GameObject> lines = new();

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

    private GameObject activePanel;
    private Transform currentLinesContainer;
    private RectTransform currentPlayArea;
    private Dictionary<string, List<string>> dynamicConnections = new Dictionary<string, List<string>>();
    private GameObject previewLineGO;
    private RectTransform previewLineRect;
    private float qHeldTime = 0f;
    [SerializeField] private float disconnectHoldSeconds = 2f;
    [SerializeField] private float lineYOffset = 10f;

    public bool IsOnMainMenu { get { return activePanel == mainMenuPanel; } }

    #region Eventos y Ciclo de Vida (Unity)

    private void Awake()
    {
        Cursor.visible = false;
        foreach (var node in clueNodes)
        {
            if (node == null) continue;
            node.BindBoard(this);
            node.SetFound(PlayerClueTracker.Instance.HasClue(node.data.clueID));
        }
    }

    private void OnEnable()
    {
        PlayerClueTracker.OnCluesAdded += HandleCluesAdded;
    }

    private void OnDisable()
    {
        PlayerClueTracker.OnCluesAdded -= HandleCluesAdded;
    }

    private void Update()
    {
        if (clueBoard.activeSelf)
        {
            HandleDisconnectHold();
        }
    }

    #endregion

    #region Lógica Principal (Abrir/Cerrar/Refrescar)

    public void OpenBoard()
    {
        ChangeCursor(hover);
        clueBoard.SetActive(true);

        if (folioManager != null)
        {
            folioManager.ShowFolioIfReady();
        }

        ShowPanel(mainMenuPanel);

        StartCoroutine(RefreshBoardDelayed());
    }

    private IEnumerator RefreshBoardDelayed()
    {
        yield return null;

        RefreshBoard();
        RefreshAllMiniCorchoLines();
    }

    public void CloseBoard()
    {
        if (activePanel != null && activePanel != mainMenuPanel)
        {
            ReturnCluesToMainMenu();
        }

        if (activePanel != null)
            activePanel.SetActive(false);

        mainMenuPanel.SetActive(false);

        if (folioManager != null)
        {
            folioManager.HideFolio();
        }

        cursor.CursorSpriteChange(defaultCursor, new Vector2(1, 1));
        clueBoard.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetBlockEInput(false);
            GameManager.Instance.UnlockUI();
        }

        FindObjectOfType<CorchoInteract>()?.SetUIState(false);
    }

    public void RefreshBoard()
    {
        dynamicConnections.Clear();
        foreach (var node in clueNodes)
        {
            if (node == null)
            {
                Debug.LogWarning("ClueBoardManager: Se encontró un slot nulo en la lista clueNodes. Saltando...");
                continue;
            }

            if (PlayerPrefs.HasKey(node.data.clueID + "_parent"))
            {
                string parentName = PlayerPrefs.GetString(node.data.clueID + "_parent");
                Transform parent = GameObject.Find(parentName)?.transform;
                if (parent != null)
                {
                    node.MoveToCorcho(parent as RectTransform, this);
                }
            }

            if (PlayerPrefs.HasKey(node.data.clueID + "_x"))
            {
                float x = PlayerPrefs.GetFloat(node.data.clueID + "_x");
                float y = PlayerPrefs.GetFloat(node.data.clueID + "_y");

                if (node.RectTransform != null)
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

            if (node.RectTransform == null)
            {
                Debug.LogError($"ClueBoardManager: El RectTransform de '{node.name}' es nulo. Problema de orden de ejecución.");
            }
            else
            {
                node.RectTransform.localScale = Vector3.one;
            }
        }

        // Bucle para cargar la "basura"
        foreach (var basura in brokenClues)
        {
            if (basura == null) continue;

            string uniqueID = string.IsNullOrEmpty(basura.BrokenClueID) ? basura.gameObject.name : basura.BrokenClueID;

            if (string.IsNullOrEmpty(uniqueID)) continue;

            string key = uniqueID + "_parent";
            if (PlayerPrefs.HasKey(key))
            {
                string parentName = PlayerPrefs.GetString(key);
                Transform parent = GameObject.Find(parentName)?.transform;
                if (parent != null)
                {
                    basura.MoveToCorcho(parent as RectTransform);
                }
            }
        }
    }

    #endregion

    #region Lógica de Navegación (Zoom)

    public void ZoomToPanel(int panelIndex)
    {
        int index = panelIndex - 1;
        if (index < 0 || index >= caseData.Count)
        {
            Debug.LogError($"Índice de panel inválido: {panelIndex}");
            return;
        }

        CaseContainers data = caseData[index];

        TransferChildren(data.miniCorchoArea, data.playArea);
        TransferChildren(data.miniCorchoLines, data.linesContainer);
        SetActiveContainers(data.playArea, data.linesContainer);
        ShowPanel(data.panel);
        RecalculateLines();
    }

    public void ZoomToMainMenu()
    {
        if (activePanel == null || activePanel == mainMenuPanel) return;

        ReturnCluesToMainMenu();
        ShowPanel(mainMenuPanel);
        RefreshAllMiniCorchoLines();
    }

    private void ReturnCluesToMainMenu()
    {
        if (activePanel == null || activePanel == mainMenuPanel) return;

        CaseContainers data = FindCaseDataByPanel(activePanel);
        if (data == null) return;

        TransferChildren(data.playArea, data.miniCorchoArea);
        TransferChildren(data.linesContainer, data.miniCorchoLines);
        SetActiveContainers(data.miniCorchoArea, data.miniCorchoLines);
    }

    private CaseContainers FindCaseDataByPanel(GameObject panel)
    {
        return caseData.Find(c => c.panel == panel);
    }

    #endregion

    #region Métodos Ayudantes (Paneles, Hijos, Contenedores)

    private void ShowPanel(GameObject panelToShow)
    {
        mainMenuPanel.SetActive(false);

        foreach (var data in caseData)
        {
            if (data.panel != null)
                data.panel.SetActive(false);
        }

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }

        activePanel = panelToShow;
    }

    private void SetActiveContainers(RectTransform playArea, Transform linesContainer)
    {
        currentPlayArea = playArea;
        currentLinesContainer = linesContainer;
    }

    private void TransferChildren(Transform fromParent, Transform toParent)
    {
        if (fromParent == null || toParent == null) return;

        for (int i = fromParent.childCount - 1; i >= 0; i--)
        {
            Transform child = fromParent.GetChild(i);

            bool isClue = child.GetComponent<ClueNode>() != null;
            bool isLine = child.GetComponent<ClueLine>() != null;
            bool isBasura = child.GetComponent<BrokenClueCleaner>() != null;

            if (!isClue && !isLine && !isBasura)
            {
                continue;
            }

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

            if (isClue)
            {
                child.GetComponent<ClueNode>().SaveState();
            }
            else if (isBasura)
            {
                child.GetComponent<BrokenClueCleaner>().SaveState();
            }
        }
    }

    #endregion

    #region Lógica de Líneas (Dibujo y Conexiones)

    public void RecalculateLines(bool clearLines = true)
    {
        if (clearLines)
        {
            foreach (var l in lines) Destroy(l);
            lines.Clear();
        }

        if (currentLinesContainer == null) return;

        foreach (var node in clueNodes)
        {
            if (node == null || node.transform.parent != currentPlayArea) continue;

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

    private void RefreshAllMiniCorchoLines()
    {
        foreach (var l in lines) Destroy(l);
        lines.Clear();

        foreach (var data in caseData)
        {
            SetActiveContainers(data.miniCorchoArea, data.miniCorchoLines);
            RecalculateLines(false);
        }

        SetActiveContainers(null, null);
    }

    private void DrawLine(ClueNode from, ClueNode to)
    {
        if (from.transform.parent != currentPlayArea || to.transform.parent != currentPlayArea)
            return;

        if (string.CompareOrdinal(from.data.clueID, to.data.clueID) > 0) return;

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

    #endregion

    #region Eventos y Handlers (Input, Tracker)

    private void HandleCluesAdded(List<string> addedClueIDs)
    {
        if (clueNodes == null) return;

        foreach (string clueID in addedClueIDs)
        {
            ClueNode node = clueNodes.Find(n => n.data.clueID == clueID);
            if (node != null)
            {
                node.SetFound(true);
            }
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

    public void ShowPreviewFromNodeToScreen(ClueNode fromNode, Vector2 screenPos, Camera uiCam)
    {
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
            if (node == null) continue;
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

    #endregion
}