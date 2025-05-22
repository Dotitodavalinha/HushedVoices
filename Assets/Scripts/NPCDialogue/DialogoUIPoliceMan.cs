using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogoUIPoliceMan : MonoBehaviour
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
    /*  public void MostrarSubOpcionesPeligrosas()
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
      }*/

    /*  void ManejarSubOpcion(DialogueOption sub)
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
  */

    void OnElegirOpcion(DialogueOption opcion)
    {
        Debug.Log("Elegiste: " + opcion.text);

        if (opcion.text.Contains("bien"))
        {
            npcText.text = "Si, Alejate que estoy trabajando";
        }

        opcionesPanel.SetActive(false);
    }

}
