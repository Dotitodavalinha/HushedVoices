using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClueNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ClueData data;
    public Image image;
    public GameObject missingOverlay;

    public RectTransform RectTransform { get; private set; }

    private Canvas canvas; // referencia para mover bien en UI
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private RectTransform playArea;

    private Vector2 currentPos;

    private static ClueNode selectedNode = null;


    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(ClueData clue, bool isFound, RectTransform area, Vector2 startPos)
    {
        data = clue;
        playArea = area;

        image.sprite = clue.clueSprite;
        missingOverlay.SetActive(!isFound);

        currentPos = startPos;
        RectTransform.anchoredPosition = currentPos;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = RectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        Vector3[] areaCorners = new Vector3[4];
        playArea.GetWorldCorners(areaCorners);

        Vector3[] nodeCorners = new Vector3[4];
        RectTransform.GetWorldCorners(nodeCorners);

        Vector3 pos = RectTransform.position;

        float nodeWidth = nodeCorners[2].x - nodeCorners[0].x;
        float nodeHeight = nodeCorners[2].y - nodeCorners[0].y;

        pos.x = Mathf.Clamp(pos.x,
            areaCorners[0].x + nodeWidth / 2,
            areaCorners[2].x - nodeWidth / 2);

        pos.y = Mathf.Clamp(pos.y,
            areaCorners[0].y + nodeHeight / 2,
            areaCorners[2].y - nodeHeight / 2);

        RectTransform.position = pos;

        currentPos = RectTransform.anchoredPosition;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        data.boardPosition = RectTransform.anchoredPosition;

        PlayerPrefs.SetFloat(data.clueID + "_x", RectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat(data.clueID + "_y", RectTransform.anchoredPosition.y);
        PlayerPrefs.Save();

        FindObjectOfType<ClueBoardManager>().RecalcularLineas();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        var board = FindObjectOfType<ClueBoardManager>();

        switch (board.currentMode)
        {
            case ClueBoardManager.ClueMode.Connect:
                if (board.selectedNode == null)
                {
                    board.selectedNode = this;
                }
                else
                {
                    if (board.selectedNode == this)
                    {
                        board.selectedNode = null;
                        return;
                    }

                    if (board.selectedNode.data.connectedClues.Contains(data.clueID))
                    {
                        board.selectedNode.data.connectedClues.Remove(data.clueID);
                        data.connectedClues.Remove(board.selectedNode.data.clueID);
                    }
                    else
                    {
                        board.selectedNode.data.connectedClues.Add(data.clueID);
                        data.connectedClues.Add(board.selectedNode.data.clueID);
                    }

                    board.RecalcularLineas();
                    board.selectedNode = null;
                }
                break;

            case ClueBoardManager.ClueMode.DeleteConnections:
                foreach (var targetID in new List<string>(data.connectedClues))
                {
                    var targetNode = board.GetNodeByID(targetID);
                    if (targetNode != null)
                        targetNode.data.connectedClues.Remove(data.clueID);
                }
                data.connectedClues.Clear();

                board.RecalcularLineas();
                break;

            case ClueBoardManager.ClueMode.Normal:
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (Time.time - lastClickTime < doubleClickThreshold)
                    {
                        ClueZoomUI.Instance.ShowClue(data);
                    }
                    lastClickTime = Time.time;
                }
                break;
        }
    }



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