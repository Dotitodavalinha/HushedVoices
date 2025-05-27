using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private GameObject pressEText;

    [SerializeField] private bool playerInRange = false;

    [SerializeField] private string npcName = "NombreNPC"; 


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
                DialogueManager.Instance.StartDialogue(dialogue, npcName);

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
