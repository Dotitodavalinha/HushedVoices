using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggablePart : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string targetId;
    public float snapMaxDistance = 60f; // píxeles aprox
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

        SocketZone targetSocket = null;
        foreach (var r in results)
            if (r.gameObject.TryGetComponent(out SocketZone s)) { targetSocket = s; break; }

        if (targetSocket != null && targetSocket.id == targetId)
        {
            // 1) punto de snap: usa SnapPoint si existe; si no, el centro del rect
            var socketRT = targetSocket.GetComponent<RectTransform>();
            Transform snapT = socketRT.Find("SnapPoint");
            Vector3 snapWorldPos = snapT != null
                ? ((RectTransform)snapT).position
                : socketRT.TransformPoint(socketRT.rect.center);

            // 2) como ya hicimos raycast sobre el rect, no hace falta distancia:
            //    si querés margen de error, podés mantener un umbral:
            // if (Vector2.Distance(rt.position, snapWorldPos) <= snapMaxDistance) { ... }

            // pegar y terminar
            rt.position = snapWorldPos;
            gameObject.SetActive(false);                 // oculto la pieza UI
            PuzzleManager.Instance.CorrectPlacement(targetId);
            return;
        }

        rt.anchoredPosition = startAnchoredPos;
        cg.blocksRaycasts = true;
    }
}
