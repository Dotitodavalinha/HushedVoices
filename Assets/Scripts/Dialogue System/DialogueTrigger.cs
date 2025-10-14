using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject pressEText;

    [SerializeField] public bool playerInRange = false;

    [SerializeField] private NPCDialogue npcDialogue;

    private void Start()
    {
        pressEText.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && !DialogueManager.Instance.IsOpen)
            {
                if (!GameManager.Instance.TryLockUI())
                    return;

                npcDialogue.StartDialogue(npcDialogue.currentRoot);
                pressEText.SetActive(false);
            }
            // Adelantar dialogo con E/Click izq
            else if (DialogueManager.Instance.IsOpen && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
            {
                if (DialogueManager.Instance.IsTyping)
                {
                    DialogueManager.Instance.FinishTypingCurrentText();
                }
                else if (DialogueManager.Instance.HasResponses && Input.GetKeyDown(KeyCode.E))
                {
                    DialogueManager.Instance.SelectCurrentResponse();
                }
                else if (!DialogueManager.Instance.HasResponses)
                {
                    DialogueManager.Instance.EndDialogue();
                }
            }

            if (DialogueManager.Instance.IsOpen && DialogueManager.Instance.HasResponses && !DialogueManager.Instance.IsTyping)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    DialogueManager.Instance.ChangeSelectedResponse(-1);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    DialogueManager.Instance.ChangeSelectedResponse(1);
                }
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