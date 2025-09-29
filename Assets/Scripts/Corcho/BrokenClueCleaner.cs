using UnityEngine;
using UnityEngine.EventSystems;

public class BrokenClueCleaner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;
    private ClueBoardManager board;

    private void Start()
    {
        board = FindObjectOfType<ClueBoardManager>();
    }

    void Update()
    {
        if (isHovering && Input.GetKeyDown(KeyCode.B))
        {
            Destroy(gameObject);
            if (board != null) board.ChangeCursor(board.hover);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        if (board != null) board.ChangeCursor(board.deleteClues);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        GetComponent<UnityEngine.UI.Image>().color = Color.white;
        if (board != null) board.ChangeCursor(board.hover);
    }
}
