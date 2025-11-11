using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueDatabase", menuName = "Clues/Clue Database")]
public class ClueDatabase : ScriptableObject
{
    public List<ClueData> allClues;

    private Dictionary<string, Sprite> _clueDictionary;

    public Sprite GetSpriteByID(string id)
    {
        if (_clueDictionary == null)
        {
            _clueDictionary = new Dictionary<string, Sprite>();
            foreach (ClueData clue in allClues)
            {
                if (clue != null && !_clueDictionary.ContainsKey(clue.clueID))
                {
                    _clueDictionary.Add(clue.clueID, clue.clueSprite);
                }
            }
        }

        if (_clueDictionary.TryGetValue(id, out Sprite icon))
        {
            return icon;
        }

        Debug.LogWarning($"No se encontró el Sprite para la pista con ID: {id} en la ClueDatabase.");
        return null;
    }
}