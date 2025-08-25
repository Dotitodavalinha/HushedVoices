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


    private void ResetAllManagers()
    {
        if (progressManager != null)
            progressManager.ResetAllBools();

        if (gameManager != null)
        {
            gameManager.uiLockCount = 0;
        }

        if (lightingManager != null)
        {
            lightingManager.ResetTime();
        }
    }
}

