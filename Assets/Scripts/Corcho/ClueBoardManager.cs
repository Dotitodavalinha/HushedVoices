using UnityEngine;
using System.Collections.Generic;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] private List<ClueData> allClues;
    [SerializeField] private ClueNode cluePrefab;
    [SerializeField] private RectTransform boardParent;

    private List<GameObject> nodosInstanciados = new();

    public void AbrirCorcho()
    {
        boardParent.gameObject.SetActive(true);
        RegenerarBoard();
    }

    public void CerrarCorcho()
    {
        boardParent.gameObject.SetActive(false);
        LimpiarBoard();
    }


    private void RegenerarBoard()
    {
        Debug.Log("Regenerando Board Corcho");

        LimpiarBoard();

        PlayerClueTracker.Instance.clues = new HashSet<string>(PlayerClueTracker.Instance.cluesList);

        // Chequear si hay al menos una pista encontrada
        bool hayPistas = false;
        foreach (var clue in allClues)
        {
            if (PlayerClueTracker.Instance.HasClue(clue.clueID))
            {
                hayPistas = true;
                break;
            }
        }

        if (!hayPistas) return; // No generamos nada si no hay pistas encontradas

        foreach (var clue in allClues)
        {
            bool isFound = PlayerClueTracker.Instance.HasClue(clue.clueID);
            var node = Instantiate(cluePrefab, boardParent);
            node.GetComponent<RectTransform>().anchoredPosition = clue.boardPosition;
            node.Init(clue, isFound);
            nodosInstanciados.Add(node.gameObject);
        }
    }


    private void LimpiarBoard()
    {
        foreach (var obj in nodosInstanciados)
        {
            Destroy(obj);
        }
        nodosInstanciados.Clear();
    }
}
