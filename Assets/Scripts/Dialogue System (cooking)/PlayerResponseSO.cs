using UnityEngine;
using UnityEngine.Events;

public enum MoodChange { None, Happy, Normal, Angry }

[CreateAssetMenu(menuName = "Dialogue/Player Response")]
public class PlayerResponseSO : ScriptableObject
{
    [TextArea] public string responseText;
    public DialogueNodeSO nextNode;
    public MoodChange moodChange = MoodChange.None;
    public UnityEvent onResponseChosen;

    public bool paranoiaAffected; // marcar desde el editor las respuestas afectadas
    public string requiredClue; // nombre de la pista necesaria para que aparezca esta respuesta

}
