using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogoUIChloe : MonoBehaviour
{
    public GameObject opcionesPanel;
    public GameObject botonOpcionPrefab;
    public GameObject cajaDialogo;

    [SerializeField] private PlayerMovementLocker playerLocker;
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    public TMP_Text npcText;

    private List<DialogueOption> opcionesOriginales = new List<DialogueOption>();
    public Cinemachine.CinemachineFreeLook freeLookCam;
    [SerializeField] private NPCMoodController moodController;

    void Start()
    {
        opcionesPanel.SetActive(false);
        cajaDialogo.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (opcionesPanel.activeSelf)
            {
                CerrarDialogo();
            }
            else if (zonaInteraccion.jugadorDentro)
            {
                AbrirDialogo("Hola Luke!", opcionesOriginales);
            }
        }
    }

    void CerrarDialogo()
    {
        opcionesPanel.SetActive(false);
        cajaDialogo.SetActive(false);
        npcText.text = "";
        playerLocker.UnlockMovement();
        freeLookCam.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void AbrirDialogo(string texto, List<DialogueOption> opciones)
    {
        cajaDialogo.SetActive(true);
        opcionesPanel.SetActive(true);
        npcText.text = texto;
        MostrarOpciones(opciones);
        playerLocker.LockMovement();
        freeLookCam.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    void MostrarSubOpciones(List<DialogueOption> subOpciones, System.Action<DialogueOption> callback)
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueOption sub in subOpciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = sub.text;
            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                callback(sub);
            });
        }
    }

    public void MostrarSubOpciones1()
    {
        MostrarSubOpciones(new List<DialogueOption>
        {
            new DialogueOption("Triple expresso", false),
            new DialogueOption("Cafe helado", false),
            new DialogueOption("No quiero nada", false)
        }, ManejarSubOpcion);
    }

    public void MostrarSubOpciones2()
    {
        MostrarSubOpciones(new List<DialogueOption>
        {
            new DialogueOption("¿Sabes lo que le paso a Ben?", true)
        }, ManejarSubOpcion2);
    }

    public void MostrarSubOpciones3()
    {
        MostrarSubOpciones(new List<DialogueOption>
        {
            new DialogueOption("Gracias por la informacion", false),
            new DialogueOption("¿Sabes que era lo que queria?", true)
        }, ManejarSubOpcion3);
    }

    public void MostrarSubOpciones4()
    {
        MostrarSubOpciones(new List<DialogueOption>
        {
            new DialogueOption("No, para nada, el esta bien", false),
            new DialogueOption("El no volvio y estoy buscandolo", true)
        }, ManejarSubOpcion4);
    }

    void ProcesarReaccion(DialogueOption sub)
    {
        if (sub.isVeryDangerous)
        {
            moodController.SetMoodAngry();
        }
        else if (sub.isDangerous)
        {
            moodController.SetMoodAngry();
        }
        else
        {
            moodController.SetMoodHappy();
        }
    }

    void ManejarSubOpcion(DialogueOption sub)
    {
        npcText.text = sub.text.Contains("No quiero") ? "¿Necesitas algo entonces?" : "Ahora lo preparo, ¿Algo mas?";
        if (sub.text.Contains("Triple"))
        {
            moodController.SetMoodHappy();
        }
        else if (sub.text.Contains("Cafe"))
        {
            moodController.SetMoodHappy();
        }
       else if (sub.text.Contains("No quiero"))
        {
            moodController.SetMoodHappy();
        }
        ProcesarReaccion(sub);
        MostrarSubOpciones2();
    }

    void ManejarSubOpcion2(DialogueOption sub)
    {
        npcText.text = "Se pidio un triple espresso y se fue a sentar a la mesa 2";
        ProcesarReaccion(sub);
        MostrarSubOpciones3();
        if (sub.text.Contains("Ben"))
        {
            moodController.SetMoodHappy();
        }
    }

    void ManejarSubOpcion3(DialogueOption sub)
    {
        if (sub.text.Contains("informacion"))
        {
            npcText.text = "Suerte";
        ProcesarReaccion(sub);
        CerrarDialogo();
        }
        else
        {
            npcText.text = "Vino con una lista de personas pero solo me acuerdo de la primera, la maestra del colegio. ¿¡Le paso algo!?";

        ProcesarReaccion(sub);
        MostrarSubOpciones4();
        }
        if (sub.text.Contains("Gracias"))
        {
            moodController.SetMoodHappy();
        }
    }

    void ManejarSubOpcion4(DialogueOption sub)
    {
        npcText.text = sub.text.Contains("para nada")
            ? "Ah me habias asustado, termina tu cafe porfavor"
            : "NO, POBRE BEN QUE LE HABRA OCURRIDO!!!!";

        ProcesarReaccion(sub);
        CerrarDialogo();
        if (sub.text.Contains("no volvio"))
        {
            moodController.SetMoodAngry();
        }
    }

    void OnElegirOpcion(DialogueOption opcion)
    {
        Debug.Log("Elegiste: " + opcion.text);

        if (opcion.text.ToLower().Contains("cafe"))
        {
            npcText.text = "¿Que te gustaria pedir?";
            MostrarSubOpciones1();
        }
       
        ProcesarReaccion(opcion);
        if (opcion.text.Contains("Hola"))
        {
            moodController.SetMoodHappy();
        }

    }
}