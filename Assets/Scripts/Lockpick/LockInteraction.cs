using UnityEngine;
using System.Collections;

public class LockInteraction : MonoBehaviour
{
    [Header("Puzzle References")]
    [SerializeField] private GameObject interactionPromptObject; // el puzzle mismo

    [Header("Door Movement")]
    [SerializeField] private Transform doorObject;
    [SerializeField] private float targetYRotation = 350f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Player References")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    private bool puzzleActive = false;
    private bool playerNearby = false;

    private UIInteractable interactable;

    private void Start()
    {
        interactable = GetComponent<UIInteractable>();
        if (interactionPromptObject != null)
            interactionPromptObject.SetActive(false);
    }

    private void Update()
    {
        // Detectar si el jugador está cerca gracias al sistema de InteractableBase
        playerNearby = interactable != null && interactable.IsOpen;

        // Si está activo, permitir cerrar con E
        if (puzzleActive && Input.GetKeyDown(KeyCode.E))
        {
            TogglePuzzle(false);
        }
        // Si no está activo pero el interactable fue activado por E, abrimos el puzzle
        else if (!puzzleActive && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TogglePuzzle(true);
        }
    }

    private void TogglePuzzle(bool state)
    {
        puzzleActive = state;

        if (interactionPromptObject != null)
            interactionPromptObject.SetActive(state);

        if (playerMovementScript != null)
            playerMovementScript.enabled = !state;
    }

    public void CompletePuzzleSequence()
    {
        TogglePuzzle(false);

        if (doorObject != null)
            StartCoroutine(OpenDoorRotation());
    }

    private IEnumerator OpenDoorRotation()
    {
        Quaternion startRotation = doorObject.rotation;
        Quaternion endRotation = Quaternion.Euler(
            doorObject.localRotation.eulerAngles.x,
            targetYRotation,
            doorObject.localRotation.eulerAngles.z
        );

        float time = 0;
        while (time < 1)
        {
            doorObject.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            time += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        doorObject.rotation = endRotation;
    }
}
