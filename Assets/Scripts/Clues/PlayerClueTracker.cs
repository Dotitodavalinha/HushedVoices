using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerClueTracker : MonoBehaviour
{
    public static PlayerClueTracker Instance;
    [SerializeField] public HashSet<string> clues = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddClue(string clueID)
    {
        clues.Add(clueID);
    }

    public bool HasClue(string clueID)
    {
        return clues.Contains(clueID);
    }
}
