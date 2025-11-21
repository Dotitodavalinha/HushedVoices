using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedPass : MonoBehaviour
{
    public GameObject PressE_UI;
    public GameObject Outline;
    public string escenaFinalDelDia = "FinalScene";
    public float horaDormir = 22f;

    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private GameObject CantSleepYetUI;
    private bool infoOpen = false;
    [SerializeField] private GameObject cleaningObject; // referencia al objeto que debe limpiarse antes de usar la cama


    public LightingManager timeManager;
    private bool jugadorCerca;
    private bool puedeDormir;

    public static event Action OnPlayerSlept;

    [SerializeField] private Fade fadeScript;

    // Referencia local para el Locker
    private PlayerMovementLocker movementLocker;


    void Start()
    {
        PressE_UI.SetActive(false);
        timeManager = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();

        // Búsqueda inicial del Locker
        movementLocker = FindAnyObjectByType<PlayerMovementLocker>();
    }

    void Update()
    {
        //si el objeto de limpieza aun existe, no permitir interaccion con la cama
        if (cleaningObject != null)
        {
            PressE_UI.SetActive(false);
            Outline.SetActive(false);
            return;
        }

        if (timeManager != null)
        {
            puedeDormir = timeManager.TimeOfDay >= horaDormir || timeManager.TimeOfDay < 4;
        }

        if (jugadorCerca && puedeDormir)
        {
            PressE_UI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(SleepSequence());
            }
        }
        else
        {
            PressE_UI.SetActive(false);
        }
        if (puedeDormir)
        {
            Outline.SetActive(true);
        }
        else
        {
            Outline.SetActive(false);

        }
        //si queres dormir de dia
        if (jugadorCerca && !puedeDormir)
        {
            PressE_UI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!infoOpen)
                {
                    if (!GameManager.Instance.TryLockUI()) return;

                    CantSleepYetUI.SetActive(true);
                    infoOpen = true;
                }
                else
                {
                    GameManager.Instance.UnlockUI();
                    CantSleepYetUI.SetActive(false);
                    infoOpen = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    private IEnumerator SleepSequence()
    {
        if (movementLocker != null)
        {
            movementLocker.LockMovement();
        }

        fadeScript.FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        ParanoiaManager.Instance.SetParanoiaValueDirect(0f);
        DaysManager.Instance.NextDay();
        timeManager.TimeOfDay = 6f;
        timeManager.tiempoPausado = false;

        OnPlayerSlept?.Invoke();

        fadeScript.FadeOut();

        if (movementLocker != null)
        {
            movementLocker.UnlockMovement();
        }
    }
}