using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private Transform responseContainer;
    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private TextMeshProUGUI npcNameText;


    private DialogueNodeSO currentNode;
    public bool IsOpen => dialoguePanel.activeSelf;

    [SerializeField] private PlayerMovementLocker movementLocker;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void StartDialogue(DialogueSO dialogue, string npcName)
    {
        dialoguePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        npcNameText.text = npcName; 
        movementLocker.LockMovement();
        ShowNode(dialogue.rootNode);
    }


    private void ShowNode(DialogueNodeSO node)
    {
        currentNode = node;
        npcText.text = node.npcText;

        // Limpiar respuestas anteriores
        foreach (Transform child in responseContainer)
            Destroy(child.gameObject);

        // Crear botones de respuesta
        foreach (var response in node.responses)
        {
            GameObject btn = Instantiate(responseButtonPrefab, responseContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
            btn.GetComponent<Button>().onClick.AddListener(() => OnResponseSelected(response));
        }
    }

    private void OnResponseSelected(PlayerResponseSO response)
    {
        response.onSelected?.Invoke();

        if (response.nextNode != null)
            ShowNode(response.nextNode);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        movementLocker.UnlockMovement();
        currentNode = null;
    }
}
