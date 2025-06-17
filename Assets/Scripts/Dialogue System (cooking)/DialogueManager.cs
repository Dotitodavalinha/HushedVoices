using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Cinemachine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private Transform responseContainer;
    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private TextMeshProUGUI npcNameText;
    private NPCDialogue currentNPC;

    [Header("CAMARA")]
    public CameraManagerZ camManager;
    public CinemachineVirtualCamera lukeCamera;
    private CinemachineVirtualCamera camAnterior;


    private DialogueNodeSO currentNode;
    public bool IsOpen => dialoguePanel.activeSelf;

    [SerializeField] private PlayerMovementLocker movementLocker;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    private void Start()
    {
        EndDialogue();
    }
    public void StartDialogue(DialogueSO dialogue, NPCDialogue npc)
    {


        dialoguePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentNPC = npc;
        npcNameText.text = npc.npcName;
        movementLocker.LockMovement();
        ShowNode(dialogue.rootNode);

        camAnterior = camManager.GetCurrentCamera();
        camManager.SwitchCamera(lukeCamera);
        camManager.CambiarLookAt(npc.transform);


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
        response.onResponseChosen?.Invoke();
        switch (response.moodChange)
        {
            case MoodChange.Happy:
                currentNPC.moodController.SetMoodHappy();
                break;
            case MoodChange.Angry:
                currentNPC.moodController.SetMoodAngry();
                break;
            case MoodChange.Normal:
                currentNPC.moodController.SetMoodNormal();
                break;
        }


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

        var player = GameObject.FindWithTag("Player").transform;
        if (camAnterior != null)
        {
            camManager.SwitchCamera(camAnterior);
        }
        camManager.CambiarLookAt(player.transform);

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (dialoguePanel == null)
        {
            GameObject dialogueCanvas = GameObject.Find("DialogueCanvas");

            if (dialogueCanvas != null)
            {
                dialoguePanel = dialogueCanvas.transform.Find("DialoguePanel")?.gameObject;
                if (dialoguePanel != null)
                {
                    responseContainer = dialoguePanel.transform.Find("ButtonContainer");

                    // Buscar dentro de DialogueBox
                    Transform dialogueBox = dialoguePanel.transform.Find("DialogueBox");
                    if (dialogueBox != null)
                    {
                        npcNameText = dialogueBox.transform.Find("NombreNPC")?.GetComponent<TextMeshProUGUI>();
                        npcText = dialogueBox.transform.Find("DialogoNPC")?.GetComponent<TextMeshProUGUI>();
                    }

                    dialoguePanel.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("No se encontró DialogueCanvas en la escena.");
            }
        }

        if (movementLocker == null)
        {
            movementLocker = FindObjectOfType<PlayerMovementLocker>();
        }
    }

}


