using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
   // [SerializeField] private DialogueSO dialogue;
    [SerializeField] private GameObject pressEText;

    [SerializeField] public bool playerInRange = false;

    [SerializeField] private NPCDialogue npcDialogue;

    private void Start()
    {
        pressEText.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance.IsOpen)
            {
                DialogueManager.Instance.EndDialogue();
            }
            else
            {
                // evitar abrir si hay otra UI en uso
                if (!GameManager.Instance.TryLockUI())
                    return;

                npcDialogue.StartDialogue(npcDialogue.currentRoot);
                pressEText.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pressEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pressEText.SetActive(false);
        }
    }
}
