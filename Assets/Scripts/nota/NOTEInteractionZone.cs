using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTEInteractionZone : MonoBehaviour
{
    public bool jugadorDentro = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorDentro = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorDentro = false;
    }

    private void OnDisable()
    {
        jugadorDentro = false;
    }
}
