using System.Collections.Generic;
using UnityEngine;

public class TestDialogo : MonoBehaviour
{
    void Start()
    {
        List<DialogueOption> opciones = new List<DialogueOption>()
          {

             new DialogueOption("¿Viste a Ben últimamente?", true, true),
             new DialogueOption("¿Cómo van las cosas en casa?", false),
             new DialogueOption("¿Notaste algo raro en el pueblo?", true),
             new DialogueOption("Lindo día para un paseo, ¿no?", false)
          };

        FindObjectOfType<DialogoUI>().MostrarOpciones(opciones);
    }
}


