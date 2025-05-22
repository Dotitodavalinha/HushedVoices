using System.Collections.Generic;
using UnityEngine;

public class TextDialoguePolice : MonoBehaviour
{
    void Start()
    {
        List<DialogueOption> opciones = new List<DialogueOption>()
          {
             new DialogueOption("Buen dia oficial,¿Todo bien en las calles?", false),
          };

        FindObjectOfType<DialogoUIPoliceMan>().MostrarOpciones(opciones);
    }
}



