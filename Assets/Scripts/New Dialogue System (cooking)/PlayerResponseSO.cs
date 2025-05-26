using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Player Response")]
public class PlayerResponseSO : ScriptableObject
{
    public string responseText;
    public DialogueNodeSO nextNode;
    public UnityEngine.Events.UnityEvent onSelected;
}
