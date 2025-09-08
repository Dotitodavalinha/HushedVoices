using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClueNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public ClueData data;
    public Image image;
    public GameObject missingOverlay;

    public RectTransform RectTransform { get; private set; }

    private Canvas canvas; // referencia para mover bien en UI
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private RectTransform playArea;
    private Vector2 currentPos;

    // --- NUEVO: referencia directa al board ---
    private ClueBoardManager board;

    // --- Click/drag izquierdo (abrir zoom vs mover) ---
    private bool isLeftDragging = false;
    private float leftPressTime = 0f;
    private Vector2 leftPressPos;
    private const float clickTimeThreshold = 0.25f;
    private const float clickMoveThreshold = 8f; // px UI

    // --- Drag derecho para conectar ---
    private bool isRightDragging = false;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void BindBoard(ClueBoardManager b) => board = b;

    public void Init(ClueData clue, bool isFound, RectTransform area, Vector2 startPos)
    {
        data = clue;
        playArea = area;

        image.sprite = clue.clueSprite;
        missingOverlay.SetActive(!isFound);

        currentPos = startPos;
        RectTransform.anchoredPosition = currentPos;
    }

    // ---------------- POINTER DOWN / UP ----------------

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
            // Empieza preview de conexión
            isRightDragging = true;
            if (board != null)
            {
                board.ShowPreviewFromNodeToScreen(this, eventData.position, eventData.pressEventCamera);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Final del drag derecho: intento conectar si suelto sobre otro nodo
        if (eventData.button == PointerEventData.InputButton.Right && isRightDragging)
        {
            isRightDragging = false;

            if (board != null)
            {
                // Detecto si solté sobre otro ClueNode
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

        // Left: si no llegó a ser drag y fue un tap corto sin mover => abrir zoom
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

    // ---------------- DRAG DE UI ----------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isLeftDragging = true;
            originalPosition = RectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = false;
        }
        // Nota: El drag derecho lo manejamos con OnPointerDown/Up + OnDrag para preview
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Arrastre izquierdo: mover nodo
        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            // Clamp al playArea
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

            currentPos = RectTransform.anchoredPosition;

            // Actualizar líneas EN VIVO mientras arrastro
            if (board != null) board.RecalcularLineas();
        }

        // Arrastre derecho: actualizar preview hasta el mouse
        if (isRightDragging && eventData.button == PointerEventData.InputButton.Right)
        {
            if (board != null)
            {
                board.ShowPreviewFromNodeToScreen(this, eventData.position, eventData.pressEventCamera);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isLeftDragging)
        {
            isLeftDragging = false;
            canvasGroup.blocksRaycasts = true;
            data.boardPosition = RectTransform.anchoredPosition;

            PlayerPrefs.SetFloat(data.clueID + "_x", RectTransform.anchoredPosition.x);
            PlayerPrefs.SetFloat(data.clueID + "_y", RectTransform.anchoredPosition.y);
            PlayerPrefs.Save();

            if (board != null) board.RecalcularLineas();
        }
    }

    // (Se mantiene por compatibilidad, pero ya no usamos “modos” ni doble click)
    public void OnPointerClick(PointerEventData eventData)
    {
        // Intencionalmente vacío: el click se maneja en OnPointerUp (tap corto) y el drag en OnDrag.
    }

    // -------- Persistencia util --------

    private void SavePosition(ClueData clue)
    {
        PlayerPrefs.SetFloat(clue.clueID + "_x", clue.boardPosition.x);
        PlayerPrefs.SetFloat(clue.clueID + "_y", clue.boardPosition.y);
    }

    private Vector2 LoadPosition(ClueData clue)
    {
        float x = PlayerPrefs.GetFloat(clue.clueID + "_x", clue.boardPosition.x);
        float y = PlayerPrefs.GetFloat(clue.clueID + "_y", clue.boardPosition.y);
        return new Vector2(x, y);
    }
}
