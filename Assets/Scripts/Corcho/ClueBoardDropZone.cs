using UnityEngine;
using UnityEngine.EventSystems;

public class ClueBoardDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private ClueBoardManager board;
    public RectTransform corchoArea;

    public void OnDrop(PointerEventData eventData)
    {
        var clueNode = eventData.pointerDrag?.GetComponent<ClueNode>();
        if (clueNode != null)
        {
            clueNode.transform.SetParent(corchoArea, true);

            if (board != null)
                clueNode.BindBoard(board, corchoArea);

            clueNode.SaveState();
        }
    }
}
