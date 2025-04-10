[System.Serializable]
public class DialogueOption
{
    public string text;
    public bool isDangerous;

    public DialogueOption(string t, bool danger = false)
    {
        text = t;
        isDangerous = danger;
    }
}
