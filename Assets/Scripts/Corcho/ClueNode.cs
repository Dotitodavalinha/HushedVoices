using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClueNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ClueData data;

    public RectTransform RectTransform { get; private set; }

    private Canvas canvas;
    private CanvasGroup canvasGroup;
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

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
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
        if (clueVisual != null)
            clueVisual.SetActive(found);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        board?.ChangeCursor(board.zoomIn);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        board?.ChangeCursor(board.hover);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isLeftDragging = true;
            originalPosition = RectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

            board?.RecalculateLines();
            board?.ChangeCursor(board.grab);

            // --- ¡AQUÍ ESTÁ LA MODIFICACIÓN! ---
            if (transform.parent != null)
            {
                // 1. Chequeamos si el padre es una zona de "drop" (el corcho)
                bool shouldClamp = transform.parent.GetComponent<ClueBoardDropZone>() != null;

                // 2. Si lo es, aplicamos la "pared invisible"
                if (shouldClamp)
                {
                    RectTransform playArea = transform.parent.GetComponent<RectTransform>();

                    Vector3[] areaCorners = new Vector3[4];
                    playArea.GetWorldCorners(areaCorners);

                    Vector3[] nodeCorners = new Vector3[4];
                    RectTransform.GetWorldCorners(nodeCorners);

                    Vector3 pos = RectTransform.position;
                    float nodeWidth = nodeCorners[2].x - nodeCorners[0].x;
                    float nodeHeight = nodeCorners[2].y - nodeCorners[0].y;

                    pos.x = Mathf.Clamp(pos.x, areaCorners[0].x + nodeWidth / 2, areaCorners[2].x - nodeWidth / 2);
                    pos.y = Mathf.Clamp(pos.y, areaCorners[0].y + nodeHeight / 2, areaCorners[2].y - nodeHeight / 2);

                    RectTransform.position = pos;
                }
                // 3. Si NO lo es (ej. es el "Folio"), no hacemos nada y dejamos
                //    que el jugador la arrastre libremente.
            }
            // --- FIN DE LA MODIFICACIÓN ---

            CheckCollisionAndSetColor();
        }

        if (eventData.button == PointerEventData.InputButton.Right && isRightDragging)
        {
            board?.ShowPreviewFromNodeToScreen(this, eventData.position, eventData.pressEventCamera);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            isLeftDragging = false;
            canvasGroup.blocksRaycasts = true;

            bool isColliding = CheckCollisionAndSetColor();

            if (isColliding)
            {
                RectTransform.anchoredPosition = originalPosition;
                data.boardPosition = originalPosition;

                SetVisualColor(originalColor);
            }
            else
            {
                data.boardPosition = RectTransform.anchoredPosition;
                SetVisualColor(originalColor);
            }

            SaveState();

            board?.RecalculateLines();
            board?.ChangeCursor(board.hover);
        }
    }


    private void SetVisualColor(Color color)
    {
        if (clueVisualImage != null)
        {
            clueVisualImage.color = color;
        }
    }

    private bool CheckCollisionAndSetColor()
    {
        if (board == null || board.clueNodes == null || clueVisualImage == null) return false;

        bool isColliding = false;
        Rect currentRect = GetRectFromRectTransform();

        foreach (var otherNode in board.clueNodes)
        {
            if (otherNode == this) continue;
            if (otherNode.transform.parent != this.transform.parent) continue;
            if (otherNode.RectTransform == null) continue;

            Rect otherRect = otherNode.GetRectFromRectTransform();

            if (currentRect.Overlaps(otherRect))
            {
                isColliding = true;
                break;
            }
        }

        SetVisualColor(isColliding ? Color.red : originalColor);
        return isColliding;
    }

    public void SaveState()
    {
        PlayerPrefs.SetFloat(data.clueID + "_x", RectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat(data.clueID + "_y", RectTransform.anchoredPosition.y);

        string parentName = transform.parent.name;
        PlayerPrefs.SetString(data.clueID + "_parent", parentName);

        if (transform.parent != null)
        {
            if (PlayerClueTracker.Instance != null)
            {
                // PlayerClueTracker.Instance.AddSafeClue(data.clueID);
            }
        }

        string connections = string.Join(",", data.connectedClues);
        PlayerPrefs.SetString(data.clueID + "_connections", connections);

        PlayerPrefs.Save();
    }


    private Rect GetRectFromRectTransform()
    {
        Vector2 anchoredPos = RectTransform.anchoredPosition;
        Vector2 size = RectTransform.sizeDelta;
        Vector2 pivot = RectTransform.pivot;

        float x = anchoredPos.x - size.x * pivot.x;
        float y = anchoredPos.y - size.y * pivot.y;

        return new Rect(x, y, size.x, size.y);
    }

    public void MoveToCorcho(RectTransform newParent, ClueBoardManager b)
    {
        transform.SetParent(newParent, true);
        BindBoard(b);

        if (PlayerClueTracker.Instance != null && data != null && !string.IsNullOrEmpty(data.clueID))
        {
            // PlayerClueTracker.Instance.AddSafeClue(data.clueID);
        }
    }

    public void OnPointerClick(PointerEventData eventData) { }
}