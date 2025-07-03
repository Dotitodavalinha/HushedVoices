using UnityEngine;
using System.Collections.Generic;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] private List<ClueData> allClues;
    [SerializeField] private ClueNode cluePrefab;
    [SerializeField] private RectTransform boardParent;

    private List<GameObject> nodosInstanciados = new();

    [SerializeField] private GameObject lineUIPrefab;
    [SerializeField] private float lineYOffset = 10f; // qué tan alto se dibuja
    [SerializeField] private float lineThickness = 3f; // grosor de la línea


    private Dictionary<string, ClueNode> spawnedNodes = new();
    private List<GameObject> lineasInstanciadas = new();


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

        bool hayPistas = false;
        foreach (var clue in allClues)
        {
            if (PlayerClueTracker.Instance.HasClue(clue.clueID))
            {
                hayPistas = true;
                break;
            }
        }

        if (!hayPistas) return;

        spawnedNodes.Clear();

        // Instanciar nodos
        foreach (var clue in allClues)
        {
            bool isFound = PlayerClueTracker.Instance.HasClue(clue.clueID);
            var node = Instantiate(cluePrefab, boardParent);
            node.GetComponent<RectTransform>().anchoredPosition = clue.boardPosition;
            node.Init(clue, isFound);
            spawnedNodes[clue.clueID] = node;
            nodosInstanciados.Add(node.gameObject);
        }

        // Instanciar líneas entre pistas (UI)
        foreach (var clue in allClues)
        {
            if (!spawnedNodes.ContainsKey(clue.clueID)) continue;

            var fromNode = spawnedNodes[clue.clueID];

            foreach (var targetID in clue.connectedClues)
            {
                if (!spawnedNodes.ContainsKey(targetID)) continue;

                var toNode = spawnedNodes[targetID];

                GameObject linea = Instantiate(lineUIPrefab, boardParent);
                RectTransform lineRect = linea.GetComponent<RectTransform>();
                Vector2 start = fromNode.RectTransform.anchoredPosition;
                Vector2 end = toNode.RectTransform.anchoredPosition;

                // aplicar offset en Y
                start.y += lineYOffset;
                end.y += lineYOffset;

                Vector2 direction = end - start;
                float distance = direction.magnitude;

                lineRect.sizeDelta = new Vector2(distance, lineThickness);
                lineRect.anchoredPosition = start + direction / 2f;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRect.rotation = Quaternion.Euler(0, 0, angle);

                lineRect.SetAsFirstSibling(); // mandar arriba

                lineasInstanciadas.Add(linea);
            }
        }

    }



    private void LimpiarBoard()
    {
        foreach (var obj in nodosInstanciados)
        {
            Destroy(obj);
        }
        nodosInstanciados.Clear();

        foreach (var linea in lineasInstanciadas)
        {
            Destroy(linea);
        }
        lineasInstanciadas.Clear();

    }
}
