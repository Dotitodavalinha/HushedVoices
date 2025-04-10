using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogoUI : MonoBehaviour
{
    public GameObject opcionesPanel;
    public GameObject botonOpcionPrefab;

    [SerializeField]public NPCInteractionZone zonaInteraccion;

    void Start()
    {
        opcionesPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && zonaInteraccion.jugadorDentro == true)
        {
            opcionesPanel.SetActive(true);
        }
        if (zonaInteraccion.jugadorDentro == false)
        {
            opcionesPanel.SetActive(false);
        }
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
        Debug.Log("Elegiste: " + opcion.text);

        if (opcion.isDangerous)
        {
            Debug.Log(" Opción peligrosa. El NPC podría reaccionar...");
        }

        opcionesPanel.SetActive(false); //cerramos panel
    }
}
