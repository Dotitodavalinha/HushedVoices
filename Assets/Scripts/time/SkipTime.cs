using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipTime : MonoBehaviour
{
    [SerializeField] private LightingManager lightingManager;

    private float day = 5f;
    private float afternoon = 14f;
    private float night = 20f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        BuscarTimeManager();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuscarTimeManager();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTime(day);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTime(afternoon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTime(night);
        }
    }

    void SetTime(float hour)
    {
        if (lightingManager == null)
        {
            Debug.LogWarning("LightingManager no está asignado en SkipTime");
            return;
        }

        lightingManager.TimeOfDay = hour;
        lightingManager.tiempoPausado = false;
    }

    void BuscarTimeManager()
    {
        lightingManager = FindObjectOfType<LightingManager>();

        if (lightingManager == null)
        {
            Debug.LogWarning("LightingManager no encontrado en la escena actual.");
        }
    }
}
