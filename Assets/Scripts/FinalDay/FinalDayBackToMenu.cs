using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDayBackToMenu : MonoBehaviour
{
    [SerializeField] private LightingManager lightingManager;
   
    private void Start()
    {
        lightingManager = FindObjectOfType<LightingManager>();      
    }
    public void LoadScene(string sceneName)
    {
        lightingManager.TimeOfDay = 5.22f;
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }



    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
