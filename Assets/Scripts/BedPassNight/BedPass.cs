using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedPass : MonoBehaviour
{
    public GameObject dormirUI;
    public string escenaFinalDelDia = "FinalScene";
    public float horaDormir = 22f;

    public LightingManager lightingManager;
    private bool jugadorCerca;
    private bool puedeDormir;
    void Start()
    {
        dormirUI.SetActive(false);
        lightingManager = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();
    }

    void Update()
    {
        if (lightingManager != null)
        {
            puedeDormir = lightingManager.TimeOfDay >= horaDormir || lightingManager.TimeOfDay < 4;
        }

        if (jugadorCerca && puedeDormir)
        {
            dormirUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {

                int currentIndex = SceneManager.GetActiveScene().buildIndex;
                int nextIndex = currentIndex + 4;

                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextIndex);
                }
            }
        }
        else
        {
            dormirUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}


