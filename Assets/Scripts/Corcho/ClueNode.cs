using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClueNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ClueData data;

    private RectTransform _rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private Vector2 originalPosition;

    private bool isLeftDragging = false;
    private float leftPressTime = 0f;
    private Vector2 leftPressPos;
    private const float clickTimeThreshold = 0.25f;
    private const float clickMoveThreshold = 8f;

    private bool isRightDragging = false;

    private ClueBoardManager board;

    public GameObject clueVisual;
    private Image clueVisualImage;
    private Color originalColor;

    public bool isDefaultClue = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (clueVisual != null)
        {
            clueVisualImage = clueVisual.GetComponent<Image>();
            if (clueVisualImage != null)
            {
                originalColor = clueVisualImage.color;
            }
        }
    }

    public void BindBoard(ClueBoardManager b)
    {
        if (b != null)
        {
            board = b;
        }
    }

    public void SetFound(bool found)
    {
        if (isDefaultClue)
        {
            found = true;
        }

        if (clueVisual != null)
        {
            clueVisual.SetActive(found);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board == null) return;
        if (board.IsOnMainMenu) board.ChangeCursor(board.hover);
        else board.ChangeCursor(board.zoomIn);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        board?.ChangeCursor(board.hover);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (board != null && board.IsOnMainMenu) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isLeftDragging = false;
            leftPressTime = Time.unscaledTime;
            leftPressPos = eventData.position;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightDragging = true;
            board?.ShowPreviewFromNodeToScreen(this, eventData.position, eventData.pressEventCamera);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (board != null && board.IsOnMainMenu) return;

        if (eventData.button == PointerEventData.InputButton.Right && isRightDragging)
        {
            isRightDragging = false;
            if (board != null)
            {
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                ClueNode target = null;
                foreach (var r in results)
                {
                    target = r.gameObject.GetComponentInParent<ClueNode>();
                    if (target != null && target != this) break;
                }

                if (target != null)
                {
                    board.AddConnection(this.data.clueID, target.data.clueID);
                    SoundManager.instance.PlaySound(SoundID.HiloCorcho, false, 1f, 5.5f);
                }
                board.HidePreviewLine();
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left && !isLeftDragging)
        {
            float elapsed = Time.unscaledTime - leftPressTime;
            float moved = (eventData.position - leftPressPos).magnitude;

            if (elapsed <= clickTimeThreshold && moved <= clickMoveThreshold)
            {
                ClueZoomUI.Instance.ShowClue(data);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (board != null && board.IsOnMainMenu)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SoundManager.instance.PlaySound(SoundID.ClueFromFolder, false, 1f, 5.5f);
            isLeftDragging = true;

            originalPosition = RectTransform.anchoredPosition;
            originalParent = transform.parent;

            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();

            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (board != null && board.IsOnMainMenu) return;

        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

            board?.RecalculateLines();
            board?.ChangeCursor(board.grab);

            CheckCollisionAndSetColor();
        }

        if (eventData.button == PointerEventData.InputButton.Right && isRightDragging)
        {
            board?.ShowPreviewFromNodeToScreen(this, eventData.position, eventData.pressEventCamera);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (board != null && board.IsOnMainMenu) return;

        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            isLeftDragging = false;
            canvasGroup.blocksRaycasts = true;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool droppedOnFolder = false;
            Transform folderTransform = null;
            bool droppedOnBoard = false;
            Transform boardTransform = null;

            foreach (var r in results)
            {
                ClueFolderDropZone folderHit = r.gameObject.GetComponentInParent<ClueFolderDropZone>();
                if (folderHit != null)
                {
                    droppedOnFolder = true;
                    folderTransform = folderHit.transform;
                    break;
                }

                ClueBoardDropZone boardHit = r.gameObject.GetComponentInParent<ClueBoardDropZone>();
                if (boardHit != null)
                {
                    droppedOnBoard = true;
                    boardTransform = boardHit.transform;
                }
            }

            if (droppedOnFolder)
            {
                transform.SetParent(folderTransform, true);

                transform.SetSiblingIndex(1);

                PlayerPrefs.DeleteKey(data.clueID + "_x");
                PlayerPrefs.DeleteKey(data.clueID + "_y");
                PlayerPrefs.DeleteKey(data.clueID + "_parent");
                if (data.connectedClues != null) data.connectedClues.Clear();
                PlayerPrefs.DeleteKey(data.clueID + "_connections");
                PlayerPrefs.Save();

                SetVisualColor(originalColor);
                LayoutRebuilder.ForceRebuildLayoutImmediate(folderTransform as RectTransform);

                board?.RecalculateLines();
                board?.ChangeCursor(board.hover);
                return;
            }
            // CASO B: Lo soltamos en el corcho
            if (droppedOnBoard)
            {
                transform.SetParent(boardTransform, true);

                // --- LIMITAR A LOS BORDES DEL CORCHO ---
                Vector3[] areaCorners = new Vector3[4];
                ((RectTransform)boardTransform).GetWorldCorners(areaCorners);
                Vector3[] nodeCorners = new Vector3[4];
                RectTransform.GetWorldCorners(nodeCorners);

                Vector3 pos = RectTransform.position;
                float nodeWidth = nodeCorners[2].x - nodeCorners[0].x;
                float nodeHeight = nodeCorners[2].y - nodeCorners[0].y;

                pos.x = Mathf.Clamp(pos.x, areaCorners[0].x + nodeWidth / 2, areaCorners[2].x - nodeWidth / 2);
                pos.y = Mathf.Clamp(pos.y, areaCorners[0].y + nodeHeight / 2, areaCorners[2].y - nodeHeight / 2);
                RectTransform.position = pos;

                RectTransform.SetAsFirstSibling();

                bool isColliding = CheckCollisionAndSetColor();

                if (isColliding)
                {
                    transform.SetParent(originalParent, false);
                    RectTransform.anchoredPosition = originalPosition;
                    data.boardPosition = originalPosition;
                    SetVisualColor(originalColor);
                }
                else
                {
                    data.boardPosition = RectTransform.anchoredPosition;
                    SetVisualColor(originalColor);
                    SaveState();
                }

                board?.RecalculateLines();
                board?.ChangeCursor(board.hover);
                return;
            }

            transform.SetParent(originalParent, false);
            RectTransform.anchoredPosition = originalPosition;
            SetVisualColor(originalColor);
            board?.RecalculateLines();
            board?.ChangeCursor(board.hover);
        }
    }

    private void SetVisualColor(Color color)
    {
        if (clueVisualImage != null) clueVisualImage.color = color;
    }

    private bool CheckCollisionAndSetColor()
    {
        if (board == null || board.clueNodes == null || clueVisualImage == null) return false;

        bool isColliding = false;
        Rect currentRect = GetWorldRect(RectTransform);

        foreach (var otherNode in board.clueNodes)
        {
            if (otherNode == this) continue;
            if (otherNode.RectTransform == null) continue;

            // Ignoramos las pistas que estén guardadas dentro de la carpeta
            if (otherNode.transform.parent != null && otherNode.transform.parent.GetComponentInParent<ClueFolderDropZone>() != null)
                continue;

            Rect otherRect = GetWorldRect(otherNode.RectTransform);

            if (currentRect.Overlaps(otherRect))
            {
                isColliding = true;
                break;
            }
        }

        // Aplicamos un color rojo más suave. (1f, 0.4f, 0.4f) es un rojo apastelado.
        SetVisualColor(isColliding ? new Color(1f, 0.4f, 0.4f, 1f) : originalColor);
        return isColliding;
    }

    public void ResetState(Transform defaultParent)
    {
        PlayerPrefs.DeleteKey(data.clueID + "_x");
        PlayerPrefs.DeleteKey(data.clueID + "_y");
        PlayerPrefs.DeleteKey(data.clueID + "_parent");
        PlayerPrefs.DeleteKey(data.clueID + "_connections");

        if (data.connectedClues != null)
        {
            data.connectedClues.Clear();
        }

        if (defaultParent != null)
        {
            transform.SetParent(defaultParent, false);
        }

        SetFound(false);
    }

    public void SaveState()
    {
        PlayerPrefs.SetFloat(data.clueID + "_x", RectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat(data.clueID + "_y", RectTransform.anchoredPosition.y);
        string parentName = transform.parent.name;
        PlayerPrefs.SetString(data.clueID + "_parent", parentName);
        string connections = string.Join(",", data.connectedClues);
        PlayerPrefs.SetString(data.clueID + "_connections", connections);
        PlayerPrefs.Save();
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;
        return new Rect(corners[0].x, corners[0].y, width, height);
    }

    public void MoveToCorcho(RectTransform newParent, ClueBoardManager b)
    {
        transform.SetParent(newParent, true);
        transform.SetAsFirstSibling();
        BindBoard(b);
    }
    public void OnPointerClick(PointerEventData eventData) { }
}