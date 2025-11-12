using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RevealableByConcentration : MonoBehaviour

{
    [Header("Reveal settings")]
    public float revealRadius = 3f;
    public bool requireLineOfSight = false;

    [Tooltip("Si es true, al revelarse se intenta agregar la pista automaticamente")]
    public bool AddClueOnReveal = true;

    [Tooltip("ID de la pista que se añadirá al PlayerClueTracker")]
    public string clueId;

    [Header("Visuals")]
    public GameObject Outline;           // opcional: objeto que actúa como outline
    public bool keepRevealedAfterEnd = false; // si true, permanece marcado tras End

    [Header("Interactable control")]
    [Tooltip("Si este objeto comparte el mismo GameObject con un Interactable, arrastralo aquí para bloquear/permitir la interacción")]
    public InteractableBase interactableToToggle;

    [Tooltip("Interactable que se mantendrá DESACTIVADO durante la Concentración y ACTIVADO sin Concentración")]
    public InteractableBase interactableOppositeToggle;
    private bool oppositeDisabledByMe = false;

    [Header("Visual de pista (partículas o signo de pregunta)")]
    public GameObject clueIndicator;

    // runtime
    private bool highlighted = false;
    private Transform player;
    private InteractableBase interactableComponent;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // cacheamos si este objeto también es interactuable
        interactableComponent = GetComponent<InteractableBase>();
        if (ConcentrationManager.Instance != null)
        {

            ConcentrationManager.Instance.OnConcentrationStarted += OnConcentrationStarted;
            ConcentrationManager.Instance.OnConcentrationEnded += OnConcentrationEnded;
        }
        // asegurarnos de que visual empieza off

        Highlight(false);
        if (interactableToToggle != null)
        {

            interactableToToggle.enabled = false;

        }
        else if (interactableComponent != null)
        {

            // fallback: si el mismo objeto ya tiene InteractableBase, lo desactivamos

            interactableComponent.enabled = false;

        }
    }
    private void OnConcentrationStarted()
    {
        if (player == null) return;
        float d = Vector3.Distance(player.position, transform.position);
        if (d > revealRadius) return;
        if (requireLineOfSight && !HasLineOfSight()) return;
        // mostrar outline / visual
        Highlight(true);
        // activamos la interacción solo mientras está revelado
        if (interactableToToggle != null)

            interactableToToggle.enabled = true;

        else if (interactableComponent != null)

            interactableComponent.enabled = true;
        // registrar pista (si corresponde) - evita duplicados
        if (AddClueOnReveal && !string.IsNullOrEmpty(clueId))

        {

            if (PlayerClueTracker.Instance != null && !PlayerClueTracker.Instance.HasClue(clueId))

            {

                PlayerClueTracker.Instance.AddClue(clueId);

                // opcional: reproducir sonido

                // SoundManager.instance.PlaySound(SoundID.CluePickupSound);

            }

        }
        // desactivar un interactable ajeno mientras haya Concentración (si es q hay uno)

        if (interactableOppositeToggle != null)

        {

            if (!interactableOppositeToggle.IsOpen && interactableOppositeToggle.enabled)

            {

                interactableOppositeToggle.enabled = false;

                oppositeDisabledByMe = true;

            }

            else

            {

                oppositeDisabledByMe = false;

            }
        }
    }

    private void OnConcentrationEnded()
    {

        if (!keepRevealedAfterEnd)

            Highlight(false);
        if (!keepRevealedAfterEnd)
        {
            // preferimos el interactable asignado manualmente si existe

            InteractableBase target = interactableToToggle != null ? interactableToToggle : interactableComponent;
            if (target != null)
            {

                // si el interactable está abierto (player lo está usando), no lo desactivamos

                if (!target.IsOpen)

                {

                    target.enabled = false;

                }

                // si está abierto, lo dejamos activo para que el jugador pueda cerrarlo.

                // cuando el jugador cierre la UI/interacción, InteractableBase.Deactivate()

                // llamará a ConcentrationManager.RemoveInteractionHold() y si había un End

                // pendiente, se ejecutará entonces.
            }
            else
            {
                // fallback: si no hay componente interactable, apagamos el outline asignado

                if (Outline != null)

                    Outline.SetActive(false);
            }

        }
        // reactivar el interactable 'normal' si lo habiamos apagado

        if (interactableOppositeToggle != null && oppositeDisabledByMe)

        {

            interactableOppositeToggle.enabled = true;

        }
        oppositeDisabledByMe = false;
    }

    private void Highlight(bool on)

    {

        highlighted = on;
        // prioridad: si tiene InteractableBase, pedimos que active sus outlines

        if (interactableComponent != null)

        {

            interactableComponent.SetOutlinesActive(on);

        }

        else

        {

            // fallback al GameObject Outline asignado manualmente

            if (Outline != null)

                Outline.SetActive(on);

        }



        // también podés lanzar partículas/SFX aquí si querés

    }

    private bool HasLineOfSight()

    {

        Vector3 dir = (transform.position - player.position);

        RaycastHit hit;

        if (Physics.Raycast(player.position + Vector3.up * 1.6f, dir.normalized, out hit, revealRadius))

        {

            return hit.collider == GetComponent<Collider>();

        }

        return false;

    }

    private void OnDestroy()

    {

        if (ConcentrationManager.Instance != null)

        {

            ConcentrationManager.Instance.OnConcentrationStarted -= OnConcentrationStarted;

            ConcentrationManager.Instance.OnConcentrationEnded -= OnConcentrationEnded;

        }

    }

    public void OnClueCollected()
    {
        // Apagar el efecto visual de la pista
        if (clueIndicator != null && clueIndicator.activeSelf)
        {
            clueIndicator.SetActive(false);
        }

        // También podés apagar el outline si querés
        if (Outline != null && Outline.activeSelf)
        {
            Outline.SetActive(false);
        }

        // Desuscribirse de los eventos de concentración (ya no hace falta seguir escuchando)
        if (ConcentrationManager.Instance != null)
        {
            ConcentrationManager.Instance.OnConcentrationStarted -= OnConcentrationStarted;
            ConcentrationManager.Instance.OnConcentrationEnded -= OnConcentrationEnded;
        }
    }



}

