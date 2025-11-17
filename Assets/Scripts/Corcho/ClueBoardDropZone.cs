using UnityEngine;
using UnityEngine.EventSystems;

public class ClueBoardDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private ClueBoardManager board;

    public void OnDrop(PointerEventData eventData)
    {
        var clueNode = eventData.pointerDrag?.GetComponent<ClueNode>();
        if (clueNode != null)
        {
            clueNode.transform.SetParent(this.transform, true);

            if (board != null)
            {
                // ESTA ES LA LÍNEA CORREGIDA
                clueNode.BindBoard(board);
            }

            clueNode.SaveState();
        }
    }
}