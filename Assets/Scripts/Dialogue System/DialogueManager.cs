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
    GameObject lukeCameraObject; //HARDCODEADO POR FEDE PARA QUE SE ASIGNE LA CAMARA QUE QUIERO


    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private Transform responseContainer;
    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private GameObject nextPagePrompt;
    [SerializeField] private GameObject NextPageBox;
    private NPCDialogue currentNPC;
    GameObject lookAtObject;

    private bool skipTyping = false; 
    private bool advancePageTrigger = false; 
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

        Vector3 targetPosition = movementLocker.transform.position;

        camAnterior = camManager.GetCurrentCamera();
        camManager.SwitchCamera(lukeCamera);

        Vector3 promedio = (npc.transform.position + targetPosition) / 2;

        lookAtObject = new GameObject("LookAtObject");
        lookAtObject.transform.position = promedio;

        camManager.CambiarLookAt(lookAtObject.transform);

        if (npc.noRotateToLook)
        {
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
        skipTyping = false;
        List<string> sentences = SplitIntoSentences(fullText);

        if (NextPageBox != null) NextPageBox.SetActive(false);

        foreach (string sentence in sentences)
        {
            npcText.text = "";
            npcText.pageToDisplay = 1;
            foreach (char c in sentence)
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
            yield return new WaitForSeconds(0.5f);
            if (sentence != sentences[sentences.Count - 1])
            {
                if (NextPageBox != null) NextPageBox.SetActive(true);
                if (nextPagePrompt != null) nextPagePrompt.SetActive(true);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) || advancePageTrigger);
                if (NextPageBox != null) NextPageBox.SetActive(false);
                if (nextPagePrompt != null) nextPagePrompt.SetActive(false);

                advancePageTrigger = false;
            }
        }
        isTyping = false;
        if (NextPageBox != null) NextPageBox.SetActive(false);
        if (nextPagePrompt != null) nextPagePrompt.SetActive(false);

        ShowResponses(currentNode, npc);
    }

    private List<string> SplitIntoSentences(string text)
    {
        string tempPlaceholder = "[[ELLIPSIS]]";
        string textProtected = text.Replace("...", tempPlaceholder);
        string processed = textProtected.Replace(".", ".|").Replace("?", "?|").Replace("!", "!|");

        string[] splitArray = processed.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);

        List<string> cleanSentences = new List<string>();
        foreach (string s in splitArray)
        {
            string trimmed = s.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                string finalSentence = trimmed.Replace(tempPlaceholder, "...");
                cleanSentences.Add(finalSentence);
            }
        }
        return cleanSentences;
    }

    public void OnNextPageButtonPressed()
    {
        if (isTyping)
        {
            return;
        }
        advancePageTrigger = true;
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
        if (isTyping)
        {
            skipTyping = true;
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

        StartCoroutine(UnlockMovementAfterTime(0.7f));
        currentNode = null; 

        var player = GameObject.FindWithTag("Player").transform;
        if (camAnterior != null)
            camManager.SwitchCamera(camAnterior);

        Destroy(lookAtObject);
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
        GameObject canvas = GameObject.Find("DialogueCanvas");

        if (canvas != null)
        {
            Transform panelTransform = canvas.transform.Find("DialoguePanel");

            if (panelTransform != null)
            {
                dialoguePanel = panelTransform.gameObject;
                responseContainer = panelTransform.Find("ButtonContainer");

                Transform box = panelTransform.Find("DialogueBox");
                if (box != null)
                {
                    npcNameText = box.Find("NombreNPC")?.GetComponent<TextMeshProUGUI>();
                    npcText = box.Find("DialogoNPC")?.GetComponent<TextMeshProUGUI>();
                    Transform nextPageBoxTransform = box.Find("NextPageBox");

                    if (nextPageBoxTransform != null)
                    {
                        NextPageBox = nextPageBoxTransform.gameObject;
                        NextPageBox.SetActive(false);
                        Transform promptTransform = nextPageBoxTransform.Find("NextPagePrompt");
                        if (promptTransform != null)
                        {
                            nextPagePrompt = promptTransform.gameObject;
                          
                        }
                    }
                    else
                    {
                        nextPageBoxTransform = panelTransform.Find("NextPageBox");
                        if (nextPageBoxTransform != null)
                        {
                            NextPageBox = nextPageBoxTransform.gameObject;
                            NextPageBox.SetActive(false);
                        }
                    }
                }

                Transform btnNextTransform = panelTransform.Find("Button");
                if (btnNextTransform != null)
                {
                    Button btnNext = btnNextTransform.GetComponent<Button>();
                    btnNext.onClick.RemoveAllListeners();
                    btnNext.onClick.AddListener(OnNextPageButtonPressed);
                }
                DialoguePanelOff();
            }
        }
        else
        {
            Debug.LogWarning("No se encontró 'DialogueCanvas' en esta escena.");
        }
        if (movementLocker == null) movementLocker = FindObjectOfType<PlayerMovementLocker>();
        if (lukeCameraObject == null) lukeCameraObject = GameObject.Find("LukeCamera");
        if (lukeCameraObject != null) lukeCamera = lukeCameraObject.GetComponent<CinemachineFreeLook>();
        if (camManager == null) camManager = FindObjectOfType<CameraManagerZ>();
    }

    private void DialoguePanelOff()
    {
        if (NextPageBox != null) NextPageBox.SetActive(false);

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

    private IEnumerator UnlockMovementAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.UnlockUI();
        movementLocker.UnlockMovement();
    }

}