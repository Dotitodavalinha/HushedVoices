using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LockInteraction : MonoBehaviour
{
    [Header("Comportamiento de interacción")]
    [SerializeField] private bool needInteract = true; // si true necesita apretar E para abrir
    [SerializeField] private GameObject pressE_UI; // UI que muestra "E"
    [SerializeField] private NOTEInteractionZone zonaInteraccion; // opcional - si usas tu sistema de zona
    [SerializeField] private float maxDistanceToOpen = 0f; // opcional (0 = sin check adicional)

    [Header("Puzzle / Lock-specific")]
    [SerializeField] private GameObject PuzzleObject; // el puzzle mismo (opcional)
    [SerializeField] private Transform doorObject;
    [SerializeField] private float targetYRotation = 350f;
    [SerializeField] private float rotationSpeed = 3f;

    // estados internos
    private bool inRange = false;
    private bool active = false; // si el interactable está abierto
    private bool puzzleActive = false;
    private bool completed = false;

    // si se usa zona en lugar de trigger
    private bool useZone => zonaInteraccion != null;

    private void Awake()
    {
        // asegurar collider trigger si usa detección por trigger (siempre deja como trigger)
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;

        if (pressE_UI != null)
            pressE_UI.SetActive(false);

        if (PuzzleObject != null)
            PuzzleObject.SetActive(false);

        // si se usa zona, sincronizar estado inicial y mostrar pressE si corresponde
        if (useZone)
            inRange = zonaInteraccion.jugadorDentro;

        UpdatePressEState();
    }

    private void Update()
    {
        // si estamos usando zona externa, actualizar inRange desde ella
        if (useZone)
            inRange = zonaInteraccion.jugadorDentro;

        // actualizar visual del press E cada frame según el estado
        UpdatePressEState();

        // 1) Si no estamos en rango, nada de tecla E para activar
        if (!inRange) return;

        // 2) Si ya hay UI abierta por este interactable (active == true) manejamos la E diferentemente
        if (active)
        {
            // si el puzzle está activo, LockInteraction se encarga de cerrar con E
            if (puzzleActive)
            {
                if (Input.GetKeyDown(KeyCode.E))
                    CloseInteractableFromThis();
                return;
            }

            // si estamos abiertos, E cierra
            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseInteractableFromThis();
            }

            return;
        }

        // 3) Si no está abierto: permitir abrir si needInteract y se aprieta E, o abrir automáticamente si !needInteract
        if (completed) return; // no abrir si ya completado

        if (needInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryOpen();
            }
        }
        else
        {
            TryOpen();
        }
    }

    // Intento de apertura: primero pedir el lock al GameManager (TryLockUI), luego abrir el interactable (SendMessage para compat)
    private void TryOpen()
    {
        if (!CanBeInteracted()) return;

        // opcional: distancia extra check
        if (maxDistanceToOpen > 0f)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                if (Vector3.Distance(playerObj.transform.position, transform.position) > maxDistanceToOpen)
                    return;
            }
        }

        // seguridad: que exista GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance es null en LockInteraction.TryOpen()");
            return;
        }

        if (!GameManager.Instance.TryLockUI()) return; // si no pudo lockear, no abrimos

        // marcar opened
        active = true;

        // notificar (compatibilidad con otros scripts que escuchan OnInteractableOpened)
        SendMessage("OnInteractableOpened", SendMessageOptions.DontRequireReceiver);

        // ocultar pressE (ya está abierto)
        SetPressEActive(false);
    }

    // Cierre provocado desde este LockInteraction (libera GameManager)
    private void CloseInteractableFromThis()
    {
        // notificar cierre (compatibilidad)
        SendMessage("OnInteractableClosed", SendMessageOptions.DontRequireReceiver);

        // liberar lock de GameManager (proteger si no existe)
        if (GameManager.Instance != null)
            GameManager.Instance.UnlockUI();

        // marcar estados
        active = false;
        puzzleActive = false;

        // reactivar pressE si seguimos en rango y podemos interactuar
        UpdatePressEState();
    }

    // Handler cuando alguien (SendMessage) dispara OnInteractableOpened (por compatibilidad)
    public void OnInteractableOpened()
    {
        if (completed) return;
        // Si este interactable tiene un puzzle, activarlo
        puzzleActive = PuzzleObject != null;
        if (PuzzleObject != null)
            PuzzleObject.SetActive(true);
    }

    // Handler cuando alguien (SendMessage) dispara OnInteractableClosed
    public void OnInteractableClosed()
    {
        // desactivar estado de puzzle y objeto puzzle
        puzzleActive = false;
        if (PuzzleObject != null)
            PuzzleObject.SetActive(false);

        // marcar no activo
        active = false;
        // NO llamamos UnlockUI aquí para evitar doble unlock; CloseInteractableFromThis lo hace.
    }

    // Public: llamado por el puzzle cuando se completa
    public void CompletePuzzleSequence()
    {
        if (completed) return;
        completed = true;

        // cerrar UI y bloquear re-apertura
        CloseInteractableFromThis();
        var baseComp = GetComponent<InteractableBase>();
        baseComp?.DisableInteraction();

        // abrir puerta si corresponde
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

        float t = 0f;
        while (t < 1f)
        {
            doorObject.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            t += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        doorObject.rotation = endRotation;
    }

    // Trigger detection (si preferís usar NOTEInteractionZone, asignalo y usa zona en lugar de triggers)
    private void OnTriggerEnter(Collider other)
    {
        if (useZone) return; // si se usa zona externa, ignorar triggers
        if (!IsPlayer(other)) return;
        inRange = true;
        UpdatePressEState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (useZone) return;
        if (!IsPlayer(other)) return;
        inRange = false;
        UpdatePressEState();

        // si dejamos el area y el interactable estaba abierto, cerramos (comportamiento previo)
        if (active && !completed)
        {
            CloseInteractableFromThis();
        }
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player");
    }

    private bool CanBeInteracted()
    {
        var baseComp = GetComponent<InteractableBase>();
        return baseComp == null ? true : baseComp.CanBeInteracted;
    }

    private void SetPressEActive(bool on)
    {
        if (pressE_UI != null)
            pressE_UI.SetActive(on);
    }

    private void UpdatePressEState()
    {
        // mostrar pressE solo si está en rango, no está abierto el interactable, no está completado y puede ser interactuado
        bool shouldShow = inRange && !active && !completed && CanBeInteracted();
        SetPressEActive(shouldShow);
    }
}
