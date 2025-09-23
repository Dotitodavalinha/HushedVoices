using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueInteract : MonoBehaviour
{
    [Header("Prefabs de diálogos (Respetar orden)")]
    [SerializeField] private List<GameObject> dialoguePrefabs = new List<GameObject>();

    [Header("Opciones")]
    [SerializeField] private bool NeedInteract = true;
    [SerializeField] private bool DestroyAfterUse = false;

    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private GameObject PressE_UI;

    private Transform canvasTransform;
    private bool inRange = false;
    private bool dialogueActive = false;
    private int currentIndex = 0;
    private GameObject currentDialogueInstance;

    private void Start()
    {
        AssignCanvas();

        if (PressE_UI != null)
            PressE_UI.SetActive(false);
    }

    private void Update()
    {
        if (NeedInteract)
        {
            // Icono "E"
            if (zonaInteraccion != null && zonaInteraccion.jugadorDentro && !dialogueActive)
            {
                inRange = true;
                if (PressE_UI != null) PressE_UI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                    TryStartDialogue();
            }
            else
            {
                inRange = false;
                if (PressE_UI != null) PressE_UI.SetActive(false);
            }
        }
        else
        {
            // Trigger automático
            if (zonaInteraccion != null && zonaInteraccion.jugadorDentro && !dialogueActive)
            {
                TryStartDialogue();
            }
        }

        // Avanzar diálogo con E
        if (dialogueActive && Input.GetKeyDown(KeyCode.E))
            NextDialogue();
    }

    private void TryStartDialogue()
    {
        if (!GameManager.Instance.TryLockUI()) return;

        dialogueActive = true;
        currentIndex = 0;
        ShowDialogue(currentIndex);
    }

    private void ShowDialogue(int index)
    {
        if (index < 0 || index >= dialoguePrefabs.Count) return;

        if (currentDialogueInstance != null)
            Destroy(currentDialogueInstance);

        currentDialogueInstance = Instantiate(dialoguePrefabs[index], canvasTransform);
        currentDialogueInstance.transform.SetAsLastSibling();
    }

    private void NextDialogue()
    {
        currentIndex++;
        if (currentIndex < dialoguePrefabs.Count)
        {
            ShowDialogue(currentIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        GameManager.Instance.UnlockUI();

        if (currentDialogueInstance != null)
            Destroy(currentDialogueInstance);

        if (DestroyAfterUse)
            Destroy(gameObject);

        if (PressE_UI != null)
            PressE_UI.SetActive(false);
    }

    private void AssignCanvas()
    {
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
            canvasTransform = canvasObj.transform;
        else
            Debug.LogWarning("No se encontró un objeto llamado 'Canvas' en la escena.");
    }
}
