using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public GameObject puzzlePanel;

    [Header("Configuración de Horario")]
    [Range(0, 24)] public float startHour = 22f;
    [Range(0, 24)] public float endHour = 4f;

    private LightingManager timeManager;
    private bool playerIsNear = false;

    // Referencia al script que quieres controlar sin modificar su código
    private ClueInteractable clueUI;

    void Start()
    {
        timeManager = FindObjectOfType<LightingManager>();

        // Buscamos el script de la pista en este mismo objeto
        clueUI = GetComponent<ClueInteractable>();

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            UpdateUIState(); // Chequeo inicial al entrar
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;

            if (puzzlePanel.activeSelf)
            {
                TogglePuzzle(false);
            }

            if (clueUI != null) clueUI.enabled = true;
        }
    }

    private void OnDisable()
    {
        playerIsNear = false;
    }

    private void Update()
    {
        if (playerIsNear)
        {
            UpdateUIState();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (CheckTime())
                {
                    TogglePuzzle();
                }
            }
        }
    }

    private void UpdateUIState()
    {
        if (clueUI == null) return;

        bool isTime = CheckTime();

        if (clueUI.enabled != isTime)
        {
            clueUI.enabled = isTime;
        }
    }

    private bool CheckTime()
    {
        if (timeManager == null) return true;

        float currentH = timeManager.TimeOfDay;

        if (startHour > endHour)
        {
            return currentH >= startHour || currentH < endHour;
        }
        else
        {
            return currentH >= startHour && currentH < endHour;
        }
    }

    private void TogglePuzzle(bool? state = null)
    {
        if (puzzlePanel == null) return;

        bool shouldBeActive = state ?? !puzzlePanel.activeSelf;
        puzzlePanel.SetActive(shouldBeActive);
        SetCursorState(shouldBeActive);
    }

    public void DeactivatePuzzleAndActivator()
    {
        TogglePuzzle(false);
        gameObject.SetActive(false);
    }

    private void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}