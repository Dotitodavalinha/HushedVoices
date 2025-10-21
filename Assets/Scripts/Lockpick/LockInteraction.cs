using UnityEngine;
using System.Collections;

public class LockInteraction : MonoBehaviour
{
    [Header("Puzzle References")]
    [SerializeField] private GameObject minigameRoot;
    [SerializeField] private GameObject interactionPromptObject;

    [Header("Door Movement")]
    [SerializeField] private Transform doorObject;
    [SerializeField] private float targetYRotation = 350f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Player References")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    private bool playerIsInRange = false;
    private Collider interactionCollider;

    void Start()
    {
        interactionCollider = GetComponent<Collider>();
        SetPuzzleState(false);
        if (interactionPromptObject != null) interactionPromptObject.SetActive(false);
    }

    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.E) && !minigameRoot.activeInHierarchy)
        {
            if (interactionPromptObject != null) interactionPromptObject.SetActive(false);
            SetPuzzleState(true);
        }

        else if (minigameRoot.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            SetPuzzleState(false);
            if (playerIsInRange && interactionPromptObject != null) interactionPromptObject.SetActive(true);
        }
    }

    private void SetPuzzleState(bool active)
    {
        minigameRoot.SetActive(active);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = !active;
        }

    }

    public void CompletePuzzleSequence()
    {
        //Debug.Log("Puzzle COMPLETO. Desactivando interacción y abriendo puerta.");

        SetPuzzleState(false);
        playerIsInRange = false;
        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        if (doorObject != null)
        {
            StartCoroutine(OpenDoorRotation());
        }
    }

    IEnumerator OpenDoorRotation()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (interactionPromptObject != null && !minigameRoot.activeInHierarchy)
            {
                interactionPromptObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPromptObject != null)
            {
                interactionPromptObject.SetActive(false);
            }
        }
    }
}