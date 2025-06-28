using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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


        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }

    }

}
