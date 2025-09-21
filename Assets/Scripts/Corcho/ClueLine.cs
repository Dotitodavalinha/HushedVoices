using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClueLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ClueBoardManager board;
    private string fromID;
    private string toID;
    private bool isHovering;

    public void Setup(ClueBoardManager board, string from, string to)
    {
        this.board = board;
        fromID = from;
        toID = to;
    }

    private void Update()
    {
        if (isHovering && Input.GetKeyDown(KeyCode.Q))
        {
            board.RemoveConnection(fromID, toID);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        board.ChangeCursor(board.disconnect); // cursor de tijera
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        board.ChangeCursor(board.hover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Si querés también que con click se corte:
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            board.RemoveConnection(fromID, toID);
        }
    }

}
