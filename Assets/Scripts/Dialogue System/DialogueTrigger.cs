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
        if (!playerInRange) return;

        if (!DialogueManager.Instance.IsOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!GameManager.Instance.TryLockUI())
                    return;

                npcDialogue.StartDialogue(npcDialogue.currentRoot);
                pressEText.SetActive(false);
            }
        }
        else
        {
            if (DialogueManager.Instance.IsTyping)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    DialogueManager.Instance.FinishTypingCurrentText();
                }
            }
            else
            {
                if (DialogueManager.Instance.HasResponses)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    {
                        DialogueManager.Instance.ChangeSelectedResponse(-1);
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        DialogueManager.Instance.ChangeSelectedResponse(1);
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueManager.Instance.SelectCurrentResponse();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        DialogueManager.Instance.EndDialogue();
                    }
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