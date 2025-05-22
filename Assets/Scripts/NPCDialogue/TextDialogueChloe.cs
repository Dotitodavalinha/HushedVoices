using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDialogueChloe : MonoBehaviour
{

    void Start()
    {
        List<DialogueOption> opciones = new List<DialogueOption>()
          {

             new DialogueOption("Hola ¿Puede ser un cafe?", true, true),
          };

        FindObjectOfType<DialogoUIChloe>().MostrarOpciones(opciones);
    }
}
