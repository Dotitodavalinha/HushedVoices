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

    public bool ModoParanoia { get; private set; }
    [SerializeField] private GameObject ButtonPrefabParanoia;



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
        dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        movementLocker.UnlockMovement();


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

        //la camara cambia a la de luke y mira al npc
        camAnterior = camManager.GetCurrentCamera();
        camManager.SwitchCamera(lukeCamera);
        camManager.CambiarLookAt(npc.transform);

        //npc mira a luke y luke a npc

        Vector3 targetPosition = movementLocker.transform.position;
        targetPosition.y = npc.transform.position.y;
        npc.transform.LookAt(targetPosition);

        Vector3 npcPos = npc.transform.position;
        npcPos.y = movementLocker.transform.position.y;
        movementLocker.transform.LookAt(npcPos);



    }

    public void SetModoParanoia(bool valor)
    {
        ModoParanoia = valor;
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
            GameObject prefab = (ModoParanoia && response.paranoiaAffected) ? ButtonPrefabParanoia : responseButtonPrefab; // si hay paranoia y el boton esta afectado usa otro prefab

            GameObject btn = Instantiate(prefab, responseContainer);

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


        if (lukeCamera == null)
        {
            GameObject camObj = GameObject.Find("LukeCamera");
            if (camObj != null)
            {
                lukeCamera = camObj.GetComponent<CinemachineVirtualCamera>();
            }

            if (lukeCamera == null)
            {
                // Como fallback, encontrar cualquier CinemachineVirtualCamera
                lukeCamera = FindObjectOfType<CinemachineVirtualCamera>();
                Debug.LogWarning("No se encontró la cámara de Luke por nombre. Se asignó la primera cámara encontrada en la escena.");
            }
        }


        if (camManager == null)
        {
            camManager = FindObjectOfType<CameraManagerZ>();
            if (camManager == null)
                Debug.LogWarning("No se encontró CameraManagerZ en la escena.");
        }

    }



}


