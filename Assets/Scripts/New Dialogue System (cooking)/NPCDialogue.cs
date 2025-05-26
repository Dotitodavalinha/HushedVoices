using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private List<DialogueSO> dialogues;    // tener todos los dialogos aca phite


    public DialogueSO GetActiveDialogue()
    {
        // Por ahora devolvemos el primero (después agregamos condiciones)
        return dialogues.Count > 0 ? dialogues[0] : null;
    }
}
