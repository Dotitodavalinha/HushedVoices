using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/Event Dialogue Mapping")]
public class EventDialogueMappingSO : ScriptableObject
{
    public string eventName;
    public List<NPCDialogueEntry> npcDialogues;
}

[System.Serializable]
public class NPCDialogueEntry
{
    public string npcName;
    public DialogueSO dialogue;
}
