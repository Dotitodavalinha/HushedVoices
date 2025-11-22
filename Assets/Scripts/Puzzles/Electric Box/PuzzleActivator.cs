using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public GameObject puzzlePanel;

    private bool playerIsNear = false;

    void Start()
    {
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
            Debug.Log("Pulsa E para interactuar.");
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
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            TogglePuzzle();
        }
    }

    private void TogglePuzzle(bool? state = null)
    {
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