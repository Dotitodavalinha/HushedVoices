using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerClueTracker : MonoBehaviour
{
    public static PlayerClueTracker Instance;

    [Header("Pistas Actuales")]
    [SerializeField] public List<string> cluesList = new List<string>();
    public HashSet<string> clues = new HashSet<string>();

    [Header("Pistas Perdidas (En Comisaría)")]
    [SerializeField] public List<string> lostClues = new List<string>();

    [Header("Pistas Seguras (No se pierden)")]
    [Tooltip("Los IDs de las pistas que NUNCA se pierden al ser arrestado")]
    [SerializeField] public List<string> safeClues = new List<string>();

    public static event Action<List<string>> OnCluesAdded; // MODIFICADO
    public static event Action<List<string>> OnCluesLost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        clues = new HashSet<string>(cluesList);
    }

    public void AddClue(string clueID)
    {
        if (clues.Add(clueID))
        {
            cluesList.Add(clueID);
            OnCluesAdded?.Invoke(new List<string> { clueID }); // MODIFICADO
        }
    }

    public void AddClues(List<string> clueIDs)
    {
        List<string> addedCluesList = new List<string>(); // MODIFICADO
        foreach (string clueID in clueIDs)
        {
            if (clues.Add(clueID))
            {
                cluesList.Add(clueID);
                addedCluesList.Add(clueID); // MODIFICADO
            }
        }

        if (addedCluesList.Count > 0)
        {
            OnCluesAdded?.Invoke(addedCluesList); // MODIFICADO
        }
    }
    public void AddCluesByList(string clueID)
    {
        if (clues.Add(clueID))
        {
            cluesList.Add(clueID);
            OnCluesAdded?.Invoke(new List<string> { clueID }); // MODIFICADO
        }
    }

    public bool HasClue(string clueID)
    {
        return clues.Contains(clueID);
    }

    public void AddSafeClue(string clueID)
    {
        if (string.IsNullOrEmpty(clueID)) return;

        if (safeClues.Contains(clueID))
        {
            return;
        }

        safeClues.Add(clueID);
    }


    public void LoseAllClues()
    {
        if (cluesList.Count == 0) return;

        List<string> cluesToLose = new List<string>();
        foreach (string clue in cluesList)
        {
            if (!safeClues.Contains(clue))
            {
                cluesToLose.Add(clue);
            }
        }

        if (cluesToLose.Count == 0)
        {
            return;
        }

        lostClues.AddRange(cluesToLose);

        foreach (string clueID in cluesToLose)
        {
            clues.Remove(clueID);
            cluesList.Remove(clueID);
        }

        OnCluesLost?.Invoke(cluesToLose);
    }

    public void RecoverAllClues()
    {
        if (lostClues.Count == 0)
        {
            return;
        }

        // Esta función ya llama a AddClues, que ahora dispara el evento.
        // No se necesita más código aquí.
        AddClues(new List<string>(lostClues)); // Se pasa una copia por si acaso

        lostClues.Clear();
    }

    public void LoseSpecificClue(string clueID)
    {
        if (safeClues.Contains(clueID))
        {
            return;
        }

        if (HasClue(clueID))
        {
            clues.Remove(clueID);
            cluesList.Remove(clueID);

            if (!lostClues.Contains(clueID))
            {
                lostClues.Add(clueID);
            }

            OnCluesLost?.Invoke(new List<string> { clueID });
        }
    }
}