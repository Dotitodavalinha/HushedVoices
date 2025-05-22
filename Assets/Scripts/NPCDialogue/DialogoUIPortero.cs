using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogoUIPortero : MonoBehaviour
{
    public GameObject opcionesPanel;
    public GameObject botonOpcionPrefab;
    public GameObject cajaDialogo;

    [SerializeField] private PlayerMovementLocker playerLocker;
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    public TMP_Text npcText;

    public Cinemachine.CinemachineFreeLook freeLookCam;

    private List<DialogueOption> opcionesOriginales = new List<DialogueOption>();
  
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
                AbrirDialogoInicial();
            }
        }
    }

    void AbrirDialogoInicial()
    {
        cajaDialogo.SetActive(true);
        opcionesPanel.SetActive(true);
        npcText.text = "Buenas señor, ¿busca a alguien?";
        opcionesOriginales = new List<DialogueOption>
        {
            new DialogueOption("¿Viste a Ben?")
        };
        MostrarOpciones(opcionesOriginales);

        playerLocker.LockMovement();
        freeLookCam.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MostrarOpciones(List<DialogueOption> opciones)
    {
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

    void OnElegirOpcion(DialogueOption opcion)
    {
        if (opcion.text.Contains("Ben"))
        {
            npcText.text = "Oh, hace poco escuché de él. Una de las maestras lo nombró, pero no estaría seguro.";
        }
        Invoke("CerrarDialogo", 2.5f); // cierra luego de mostrar respuesta
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
}

