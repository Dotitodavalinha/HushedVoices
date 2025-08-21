using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDayBackToMenu : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private LightingManager lightingManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        lightingManager = FindObjectOfType<LightingManager>();
    }

    public void LoadScene(string sceneName)
    {
        gameManager.uiLockCount = 0; // Aseguramos que el contador de UI se reinicie
        SceneManager.LoadScene(sceneName);
    }



    public void NewGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
       
    }

    public void QuitGame()
    {
        gameManager.uiLockCount = 0; // Aseguramos que el contador de UI se reinicie
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

