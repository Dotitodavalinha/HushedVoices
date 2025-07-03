using System.Collections.Generic;
using UnityEngine;

public class PlayerClueTracker : MonoBehaviour
{
    public static PlayerClueTracker Instance;

    [SerializeField] public List<string> cluesList = new List<string>();
    public HashSet<string> clues = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar la lista serializada al HashSet
        clues = new HashSet<string>(cluesList);
    }

    public void AddClue(string clueID)
    {
        if (clues.Add(clueID))
        {
            cluesList.Add(clueID); // reflejar en la lista para ver en editor
        }
    }

    public void AddClues(List<string> clueIDs)
    {
        foreach (string clueID in clueIDs)
        {
            if (clues.Add(clueID))
            {
                cluesList.Add(clueID);
            }
        }
    }
    public void AddCluesByList(string clueID)
    {
        if (clues.Add(clueID))
        {
            cluesList.Add(clueID); // reflejar en la lista para ver en editor
        }
    }

    public bool HasClue(string clueID)
    {
        return clues.Contains(clueID);
    }
}
