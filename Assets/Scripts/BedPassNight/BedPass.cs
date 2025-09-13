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
                ParanoiaManager.Instance.SetParanoiaValueDirect(0f);

                // pasamos al siguiente día
                DaysManager.Instance.NextDay();

                //reseteamos el tiempo para la mañana
                timeManager.TimeOfDay = 6f;
                timeManager.tiempoPausado = false;

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