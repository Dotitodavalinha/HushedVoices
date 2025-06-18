using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToStreet2 : MonoBehaviour
{
    [SerializeField] private GameObject[] objetosAApagar;

    private void OnEnable() //cuando se activa el GameObject
    {
        foreach (GameObject obj in objetosAApagar)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) // ir a la siguiente escena
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.LoadScene("Pueblo2");

    }

}
