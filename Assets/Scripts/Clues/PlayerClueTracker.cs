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

    public static event Action OnClueAdded;
    public static event Action OnClueLost;

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
            OnClueAdded?.Invoke();
        }
    }

    public void AddClues(List<string> clueIDs)
    {
        bool cluesWereAdded = false;
        foreach (string clueID in clueIDs)
        {
            if (clues.Add(clueID))
            {
                cluesList.Add(clueID);
                cluesWereAdded = true;
            }
        }

        if (cluesWereAdded)
        {
            OnClueAdded?.Invoke();
        }
    }
    public void AddCluesByList(string clueID)
    {
        if (clues.Add(clueID))
        {
            cluesList.Add(clueID);
            OnClueAdded?.Invoke();
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
        //Debug.Log($"PISTA SEGURA: '{clueID}' se añadió a safeClues.");
    }


    public void LoseAllClues()
    {
        //Debug.Log($"LOSEALLCLUES: Método llamado. Pistas actuales: {cluesList.Count}");
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
            //Debug.Log("LOSEALLCLUES: El jugador solo tiene pistas seguras. No se pierde nada.");
            return;
        }

        lostClues.AddRange(cluesToLose);
        //Debug.Log($"LOSEALLCLUES: {cluesToLose.Count} pistas movidas a lostClues.");

        foreach (string clueID in cluesToLose)
        {
            clues.Remove(clueID);
            cluesList.Remove(clueID);
        }

        OnClueLost?.Invoke();
        //Debug.Log($"LOSEALLCLUES: Proceso completado. Pistas restantes: {cluesList.Count}. Pistas perdidas: {lostClues.Count}");
    }

    public void RecoverAllClues()
    {
        if (lostClues.Count == 0)
        {
            //Debug.Log("RECOVER: No hay pistas en lostClues para recuperar.");
            return;
        }

        //Debug.Log($"RECOVER: Recuperando {lostClues.Count} pistas.");

        AddClues(lostClues);

        lostClues.Clear();
        //Debug.Log("RECOVER: Pistas recuperadas. lostClues ahora está vacía.");
    }

    public void LoseSpecificClue(string clueID)
    {
        if (safeClues.Contains(clueID))
        {
            //Debug.Log($"LOSE_SPECIFIC: No se puede perder la pista '{clueID}' porque es una pista segura.");
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

            OnClueLost?.Invoke();
            //Debug.Log($"Pista '{clueID}' perdida y enviada a comisaría.");
        }
    }
}