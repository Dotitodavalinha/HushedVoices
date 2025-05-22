using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderDialogos : MonoBehaviour
{

    public bool jugadorDentro = false;

    [SerializeField] GameObject PoliceManager;
    [SerializeField] GameObject NPCManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            PoliceManager.SetActive(true);
            NPCManager.SetActive(false);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            PoliceManager.SetActive(false);
            NPCManager.SetActive(true);
        }
    }
}
