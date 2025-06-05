using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPoliceZ : MonoBehaviour
{
    [SerializeField] private GameObject ChangeDialoguePolicez;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ProgressManager.Instance.Policez = true;
        ProgressManager.Instance.CambiarRootNPC("Chloe", "Root"); //testeando cambio(funciono)
        ProgressManager.Instance.ChangeDialoguePolicez = true;
        ChangeDialoguePolicez.SetActive(ProgressManager.Instance.ChangeDialoguePolicez);

    }
}
