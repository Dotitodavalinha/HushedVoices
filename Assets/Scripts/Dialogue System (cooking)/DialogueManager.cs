using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Cinemachine;
using System.Collections;

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
    public CinemachineFreeLook lukeCamera;
    //public CinemachineVirtualCamera lukeCamera;
    private CinemachineVirtualCameraBase camAnterior;


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
        DialoguePanelOn();
        SoundManager.instance.PlaySound(SoundID.DialogueTypingSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentNPC = npc;
        npcNameText.text = npc.npcName;
        movementLocker.LockMovement();
        ShowNode(dialogue.rootNode, currentNPC);

        //la camara cambia a la de luke y mira al npc
        camAnterior = camManager.GetCurrentCamera();
        camManager.SwitchCamera(lukeCamera);
        camManager.CambiarLookAt(npc.transform);

        //npc mira a luke y luke a npc
        if (npc.noRotateToLook)
        {
            Vector3 targetPosition = movementLocker.transform.position;
            targetPosition.y = npc.transform.position.y;
            npc.transform.LookAt(targetPosition);
        }

            Vector3 npcPos = npc.transform.position;
            npcPos.y = movementLocker.transform.position.y;
            movementLocker.transform.LookAt(npcPos);
        
    }

    public void SetModoParanoia(bool valor)
    {
        ModoParanoia = valor;
    }


    private void ShowNode(DialogueNodeSO node, NPCDialogue npc)
    {
        currentNode = node;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(node.npcText, npc));


        // Limpiar respuestas anteriores
        foreach (Transform child in responseContainer)
            Destroy(child.gameObject);

        // Crear botones de respuesta
        foreach (var response in node.responses)
        {
            // Si la respuesta requiere una pista y no la tenemos, no se muestra :p
            if (!string.IsNullOrEmpty(response.requiredClue) &&
                !PlayerClueTracker.Instance.HasClue(response.requiredClue))
            {
                continue;
            }

            GameObject prefab = (ModoParanoia && response.paranoiaAffected) ? ButtonPrefabParanoia : responseButtonPrefab;

            GameObject btn = Instantiate(prefab, responseContainer);

            btn.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
            btn.GetComponent<Button>().onClick.AddListener(() => OnResponseSelected(response, npc));
           btn.GetComponent<Button>().onClick.AddListener(() => SoundManager.instance.PlaySound(SoundID.DialogueOptionSound));
        }
    }


    private void OnResponseSelected(PlayerResponseSO response, NPCDialogue npc)
    {
        response.onResponseChosen?.Invoke();
        switch (response.moodChange)
        {
            case MoodChange.Happy:
                currentNPC.moodController.SetMoodHappy();
                break;
            case MoodChange.Angry:
                currentNPC.moodController.SetMoodAngry(true);
                break;
            case MoodChange.Normal:
                currentNPC.moodController.SetMoodNormal();
                break;
        }


        if (response.nextNode != null)
            ShowNode(response.nextNode, npc );
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        DialoguePanelOff();

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
            dialoguePanel = GameObject.Find("DialogueCanvas")?.transform.Find("DialoguePanel")?.gameObject;
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
                lukeCamera = camObj.GetComponent<CinemachineFreeLook>();
            }

            if (lukeCamera == null)
            {
                // Como fallback, encontrar cualquier CinemachineVirtualCamera
                lukeCamera = FindObjectOfType<CinemachineFreeLook>();
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

    private Coroutine typingCoroutine;

    private IEnumerator TypeText(string fullText, NPCDialogue npc, float typingSpeed = 0.05f)
    {
        npcText.text = "";
      
        foreach (char c in fullText)
        {
            npcText.text += c;
            switch (npc.npcVoiceType)
            {
                case 0:
                    SoundManager.instance.PlaySound(SoundID.DialogueTypingHighSound);
                    break;
                case 1:
                    SoundManager.instance.PlaySound(SoundID.DialogueTypingSound);
                    break;
                case 2:
                    SoundManager.instance.PlaySound(SoundID.DialogueTypingLowSound);
                    break;
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    private void DialoguePanelOff()
    {
        CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
        dialoguePanel.SetActive(false);
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        SoundManager.instance.StopSound(SoundID.DialogueTypingLowSound);
        SoundManager.instance.StopSound(SoundID.DialogueTypingHighSound);
        SoundManager.instance.StopSound(SoundID.DialogueTypingSound);
    }

    private void DialoguePanelOn()
    {
        CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
        dialoguePanel.SetActive(true);
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

}


