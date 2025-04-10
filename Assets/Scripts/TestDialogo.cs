using System.Collections.Generic;
using UnityEngine;

public class TestDialogo : MonoBehaviour
{
    void Start()
    {
        List<DialogueOption> opciones = new List<DialogueOption>()
        {
            new DialogueOption("¿Escuchaste lo que le pasó a Ben?", true),
            new DialogueOption("¿Cómo está tu familia?", false),
            new DialogueOption("Estoy investigando algo...", true),
            new DialogueOption("Hace calor hoy, ¿no?", false)
        };

        FindObjectOfType<DialogoUI>().MostrarOpciones(opciones);
    }
}


