using System.Collections;
using System.Collections.Generic;
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
            clueNode.transform.SetParent(transform, true);

            if (board != null)
            {
                clueNode.BindBoard(board, board.GetComponent<RectTransform>());
            }
            else
            {
                Debug.LogWarning($"No hay ClueBoardManager asignado en {name}. Asignalo en el inspector.");
            }
        }
    }

}
