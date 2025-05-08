using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogoUI : MonoBehaviour
{
    public GameObject opcionesPanel;
    public GameObject botonOpcionPrefab;

    [SerializeField] private PlayerMovementLocker playerLocker;
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    public TMP_Text npcText;

    private List<DialogueOption> opcionesOriginales = new List<DialogueOption>();

    //asigno la camara (atado con alambre)
    public Cinemachine.CinemachineFreeLook freeLookCam;


    [SerializeField] private NPCMoodController moodController;
    void Start()
    {
        opcionesPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (opcionesPanel.activeSelf)
            {
              
                opcionesPanel.SetActive(false);
                npcText.text = "";
                playerLocker.UnlockMovement();

                //atado con alambre
                freeLookCam.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (zonaInteraccion.jugadorDentro)
            {
               
                opcionesPanel.SetActive(true);
                npcText.text = "¿Qué necesitás?";
                MostrarOpciones(opcionesOriginales);

                playerLocker.LockMovement();
            }
        }
    }


    public void MostrarOpciones(List<DialogueOption> opciones)
    {
        opcionesOriginales = opciones;
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueOption opcion in opciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = opcion.text;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnElegirOpcion(opcion);
            });
        }
    }
    public void MostrarSubOpcionesPeligrosas()
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<DialogueOption> subOpciones = new List<DialogueOption>()
    {
        new DialogueOption("Insistir", true),
        new DialogueOption("Alejarse sin molestar", false)
    };

        foreach (DialogueOption sub in subOpciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = sub.text;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ManejarSubOpcion(sub);
            });

        }
    }

    void ManejarSubOpcion(DialogueOption sub)
    {
        Debug.Log("Subopción elegida: " + sub.text);
        opcionesPanel.SetActive(false);
        npcText.text = "";

        if (sub.text.Contains("Insistir"))
        {
            npcText.text = "¡Bajá la voz! ¿Querés que nos escuchen?";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }
        else if (sub.text.Contains("Alejarse"))
        {
            npcText.text = "Que tengas buen dia";
            
        }


        if (sub.text.Contains("Insistir"))
        {
            moodController.SetMoodAngry();
        }
        else if (sub.text.Contains("Alejarse"))
        {
            moodController.SetMoodNormal();
        }
    }


    void OnElegirOpcion(DialogueOption opcion)
    {
        Debug.Log("Elegiste: " + opcion.text);

        if (opcion.text.Contains("Ben"))
        {
            npcText.text = "Ben...? Me pareció verlo ir hacia la cafetería hace un par de días. Estaba apurado.";
        }
        else if (opcion.text.Contains("casa"))
        {
            npcText.text = "Estamos bien, gracias... Aunque últimamente no dormimos tranquilos.";
        }
        else if (opcion.text.Contains("raro"))
        {
            npcText.text = "Todo se siente extraño últimamente. Hay caras nuevas rondando.";
        }
        else if (opcion.text.Contains("paseo"))
        {
            npcText.text = "Sí... aunque uno ya ni se siente libre para caminar.";
        }



        if (opcion.isVeryDangerous)
        {
            Debug.Log("¡Esta opción es MUY peligrosa! Mostrando opciones secundarias...");
            MostrarSubOpcionesPeligrosas();
            return;
        }

        if (opcion.isDangerous)
        {
            Debug.Log("Opción peligrosa. El NPC podría reaccionar...");
        }

        if (opcion.isVeryDangerous)
        {
            moodController.SetMoodAngry();
            MostrarSubOpcionesPeligrosas();
            return;
        }

        if (opcion.isDangerous)
        {
            moodController.SetMoodAngry();
        }
        else
        {
            moodController.SetMoodHappy();
        }

        opcionesPanel.SetActive(false);

    }

}
