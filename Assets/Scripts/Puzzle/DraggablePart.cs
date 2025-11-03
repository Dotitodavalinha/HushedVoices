using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggablePartUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string targetId;
    public float snapMaxDistance = 60f; // en px aprox
    public GraphicRaycaster raycaster;

    RectTransform rt;
    CanvasGroup cg;
    Vector2 startAnchoredPos;
    Vector3 pointerOffset;
    bool placed;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (startAnchoredPos == Vector2.zero)
            startAnchoredPos = rt.anchoredPosition;
    }

    // Lo llama el manager después de posicionar
    public void SetInitialAnchoredPos(Vector2 pos)
    {
        rt.anchoredPosition = pos;
        startAnchoredPos = pos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placed) return;
        cg.blocksRaycasts = false;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out worldPoint);
        pointerOffset = rt.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placed) return;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out worldPoint);
        rt.position = worldPoint + pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placed) return;

        var results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        SocketZoneUI targetSocket = null;
        foreach (var r in results)
            if (r.gameObject.TryGetComponent(out SocketZoneUI s)) { targetSocket = s; break; }

        if (targetSocket != null && targetSocket.id == targetId)
        {
            var socketRT = targetSocket.GetComponent<RectTransform>();
            Vector3 socketWorldCenter = socketRT.TransformPoint(socketRT.rect.center);

            if (Vector2.Distance(rt.position, socketWorldCenter) <= snapMaxDistance)
            {
                rt.position = socketWorldCenter;
                placed = true;
                cg.blocksRaycasts = true;
                cg.interactable = false;
                PuzzleManagerUI.Instance.NotifyPlaced();
                return;
            }
        }

        rt.anchoredPosition = startAnchoredPos;
        cg.blocksRaycasts = true;
    }
}
