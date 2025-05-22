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

    //asigno la camara (atado con alambre)
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
              
                opcionesPanel.SetActive(false);
                cajaDialogo.SetActive(false);
                npcText.text = "";
                playerLocker.UnlockMovement();

                //atado con alambre
                freeLookCam.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (zonaInteraccion.jugadorDentro)
            {
                cajaDialogo.SetActive(true);
                opcionesPanel.SetActive(true);
                npcText.text = "Hola Luke!";
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
    public void MostrarSubOpciones1()
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<DialogueOption> subOpciones = new List<DialogueOption>()
    {
        new DialogueOption("Triple expresso", true),
        new DialogueOption("Cafe helado", true),
        new DialogueOption("No quiero nada", true)
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
    public void MostrarSubOpciones2()
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<DialogueOption> subOpciones = new List<DialogueOption>()
    {
        new DialogueOption("¿Sabes lo que le paso a Ben?", true)
    };

        foreach (DialogueOption sub in subOpciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = sub.text;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ManejarSubOpcion2(sub);
            });

        }
    }

    public void MostrarSubOpciones3()
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<DialogueOption> subOpciones = new List<DialogueOption>()
    {
        new DialogueOption("Gracias por la informacion", false),
        new DialogueOption("¿Sabes que era lo que queria?", true)
    };

        foreach (DialogueOption sub in subOpciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = sub.text;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ManejarSubOpcion3(sub);
            });

        }
    }

    public void MostrarSubOpciones4()
    {
        foreach (Transform child in opcionesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<DialogueOption> subOpciones = new List<DialogueOption>()
    {
        new DialogueOption("No, para nada, el esta bien", false),
        new DialogueOption("El no volvio y estoy buscandolo", true)
    };

        foreach (DialogueOption sub in subOpciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, opcionesPanel.transform);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = sub.text;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ManejarSubOpcion4(sub);
            });

        }
    }

    void ManejarSubOpcion(DialogueOption sub)
    {
        Debug.Log("Subopción elegida: " + sub.text);
        opcionesPanel.SetActive(false);
        npcText.text = "";

        if (sub.text.Contains("Triple"))
        {
            npcText.text = "Ahora lo preparo, ¿Algo mas?";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }
        else if (sub.text.Contains("Cafe"))
        {
            npcText.text = "Ahora lo preparo, ¿Algo mas?";

        }
        else if (sub.text.Contains("No quiero"))
        {
            npcText.text = "¿Necesitas algo entonces?";

        }

        if (sub.isVeryDangerous)
        {
            Debug.Log("¡Esta opción es MUY peligrosa! Mostrando opciones secundarias...");
            MostrarSubOpciones2();
            return;
        }

        if (sub.isDangerous)
        {
            Debug.Log("Opción peligrosa. El NPC podría reaccionar...");
        }

        if (sub.isVeryDangerous)
        {
            moodController.SetMoodAngry();
            MostrarSubOpciones2();
            return;
        }

        if (sub.isDangerous)
        {
            moodController.SetMoodAngry();
        }
        else
        {
            moodController.SetMoodHappy();
        }

       // opcionesPanel.SetActive(false);


        /*  if (sub.text.Contains("Insistir"))
          {
              moodController.SetMoodAngry();
          }
          else if (sub.text.Contains("Alejarse"))
          {
              moodController.SetMoodNormal();
          }*/
    }

    void ManejarSubOpcion2(DialogueOption sub)
    {
        Debug.Log("Subopción elegida: " + sub.text);
        opcionesPanel.SetActive(false);
        npcText.text = "";

        if (sub.text.Contains("Ben"))
        {
            npcText.text = "Se pidio un triple espresso y se fue a sentar a la mesa 2";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }

        if (sub.isVeryDangerous)
        {
            Debug.Log("¡Esta opción es MUY peligrosa! Mostrando opciones secundarias...");
            MostrarSubOpciones3();
            return;
        }

        if (sub.isDangerous)
        {
            Debug.Log("Opción peligrosa. El NPC podría reaccionar...");
        }

        if (sub.isVeryDangerous)
        {
            moodController.SetMoodAngry();
            MostrarSubOpciones3();
            return;
        }

        if (sub.isDangerous)
        {
            moodController.SetMoodAngry();
        }
        else
        {
            moodController.SetMoodHappy();
        }

        opcionesPanel.SetActive(false);


        /*  if (sub.text.Contains("Insistir"))
          {
              moodController.SetMoodAngry();
          }
          else if (sub.text.Contains("Alejarse"))
          {
              moodController.SetMoodNormal();
          }*/
    }

    void ManejarSubOpcion3(DialogueOption sub)
    {
        Debug.Log("Subopción elegida: " + sub.text);
        opcionesPanel.SetActive(false);
        npcText.text = "";

        if (sub.text.Contains("informacion"))
        {
            npcText.text = "Suerte";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }

        if (sub.text.Contains("Sabes"))
        {
            npcText.text = "Vino con una lista de personas pero solo me acuerdo de la primera, la maestra del colegio. ¿¡Le paso algo!?";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }

        if (sub.isVeryDangerous)
        {
            Debug.Log("¡Esta opción es MUY peligrosa! Mostrando opciones secundarias...");
            MostrarSubOpciones4();
            return;
        }

        if (sub.isDangerous)
        {
            Debug.Log("Opción peligrosa. El NPC podría reaccionar...");
        }

        if (sub.isVeryDangerous)
        {
            moodController.SetMoodAngry();
            MostrarSubOpciones4();
            return;
        }

        if (sub.isDangerous)
        {
            moodController.SetMoodAngry();
        }
        else
        {
            moodController.SetMoodHappy();
        }

        opcionesPanel.SetActive(false);


        /*  if (sub.text.Contains("Insistir"))
          {
              moodController.SetMoodAngry();
          }
          else if (sub.text.Contains("Alejarse"))
          {
              moodController.SetMoodNormal();
          }*/
    }

    void ManejarSubOpcion4(DialogueOption sub)
    {
        Debug.Log("Subopción elegida: " + sub.text);
        opcionesPanel.SetActive(false);
        npcText.text = "";

        if (sub.text.Contains("para nada"))
        {
            npcText.text = "Ah me habias asustado, termina tu cafe porfavor";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }

        if (sub.text.Contains("no volvio"))
        {
            npcText.text = "NO, POBRE BEN QUE LE HABRA OCURRIDO!!!!";
            // sumar alerta, activar eventos, etc.
            // 
            // GameManager.Instance.AumentarAlerta(1);
        }

        opcionesPanel.SetActive(false);

    }


    void OnElegirOpcion(DialogueOption opcion)
    {
        Debug.Log("Elegiste: " + opcion.text);

        if (opcion.text.Contains("cafe"))
        {
            npcText.text = "¿Que te gustaria pedir?";
        }
       


        if (opcion.isVeryDangerous)
        {
            Debug.Log("¡Esta opción es MUY peligrosa! Mostrando opciones secundarias...");
            MostrarSubOpciones1();
            return;
        }

        if (opcion.isDangerous)
        {
            Debug.Log("Opción peligrosa. El NPC podría reaccionar...");
        }

        if (opcion.isVeryDangerous)
        {
            moodController.SetMoodAngry();
            MostrarSubOpciones1();
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

     //   opcionesPanel.SetActive(false);

    }

}
