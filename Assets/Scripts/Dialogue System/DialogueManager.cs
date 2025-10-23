using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Cinemachine;
using System.Collections;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private Transform responseContainer;
    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private TextMeshProUGUI npcNameText;
    private NPCDialogue currentNPC;

    private List<Button> currentResponseButtons = new List<Button>();
    private int selectedResponseIndex = -1;

    public bool ModoParanoia { get; private set; }
    [SerializeField] private GameObject ButtonPrefabParanoia;

    [Header("CAMARA")]
    public CameraManagerZ camManager;
    public CinemachineFreeLook lukeCamera;
    private CinemachineVirtualCameraBase camAnterior;

    private DialogueNodeSO currentNode;
    public bool IsOpen => dialoguePanel.activeSelf;
    public bool HasResponses => currentResponseButtons.Count > 0;

    [SerializeField] private PlayerMovementLocker movementLocker;

    public static DialogueManager Instance { get; private set; }

    private bool isTyping = false;
    public bool IsTyping => isTyping;

    private Coroutine typingCoroutine;

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

    private void Update()
    {
        if (dialoguePanel == null)
            return;

        if (!IsOpen || isTyping || !HasResponses)
            return;

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (selectedResponseIndex < 0 || selectedResponseIndex >= currentResponseButtons.Count)
            {
                SetSelectedResponse(0);
            }
            else
            {
                SetSelectedResponse(selectedResponseIndex);
            }
        }
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

        camAnterior = camManager.GetCurrentCamera();
        camManager.SwitchCamera(lukeCamera);
        camManager.CambiarLookAt(npc.transform);

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

        foreach (Transform child in responseContainer)
            Destroy(child.gameObject);
        currentResponseButtons.Clear();
        selectedResponseIndex = -1;
        EventSystem.current.SetSelectedGameObject(null);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(node.npcText, npc));
    }

    private IEnumerator TypeText(string fullText, NPCDialogue npc, float typingSpeed = 0.05f)
    {
        isTyping = true;
        npcText.text = "";
        foreach (char c in fullText)
        {
            npcText.text += c;
            switch (npc.npcVoiceType)
            {
                case 0: SoundManager.instance.PlaySound(SoundID.DialogueTypingHighSound); break;
                case 1: SoundManager.instance.PlaySound(SoundID.DialogueTypingSound); break;
                case 2: SoundManager.instance.PlaySound(SoundID.DialogueTypingLowSound); break;
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowResponses(currentNode, npc);
    }

    private void ShowResponses(DialogueNodeSO node, NPCDialogue npc)
    {
        if (node == null || node.responses.Count == 0)
        {
            currentResponseButtons.Clear();
            selectedResponseIndex = -1;
            return;
        }

        foreach (var response in node.responses)
        {
            if (!string.IsNullOrEmpty(response.requiredClue) &&
                !PlayerClueTracker.Instance.HasClue(response.requiredClue))
                continue;

            PlayerResponseSO capturedResponse = response;

            GameObject prefab = (ModoParanoia && capturedResponse.paranoiaAffected) ? ButtonPrefabParanoia : responseButtonPrefab;
            GameObject btnObject = Instantiate(prefab, responseContainer);

            Button btn = btnObject.GetComponent<Button>();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = capturedResponse.responseText;

            btn.onClick.AddListener(() => OnResponseSelected(capturedResponse, npc));
            btn.onClick.AddListener(() => SoundManager.instance.PlaySound(SoundID.DialogueOptionSound));

            currentResponseButtons.Add(btn);
        }

        if (currentResponseButtons.Count > 0)
            SetSelectedResponse(0);
    }

    public void FinishTypingCurrentText()
    {
        if (currentNode != null)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            npcText.text = currentNode.npcText;
            isTyping = false;

            ShowResponses(currentNode, currentNPC);
        }
    }

    private void OnResponseSelected(PlayerResponseSO response, NPCDialogue npc)
    {
        currentResponseButtons.Clear();
        selectedResponseIndex = -1;
        EventSystem.current.SetSelectedGameObject(null);

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
            ShowNode(response.nextNode, npc);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        DialoguePanelOff();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentResponseButtons.Clear();
        selectedResponseIndex = -1;
        EventSystem.current.SetSelectedGameObject(null);

        GameManager.Instance.UnlockUI();
        movementLocker.UnlockMovement();
        currentNode = null;

        var player = GameObject.FindWithTag("Player").transform;
        if (camAnterior != null)
            camManager.SwitchCamera(camAnterior);
        camManager.CambiarLookAt(player.transform);
    }

    public void ChangeSelectedResponse(int direction)
    {
        if (!HasResponses) return;

        int newIndex = selectedResponseIndex + direction;
        int maxIndex = currentResponseButtons.Count - 1;

        if (newIndex < 0) newIndex = maxIndex;
        else if (newIndex > maxIndex) newIndex = 0;

        SetSelectedResponse(newIndex);
    }

    private void SetSelectedResponse(int index)
    {
        if (index < 0 || index >= currentResponseButtons.Count)
            return;

        selectedResponseIndex = index;
        Button selectedButton = currentResponseButtons[selectedResponseIndex];

        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void SelectCurrentResponse()
    {
        if (selectedResponseIndex >= 0 && selectedResponseIndex < currentResponseButtons.Count)
        {
            currentResponseButtons[selectedResponseIndex].onClick.Invoke();
        }
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
                    Transform dialogueBox = dialoguePanel.transform.Find("DialogueBox");
                    if (dialogueBox != null)
                    {
                        npcNameText = dialogueBox.transform.Find("NombreNPC")?.GetComponent<TextMeshProUGUI>();
                        npcText = dialogueBox.transform.Find("DialogoNPC")?.GetComponent<TextMeshProUGUI>();
                    }
                    dialoguePanel.SetActive(false);
                }
            }
        }

        if (movementLocker == null)
            movementLocker = FindObjectOfType<PlayerMovementLocker>();

        if (lukeCamera == null)
            lukeCamera = FindObjectOfType<CinemachineFreeLook>();

        if (camManager == null)
            camManager = FindObjectOfType<CameraManagerZ>();
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