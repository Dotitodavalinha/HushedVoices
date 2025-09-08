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

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        lightingManager = FindObjectOfType<LightingManager>();
        progressManager = FindObjectOfType<ProgressManager>();
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

        if (PlayerClueTracker.Instance != null)
        {
            PlayerClueTracker.Instance.clues.Clear();
            PlayerClueTracker.Instance.cluesList.Clear();
        }

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

        // Resetea progreso
        if (progressManager != null)
            ProgressManager.Instance.CambiarRootNPC("PolicemanZ", "RootPoliceZ0");
        progressManager.ResetNPCRoots();
            progressManager.ResetAllBools();

        // UI Lock
        if (gameManager != null)
            gameManager.uiLockCount = 0;

        // Ciclo día/noche
        if (lightingManager != null)
            lightingManager.ResetTime();

        // Paranoia
        ParanoiaManager paranoiaManager = FindObjectOfType<ParanoiaManager>();
        if (paranoiaManager != null)
            paranoiaManager.ResetManager();

        // NightManager
        NightManager nightManager = FindObjectOfType<NightManager>();
        if (nightManager != null)
            nightManager.ResetManager();

        // JailManager
        JailManager jailManager = FindObjectOfType<JailManager>();
        if (jailManager != null)
        {
            jailManager.counter = 0;
            if (jailManager.objectToActivate != null)
                jailManager.objectToActivate.SetActive(false);
        }
      
        // Clues
        if (PlayerClueTracker.Instance != null)
        {
            PlayerClueTracker.Instance.clues.Clear();
            PlayerClueTracker.Instance.cluesList.Clear();
        }
       
        NPCMoodManager.Instance.ResetAllMoods();
    }
}

