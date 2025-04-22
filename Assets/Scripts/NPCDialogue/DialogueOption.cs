[System.Serializable]
public class DialogueOption
{
    public string text;
    public bool isDangerous;
    public bool isVeryDangerous;

    public DialogueOption(string t, bool danger = false, bool veryDangerous = false)
    {
        text = t;
        isDangerous = danger;
        isVeryDangerous = veryDangerous;
    }
}

