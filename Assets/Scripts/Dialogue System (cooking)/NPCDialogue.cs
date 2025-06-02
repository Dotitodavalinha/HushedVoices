using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
   
    public string npcName;
    public NPCMoodController moodController; // asignás esto desde el NPC

    public void StartDialogue(DialogueSO dialogue)
    {
        DialogueManager.Instance.StartDialogue(dialogue, this);
    }


}
