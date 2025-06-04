using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue Set")]
public class NPCDialogueSetSO : ScriptableObject
{
    public DialogueSO[] dialoguesPorEtapa; // Etapa 0, 1, 2...
}
