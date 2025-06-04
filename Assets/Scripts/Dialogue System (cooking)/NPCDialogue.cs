using UnityEngine;


public class NPCDialogue : MonoBehaviour
{

    public string npcName;
    public NPCMoodController moodController;
    public DialogueSO currentDialogue;

    [SerializeField] private EventDialogueMappingSO[] eventDialogueMappings;

    void OnEnable()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.OnBensNoteUnlocked += HandleBensNoteUnlocked;
            ProgressManager.Instance.OnCoffeeShopUnlocked += HandleCoffeeShopUnlocked;
        }
    }

    void OnDisable()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.OnBensNoteUnlocked -= HandleBensNoteUnlocked;
            ProgressManager.Instance.OnCoffeeShopUnlocked -= HandleCoffeeShopUnlocked;
        }
    }

    private void HandleBensNoteUnlocked()
    {
        UpdateDialogue("BensNoteUnlocked");
    }

    private void HandleCoffeeShopUnlocked()
    {
        UpdateDialogue("CoffeeShopUnlocked");
    }

    private void UpdateDialogue(string eventName)
    {
        foreach (var mapping in eventDialogueMappings)
        {
            if (mapping.eventName == eventName)
            {
                foreach (var entry in mapping.npcDialogues)
                {
                    if (entry.npcName == npcName)
                    {
                        currentDialogue = entry.dialogue;
                        return;
                    }
                }
            }
        }
    }
    public void StartDialogue(DialogueSO dialogue)
    {
        DialogueManager.Instance.StartDialogue(dialogue, this);
    }


}
