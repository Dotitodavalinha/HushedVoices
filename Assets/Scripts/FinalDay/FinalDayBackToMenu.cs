using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalDayBackToMenu : MonoBehaviour
{
    public GameManager gameManager;
    public ProgressManager progressManager;
    [SerializeField] private LightingManager lightingManager;
    public ConcentrationManager concentrationManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        lightingManager = FindObjectOfType<LightingManager>();
        progressManager = ProgressManager.Instance;
        concentrationManager = ConcentrationManager.Instance;
    }

    public void LoadScene(string sceneName)
    {
        gameManager.uiLockCount = 0; // Aseguramos que el contador de UI se reinicie
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(string sceneName)
    {
        ResetAllManagers();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        ResetAllManagers();

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void ResetAllManagers()
    {
        Debug.Log("--- INICIANDO RESETEO TOTAL ---");

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetGameProgress();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.uiLockCount = 0;
            GameManager.Instance.ResetProgress();
            if (PlayerClueTracker.Instance != null)
            {
                PlayerClueTracker.Instance.clues.Clear();
                PlayerClueTracker.Instance.cluesList.Clear();

            }
            if (ConcentrationManager.Instance != null)
                ConcentrationManager.Instance.RefillUses();

            if (lightingManager != null)
                lightingManager.ResetTime();
            else
            {
                var light = FindObjectOfType<LightingManager>();
                if (light != null) light.ResetTime();
            }

            ParanoiaManager paranoiaManager = FindObjectOfType<ParanoiaManager>();
            if (paranoiaManager != null) paranoiaManager.ResetManager();

            NightManager nightManager = FindObjectOfType<NightManager>();
            if (nightManager != null) nightManager.ResetManager();

            JailManager jailManager = FindObjectOfType<JailManager>();
            if (jailManager != null)
            {
                jailManager.counter = 0;
                if (jailManager.objectToActivate != null)
                    jailManager.objectToActivate.SetActive(false);
            }

            if (NPCMoodManager.Instance != null)
                NPCMoodManager.Instance.ResetAllMoods();
        }

    }

}
