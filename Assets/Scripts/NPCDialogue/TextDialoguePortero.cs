using System.Collections.Generic;
using UnityEngine;

public class TextDialoguePortero : MonoBehaviour
{
    void Start()
    {
        List<DialogueOption> opciones = new List<DialogueOption>()
          {
             new DialogueOption("Buen dia, queria saber si tenia informacion sobre Ben", false),
          };

        FindObjectOfType<DialogoUIPortero>().MostrarOpciones(opciones);
    }
}



