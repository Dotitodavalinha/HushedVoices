using UnityEngine;
using System.Collections.Generic;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] private List<ClueData> allClues;
    [SerializeField] private ClueNode cluePrefab;
    [SerializeField] private RectTransform boardParent;
    [SerializeField] private GameObject Corchoculpable;

    private List<GameObject> nodosInstanciados = new();

    [SerializeField] private GameObject lineUIPrefab;
    [SerializeField] private float lineYOffset = 10f;
    [SerializeField] private float lineThickness = 3f;


    private Dictionary<string, ClueNode> spawnedNodes = new();
    private List<GameObject> lineasInstanciadas = new();

    [SerializeField] private RectTransform playArea;

    private Dictionary<string, List<string>> dynamicConnections = new Dictionary<string, List<string>>();
    public ClueNode selectedNode;

    public enum ClueMode { Normal, Connect, DeleteConnections }
    public ClueMode currentMode = ClueMode.Normal;



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

    public void OpenCulpables()
    {
        Corchoculpable.gameObject.SetActive(true);
        boardParent.gameObject.SetActive(false);

    }


    public void ClosedCulpables()
    {
        Corchoculpable.gameObject.SetActive(false);
        boardParent.gameObject.SetActive(true);
        RegenerarBoard();
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

        foreach (var clue in allClues)
        {
            bool isFound = PlayerClueTracker.Instance.HasClue(clue.clueID);

            float x = PlayerPrefs.GetFloat(clue.clueID + "_x", clue.boardPosition.x);
            float y = PlayerPrefs.GetFloat(clue.clueID + "_y", clue.boardPosition.y);
            Vector2 startPos = new Vector2(x, y);

            var node = Instantiate(cluePrefab, boardParent);
            node.Init(clue, isFound, playArea, startPos);

            spawnedNodes[clue.clueID] = node;
            nodosInstanciados.Add(node.gameObject);
        }



        foreach (var clue in allClues)
        {
            if (!spawnedNodes.ContainsKey(clue.clueID)) continue;

            var fromNode = spawnedNodes[clue.clueID];

            List<string> targets;
            if (dynamicConnections.ContainsKey(clue.clueID))
                targets = dynamicConnections[clue.clueID];
            else
                targets = clue.connectedClues;

            foreach (var targetID in targets)
            {
                if (!spawnedNodes.ContainsKey(targetID)) continue;

                var toNode = spawnedNodes[targetID];

                GameObject linea = Instantiate(lineUIPrefab, boardParent);
                RectTransform lineRect = linea.GetComponent<RectTransform>();
                Vector2 start = fromNode.RectTransform.anchoredPosition;
                Vector2 end = toNode.RectTransform.anchoredPosition;

                start.y += lineYOffset;
                end.y += lineYOffset;

                Vector2 direction = end - start;
                float distance = direction.magnitude;

                lineRect.sizeDelta = new Vector2(distance, lineThickness);
                lineRect.anchoredPosition = start + direction / 2f;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRect.rotation = Quaternion.Euler(0, 0, angle);

                lineRect.SetAsFirstSibling();
                lineasInstanciadas.Add(linea);
            }
        }


    }

    public void AddConnection(string fromID, string toID)
    {
        if (!dynamicConnections.ContainsKey(fromID))
            dynamicConnections[fromID] = new List<string>();

        if (!dynamicConnections[fromID].Contains(toID))
            dynamicConnections[fromID].Add(toID);

        RecalcularLineas();
    }

    public void RemoveConnection(string fromID, string toID)
    {
        if (dynamicConnections.ContainsKey(fromID) && dynamicConnections[fromID].Contains(toID))
        {
            dynamicConnections[fromID].Remove(toID);
            RecalcularLineas();
        }
    }




    public void RecalcularLineas()
    {
        foreach (var linea in lineasInstanciadas)
        {
            Destroy(linea);
        }
        lineasInstanciadas.Clear();

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

                start.y += lineYOffset;
                end.y += lineYOffset;

                Vector2 direction = end - start;
                float distance = direction.magnitude;

                lineRect.sizeDelta = new Vector2(distance, lineThickness);
                lineRect.anchoredPosition = start + direction / 2f;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRect.rotation = Quaternion.Euler(0, 0, angle);

                lineRect.SetAsFirstSibling();

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

    public ClueNode GetNodeByID(string clueID)
    {
        spawnedNodes.TryGetValue(clueID, out ClueNode node);
        return node;
    }

    public void SetModeConnect() => currentMode = ClueMode.Connect;
    public void SetModeDeleteConnections() => currentMode = ClueMode.DeleteConnections;
    public void SetModeNormal() => currentMode = ClueMode.Normal;

}
