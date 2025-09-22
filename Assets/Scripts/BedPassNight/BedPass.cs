using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedPass : MonoBehaviour
{
    public GameObject dormirUI;
    public GameObject Outline;
    public string escenaFinalDelDia = "FinalScene";
    public float horaDormir = 22f;

    [SerializeField] private float fadeDuration = 1f;


    public LightingManager timeManager;
    private bool jugadorCerca;
    private bool puedeDormir;

    public static event Action OnPlayerSlept;

    [SerializeField] private Fade fadeScript;


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
                StartCoroutine(SleepSequence());
            }
        }
        else
        {
            dormirUI.SetActive(false);
        }
        if (puedeDormir)
        {
            Outline.SetActive(true);
        }
        else
        {
            Outline.SetActive(false);

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

    private IEnumerator SleepSequence()
    {
        fadeScript.FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        ParanoiaManager.Instance.SetParanoiaValueDirect(0f);
        DaysManager.Instance.NextDay();
        timeManager.TimeOfDay = 6f;
        timeManager.tiempoPausado = false;

        OnPlayerSlept?.Invoke();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        fadeScript.FadeOut();
    }
}