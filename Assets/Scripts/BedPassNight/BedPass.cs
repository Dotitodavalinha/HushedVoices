using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedPass : MonoBehaviour
{
    public GameObject dormirUI;
    public string escenaFinalDelDia = "FinalScene";
    public float horaDormir = 22f;

    public LightingManager timeManager;
    private bool jugadorCerca;
    private bool puedeDormir;



    void Start()
    {
        dormirUI.SetActive(false);
        timeManager = GameObject.Find("TimeManager")?.GetComponent<LightingManager>();
    }

    void Update()
    {
        if (timeManager != null)
        {
            puedeDormir = timeManager.TimeOfDay >= horaDormir || timeManager.TimeOfDay < 4;
        }


        if (jugadorCerca && puedeDormir)
        {
            dormirUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ParanoiaManager.Instance.SetParanoiaValue(-1f);
                timeManager.tiempoPausado = false;
                timeManager.TimeOfDay = 6f;
                int currentIndex = SceneManager.GetActiveScene().buildIndex;
                int nextIndex = currentIndex + 4;

                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextIndex);
                }
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
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


