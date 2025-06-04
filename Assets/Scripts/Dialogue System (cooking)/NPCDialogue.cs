using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
   
    public string npcName;
    public NPCMoodController moodController; // asignás

    public NPCDialogueSetSO dialogueSet;
    public DialogueSO currentDialogue;

    public void SetDialogueByEtapa(int etapa)
    {
        if (etapa >= 0 && etapa < dialogueSet.dialoguesPorEtapa.Length)
            currentDialogue = dialogueSet.dialoguesPorEtapa[etapa];
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        DialogueManager.Instance.StartDialogue(dialogue, this);
    }


}
