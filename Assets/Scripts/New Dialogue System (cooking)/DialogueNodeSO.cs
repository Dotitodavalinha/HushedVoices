using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Node")]
public class DialogueNodeSO : ScriptableObject
{
    [TextArea] public string npcText;
    public List<PlayerResponseSO> responses;
}
