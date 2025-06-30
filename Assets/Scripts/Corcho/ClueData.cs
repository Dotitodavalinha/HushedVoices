using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Clue", menuName = "Clues/New Clue")]
public class ClueData : ScriptableObject
{
    public string clueID;
    public Sprite clueSprite;
    public Vector2 boardPosition;
    public List<string> connectedClues; //  agregar conexiones despues ??
    public bool isRevelation;
}
